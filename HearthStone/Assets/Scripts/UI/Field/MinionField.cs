using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionField : MonoBehaviour
{
    public enum SpawnAni
    {
        Normal,
        Set,
        DeathWing,
        Strong
    }

    public static MinionField instance;
    private const int MINION_SLOT_NUM = 7;

    [Range(0, MINION_SLOT_NUM)]
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

    public MinionObject[] minions = new MinionObject[MINION_SLOT_NUM];
    private Vector3[] minions_pos = new Vector3[MINION_SLOT_NUM];
    [HideInInspector] public Vector3[] minions_Attack_pos = new Vector3[MINION_SLOT_NUM];
    [HideInInspector] public float[] minions_attack_delay = new float[MINION_SLOT_NUM];
    [HideInInspector] public float attack_ready;
    [HideInInspector] public int mousePos;
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

    #region[마우스 위치 검사]
    void MouseCheck()
    {
        mousePos = minionNum;
        float minX = minionDistance * (minionNum - 1) / 2f;
        for (int i = 0; i < minionNum + 1; i++)
        {
            Vector3 v = new Vector3(transform.position.x - minX + minionDistance * i,
                transform.position.y, transform.position.z);
            v = Camera.main.WorldToScreenPoint(v);
            if (Input.mousePosition.x < v.x)
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
        bool flag = FullField();   //미니언을 설치할 위치가 없을때
        flag = flag || (DragCardObject.instance.dragCardView.cardType != CardType.하수인);   //하수인을 드래그하는게 아닐때
        flag = flag || (!DragCardObject.instance.mouseInField);   //필드로 뭔가를 드래그하는게 아닐때

        //일반적인위치지정
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
        min_attack_speed = Mathf.Min(min_attack_speed, max_attack_speed);

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
                minions[i].transform.localPosition = new Vector3(minions[i].transform.localPosition.x, minions[i].transform.localPosition.y, -40);
                Vector2 v = minions_pos[i] - minions[i].transform.position;
                v *= Time.deltaTime * Vector2.Distance(minions_pos[i], minions[i].transform.position);

                if (Vector2.Distance(Vector3.zero, v) > Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * max_speed))
                    v = v.normalized * Time.deltaTime * max_speed;
                if (Vector2.Distance(Vector3.zero, v) < Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * min_speed))
                    v = v.normalized * Time.deltaTime * min_speed;
                if (Vector2.Distance(minions_pos[i], minions[i].transform.position) < Vector2.Distance(Vector3.zero, v))
                    minions[i].transform.position = new Vector3(minions_pos[i].x, minions_pos[i].y, transform.position.z);
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

                if (minions_pos[i].y > minions[i].transform.position.y)
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
    public void AddMinion(string mName, bool cardHandSpawn)
    {
        AddMinion(mousePos, mName, cardHandSpawn);
    }

    public void AddMinion(int n, string mName, bool cardHandSpawn)
    {
        switch (mName)
        {
            case "데스윙":
                AddMinion(n, mName, cardHandSpawn, SpawnAni.DeathWing);
                break;
            case "그룰":
                AddMinion(n, mName, cardHandSpawn, SpawnAni.Strong);
                break;
            default:
                AddMinion(n, mName, cardHandSpawn, SpawnAni.Normal);
                break;
        }
    }

    public void AddMinion(int n, string name, bool cardHandSpawn, SpawnAni spawnAni = SpawnAni.Normal)
    {
        //n : 소환위치
        //name : 소환하수인
        //cardHandSpawn : 손에서 소환했는지 여부
        //spawnAni : 소환애니메이션

        if (FullField())
            return;
        if (cardHandSpawn)
            GameEventManager.instance.EventAdd(2f);

        minionNum++;

        //가장 마지막 미니언 오브젝트를
        //n번째 미니언 오브젝트로 할당
        MinionObject temp = minions[minions.Length - 1];
        for (int i = 5; i >= n; i--)
            minions[i + 1] = minions[i];
        minions[n] = temp;
        minions[n].minion_name = name;
        minions[n].InitTrigger = true; //하수인의 이름을 참고해서 해당객체가 초기화됨

        //새로운 미니언 위치 지정
        float minX = minionDistance * (minionNum - 1) / 2f;
        Vector3 v = new Vector3(transform.position.x - minX + minionDistance * n,
            transform.position.y, minions[n].transform.position.z);
        minions[n].transform.position = v;

        //직업카드 퀘스트 처리
        DataMng dataMng = DataMng.instance;
        Vector2 pair = dataMng.GetPairByName(
            DataParse.GetCardName(name));
        if (pair.x == 0)
            QuestManager.instance.CharacterCard(Job.드루이드);
        else if (pair.x == 1)
            QuestManager.instance.CharacterCard(Job.도적);

        StartCoroutine(MinionDrop(n, spawnAni, cardHandSpawn));

    }

    private IEnumerator MinionDrop(int n, SpawnAni spawnAni, bool cardHandSpawn)
    {
        if (cardHandSpawn)
        {
            switch (spawnAni)
            {
                case SpawnAni.DeathWing:
                    {
                        EffectManager.instance.FireField();
                    }
                    break;
                default:
                    {
                        DragCardObject.instance.ShowDropEffectMinion(
                            Camera.main.WorldToScreenPoint(minions[n].transform.position), 0);
                    }
                    break;
            }
        }

        yield return new WaitWhile(() => !minions[n].animator.gameObject.activeSelf);

        if (cardHandSpawn)
        {
            yield return new WaitWhile(() => !DragCardObject.instance.dropEffect.effectArrive);

            SpawnEffect(n, spawnAni, true);

            yield return new WaitWhile(() => !minions[n].SpawnAniEnd());

            minions[n].CardHandMinionSpawn();
            if (GameEventManager.instance.GetEventValue() > 1.5f)
                GameEventManager.instance.EventSet(1.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            SpawnEffect(n, spawnAni, false);
        }
    }

    private void SpawnEffect(int n, SpawnAni spawnAni, bool sound)
    {
        switch (spawnAni)
        {
            case SpawnAni.Normal:
                {
                    if (sound)
                        SoundManager.instance.PlaySE("미니언소환일반");
                    minions[n].animator.SetTrigger("NormalSpawn");
                }
                break;
            case SpawnAni.Set:
                {
                    if (sound)
                        SoundManager.instance.PlaySE("미니언소환일반");
                    minions[n].animator.SetTrigger("SetSpawn");
                }
                break;
            case SpawnAni.DeathWing:
                {
                    minions[n].animator.SetTrigger("DeathWingSpawn");
                    EffectManager.instance.VibrationEffect(2f, 20, 10);
                }
                break;
            case SpawnAni.Strong:
                {
                    minions[n].animator.SetTrigger("StrongSpawn");
                    EffectManager.instance.VibrationEffect(0.5f, 15, 4);
                }
                break;
            default:
                break;
        }
    }

    #endregion

    #region[미니언 제거]

    public void RemoveMinion(int idx)
    {
        MinionObject removeMinion = minions[idx];
        for (int i = idx; i < minionNum - 1; i++)
            minions[i] = minions[i + 1];
        minions[minionNum - 1] = removeMinion;
        minionNum--;
        removeMinion.MinionRemoveProcess();
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

    #region[필드가 꽉찼는가?]
    public bool FullField()
    {
        return (minionNum >= MINION_SLOT_NUM);
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[OnDrawGizmos]
    private void OnDrawGizmos()
    {
        float minX = minionDistance * (minionNum - 1) / 2f;
        for (int i = 0; i < minionNum; i++)
        {
            Vector3 v = new Vector3(transform.position.x - minX + minionDistance * i, transform.position.y, transform.position.z);
            Gizmos.DrawSphere(v, 20);
        }
    }
    #endregion
}
