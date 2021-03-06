﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinionField : MonoBehaviour
{
    public static EnemyMinionField instance;

    [Range(0,7)]
    public int minionNum;

    [Range(0, 1920)]
    public float minionDistance;

    [Space(20)]
    [Header("하수인 배치시 속도")]
    [Range(0, 1920)]
    public float min_speed;
    [Range(0, 1920)]
    public float max_speed;

    [Space(20)]
    [Header("하수인 공격시 처리")]
    [Range(0, 1920)]
    public float min_attack_speed;
    [Range(0, 1920)]
    public float max_attack_speed;
    [Range(0, 1920)]
    public float attack_range;
    public float attack_delay;

    [Header("-----------------------------------------")]
    [Space(20)]

    public MinionObject[] minions = new MinionObject[7];
    Vector3[] minions_pos = new Vector3[7];
    [HideInInspector] public Vector3[] minions_Attack_pos = new Vector3[7];
    [HideInInspector] public float[] minions_attack_delay = new float[7];
    [HideInInspector] public float attack_ready = 1f;
    public Transform AI_MousePos;
    [HideInInspector] public int mousePos;

    List<int> spawnList = new List<int>();
    bool spawnNow;
    [HideInInspector] public bool setMinionPos = true;

    #region[Awake]
    private void Awake()
    {
        instance = this;
        for (int i = 0; i < minions_pos.Length; i++)
            minions_pos[i] = new Vector3();
        for (int i = 0; i < minions_Attack_pos.Length; i++)
            minions_Attack_pos[i] = new Vector3();
    }
    #endregion

    #region[Update]
    void Update()
    {
        MouseCheck();
        SetPos();
        MoveMinion();
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[생성 위치 검사]
    void MouseCheck()
    {
        mousePos = minionNum;
        float minX = minionDistance * (minionNum - 1) / 2f;
        for (int i = 0; i < minionNum + 1; i++)
        {
            Vector3 v = new Vector3(transform.position.x - minX + minionDistance * i, transform.position.y, transform.position.z);
            v = Camera.main.WorldToScreenPoint(v);
            if(AI_MousePos.position.x < v.x)
            {
                mousePos = i;
                break;
            }
        }
    }
    #endregion

    #region[미니언이 공격중인지 검사]
    public bool MinionAttackCheck()
    {
        for (int i = 0; i < 7; i++)
            if (minions[i].gameObject.activeSelf && minions_Attack_pos[i] != Vector3.zero)
                return true;
        for (int i = 0; i < 7; i++)
            if (minions[i].gameObject.activeSelf && minions[i].animator.GetCurrentAnimatorStateInfo(0).IsName("하수인제거"))
                return true;
        return false;
    }
    #endregion

    #region[미니언 위치 설정]
    void SetPos()
    {
        if (!setMinionPos)
            return;
        //새로 설치할 미니언에 따른 위치 설정
        //bool flag = (minionNum == 7);   //미니언을 설치할 위치가 없을때
        //flag = flag || (DragCardObject.instance.dragCardView.cardType != CardType.하수인);   //하수인을 드래그하는게 아닐때
        //flag = flag || (!DragCardObject.instance.mouseInField);   //필드로 뭔가를 드래그하는게 아닐때

        bool flag = true;
        if (flag)
        {
            float minX = minionDistance * (minionNum - 1) / 2f;
            for (int i = 0; i < 7; i++)
            {
                Vector3 v = new Vector3(transform.position.x - minX + minionDistance * i, transform.position.y, transform.position.z);
                minions_pos[i] = v;
            }
        }
        else
        {
            float minX = minionDistance * (minionNum) / 2f;
            for (int i = 0; i < 7; i++)
            {
                int newP = (i >= mousePos) ? i + 1 : i;
                Vector3 v = new Vector3(transform.position.x - minX + minionDistance * newP, transform.position.y, transform.position.z);
                minions_pos[i] = v;
            }
        }

        //공격위치로 변경
        for (int i = 0; i < 7; i++)
        {
            if (minions_Attack_pos[i] != Vector3.zero)
            {
                if (Vector2.Distance(minions_Attack_pos[i], minions[i].transform.position) < attack_range)
                {
                    if (minions_attack_delay[i] < 0)
                    {
                        minions[i].canAttackNum--;
                        minions_Attack_pos[i] = Vector3.zero;
                    }
                    else
                    {
                        minions[i].animator.SetBool("Attack", false);
                        AttackManager.instance.AttackEffectRun();
                        minions_attack_delay[i] -= Time.deltaTime;
                    }
                }
                else
                {
                    minions[i].animator.SetBool("Attack", true);
                    minions_attack_delay[i] = attack_delay;
                }

                if (minions_Attack_pos[i] != Vector3.zero)
                    minions_pos[i] = new Vector3(minions_Attack_pos[i].x, minions_Attack_pos[i].y, transform.position.z);
            }
        }
    }
    #endregion

    #region[미니언 이동]
    void MoveMinion()
    {
        for (int i = 0; i < minions.Length; i++)
            minions[i].gameObject.SetActive(i < minionNum);

        if (!setMinionPos)
            return;

        min_speed = Mathf.Min(min_speed, max_speed);

        if (attack_ready > 0)
        {
            attack_ready -= Time.deltaTime;
            return;
        }

        for (int i = 0; i < minionNum; i++)
        {
            //일반적인 위치 조정
            if (Mathf.Abs(transform.position.y - minions[i].transform.position.y) < 1)
            {
                Vector2 v = minions_pos[i] - minions[i].transform.position;
                v *= Time.deltaTime * Vector2.Distance(minions_pos[i], minions[i].transform.position);

                if (Vector2.Distance(Vector3.zero, v) > Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * max_speed))
                    v = v.normalized * Time.deltaTime * max_speed;

                if (Vector2.Distance(Vector3.zero, v) < Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * min_speed))
                    v = v.normalized * Time.deltaTime * min_speed;
                if (Vector2.Distance(minions_pos[i], minions[i].transform.position) < Vector2.Distance(Vector3.zero, v))
                    minions[i].transform.position = new Vector3(minions_pos[i].x, minions_pos[i].y, minions[i].transform.position.z);
                else
                    minions[i].transform.position += new Vector3(v.x, v.y, 0);
            }
            //공격시 위치이동
            else if ((minions_attack_delay[i] == attack_delay) || (minions_attack_delay[i] <= 0))
            {
                minions[i].transform.localPosition = new Vector3(minions[i].transform.localPosition.x, minions[i].transform.localPosition.y, -140);
                Vector2 v = minions_pos[i] - minions[i].transform.position;
                v *= Time.deltaTime * Vector2.Distance(minions_pos[i], minions[i].transform.position);
                if (Vector2.Distance(Vector3.zero, v) > Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * max_attack_speed))
                    v = v.normalized * Time.deltaTime * max_attack_speed;
                if (Vector2.Distance(Vector3.zero, v) < Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * min_attack_speed))
                    v = v.normalized * Time.deltaTime * min_attack_speed;

                if (minions_pos[i].y < minions[i].transform.position.y)
                {
                    if (Mathf.Abs(minions_pos[i].y - minions[i].transform.position.y) < Mathf.Abs(minions_pos[i].y - transform.position.y) * 0.95f)
                    {
                        float mA = Mathf.Abs(transform.position.y - minions_pos[i].y);
                        float mB = Mathf.Abs(minions_pos[i].y - minions[i].transform.position.y);
                        float mC = (mA - mB) / mA;
                        mC = (1f - mC) * 2;
                        v = v.normalized * Time.deltaTime * max_attack_speed * Mathf.Max(1.2f, mC);
                    }
                    else
                        v = v.normalized * Time.deltaTime * min_attack_speed;
                }

                if (Vector2.Distance(minions_pos[i], minions[i].transform.position) >= Vector2.Distance(Vector3.zero, v))
                    minions[i].transform.position += new Vector3(v.x, v.y, 0);
            }
        }
    }
    #endregion

    #region[미니언 추가]
    public void AddMinion(int n, string name, bool cardHandSpawn,int spawnAni = 0)
    {
        if (minionNum == 7)
            return;
        if (cardHandSpawn)
            GameEventManager.instance.EventAdd(2f);
        MinionObject temp = minions[6];
        for (int i = 5; i >= n; i--)
            minions[i + 1] = minions[i];
        minions[n] = temp;
        minions[n].minion_name = name;
        minions[n].InitTrigger = true;

        minionNum++;

        //새로운 미니언 위치 지정
        float minX = minionDistance * (minionNum - 1) / 2f;
        Vector3 v = new Vector3(transform.position.x - minX + minionDistance * n, transform.position.y, minions[n].transform.position.z);
        minions[n].transform.position = v;
        if (cardHandSpawn && spawnAni != 2)
            DragCardObject.instance.ShowDropEffectMinion(Camera.main.WorldToScreenPoint(BattleUI.instance.enemySpellPos.transform.position),Camera.main.WorldToScreenPoint(v), 1);
        else if (cardHandSpawn && spawnAni == 2)
            EffectManager.instance.FireField();
        StartCoroutine(MinionDrop(n, spawnAni, cardHandSpawn));
    }

    private IEnumerator MinionDrop(int n, int spawnType, bool cardHandSpawn)
    {
        while (!minions[n].animator.gameObject.activeSelf)
            yield return new WaitForSeconds(0.001f);
        if (cardHandSpawn)
        {
            while (!DragCardObject.instance.dropEffect.effectArrive)
                yield return new WaitForSeconds(0.1f);
            if (spawnType == 0)
            {
                SoundManager.instance.PlaySE("미니언소환일반");
                minions[n].animator.SetTrigger("NormalSpawn");
            }
            else if (spawnType == 1)
            {
                SoundManager.instance.PlaySE("미니언소환일반");
                minions[n].animator.SetTrigger("SetSpawn");
            }
            else if (spawnType == 2)
            {
                minions[n].animator.SetTrigger("DeathWingSpawn");
                EffectManager.instance.VibrationEffect(2f, 20, 10);
            }
            else if (spawnType == 3)
            {
                minions[n].animator.SetTrigger("StrongSpawn");
                EffectManager.instance.VibrationEffect(0.5f, 15, 4);
            }
            while (!minions[n].animator.GetCurrentAnimatorStateInfo(0).IsName("하수인소환완료"))
                yield return new WaitForSeconds(0.1f);
            minions[n].CardHandMinionSpawn();
            if (GameEventManager.instance.GetEventValue() > 1.5f)
                GameEventManager.instance.EventSet(1.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            if (spawnType == 0)
                minions[n].animator.SetTrigger("NormalSpawn");
            else if (spawnType == 1)
                minions[n].animator.SetTrigger("SetSpawn");
            else if (spawnType == 2)
            {
                minions[n].animator.SetTrigger("DeathWingSpawn");
                EffectManager.instance.VibrationEffect(2f, 20, 10);
            }
            else if (spawnType == 3)
            {
                minions[n].animator.SetTrigger("StrongSpawn");
                EffectManager.instance.VibrationEffect(0.5f, 15, 4);
            }
        }
    }

    #endregion

    #region[미니언 턴 시작시 처리]
    public void MinionsTurnStartTrigger()
    {
        for (int i = 0; i < minionNum; i++)
            minions[i].turnStartTrigger = true;

    }
    #endregion

    #region[미니언 턴 종료시 처리]
    public void MinionsTurnEndTrigger()
    {
        for (int i = 0; i < minionNum; i++)
            minions[i].turnEndTrigger = true;

    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[OnDrawGizmos]
    private void OnDrawGizmos()
    {
        float minX = minionDistance * (minionNum - 1)/2f;
        for (int i = 0; i < minionNum; i++)
        {
            Vector3 v = new Vector3(transform.position.x - minX + minionDistance * i, transform.position.y, transform.position.z);
            Gizmos.DrawSphere(v, 20);
        }
    }
    #endregion
}
