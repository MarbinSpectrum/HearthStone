using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionField : MonoBehaviour
{
    public static MinionField instance;

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
    [HideInInspector] public int mousePos;

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
            Vector3 v = new Vector3(transform.position.x - minX + minionDistance * i, transform.position.y, transform.position.z);
            v = Camera.main.WorldToScreenPoint(v);
            if(Input.mousePosition.x < v.x)
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
        //새로 설치할 미니언에 따른 위치 설정
        bool flag = (minionNum == 7);   //미니언을 설치할 위치가 없을때
        flag = flag || (DragCardObject.instance.dragCardView.cardType != CardType.하수인);   //하수인을 드래그하는게 아닐때
        flag = flag || (!DragCardObject.instance.mouseInField);   //필드로 뭔가를 드래그하는게 아닐때

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

        for (int i = 0; i < 7; i++)
        {
            if (minions_Attack_pos[i] != Vector3.zero)
            {
                if (Vector3.Distance(minions_Attack_pos[i], minions[i].transform.position) < attack_range)
                {
                    if (minions_attack_delay[i] < 0)
                    {
                        minions[i].canAttackNum--;
                        minions_Attack_pos[i] = Vector3.zero;
                    }
                    else
                    {
                        AttackManager.instance.AttackEffectRun();
                        minions_attack_delay[i] -= Time.deltaTime;
                    }
                }
                else
                    minions_attack_delay[i] = attack_delay;

                if (minions_Attack_pos[i] != Vector3.zero)
                    minions_pos[i] = new Vector3(minions_Attack_pos[i].x, minions_Attack_pos[i].y, transform.position.z);
            }
        }
    }
    #endregion

    #region[미니언 이동]
    void MoveMinion()
    {
        min_speed = Mathf.Min(min_speed, max_speed);
        min_attack_speed = Mathf.Min(min_attack_speed, max_attack_speed);
        for (int i = 0; i < minionNum; i++)
        {
            minions[i].gameObject.SetActive(true);
            Vector2 v = minions_pos[i] - minions[i].transform.position;
            v *= Time.deltaTime * Vector2.Distance(minions_pos[i], minions[i].transform.position);

            if(Mathf.Abs(transform.position.y - minions[i].transform.position.y) < 5)
            {
                if (Vector2.Distance(Vector3.zero, v) > Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * max_speed))
                    v = v.normalized * Time.deltaTime * max_speed;
                if (Vector2.Distance(Vector3.zero, v) < Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * min_speed))
                    v = v.normalized * Time.deltaTime * min_speed;
                if (Vector2.Distance(minions_pos[i], minions[i].transform.position) < Vector2.Distance(Vector3.zero, v))
                    minions[i].transform.position = new Vector3(minions_pos[i].x, minions_pos[i].y, minions[i].transform.position.z);
                else
                    minions[i].transform.position += new Vector3(v.x, v.y, 0);
            }
            else if ((minions_attack_delay[i] == attack_delay) || (minions_attack_delay[i] <= 0))
            {
                if (Vector2.Distance(Vector3.zero, v) > Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * max_attack_speed))
                    v = v.normalized * Time.deltaTime * max_attack_speed;
                if (Vector2.Distance(Vector3.zero, v) < Vector2.Distance(Vector3.zero, v.normalized * Time.deltaTime * min_attack_speed))
                    v = v.normalized * Time.deltaTime * min_attack_speed;
                if (Vector2.Distance(minions_pos[i], minions[i].transform.position) >= Vector2.Distance(Vector3.zero, v))
                    minions[i].transform.position += new Vector3(v.x, v.y, 0);
            }
        }
        for (int i = minionNum; i < minions.Length; i++)
            minions[i].gameObject.SetActive(false);

    }
    #endregion

    #region[미니언 추가]
    public void AddMinion(int n, string name)
    {
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
        DragCardObject.instance.ShowDropEffectMinion(Camera.main.WorldToScreenPoint(v),0);
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
