using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;

    public List<DamageNum> damageObj = new List<DamageNum>();

    #region[Awake]
    void Awake()
    {
        instance = this;
    }
    #endregion

    #region[CheckDamageObj]
    public bool CheckDamageObj(DamageNum obj)
    {
        for (int i = 0; i < damageObj.Count; i++)
            if (damageObj[i].Equals(obj))
                return true;
        return false;
    }
    #endregion

    #region[AddDamageObj]
    public void AddDamageObj(DamageNum obj,int n)
    {
        if (CheckDamageObj(obj))
            return;
        obj.damage = n;
        damageObj.Add(obj);
    }
    #endregion

    #region[PopAllDamageObj]
    public void PopAllDamageObj()
    {
        damageObj.Clear();
    }
    #endregion

    #region[PopDamageObj]
    public void PopDamageObj(DamageNum obj)
    {
        if (!CheckDamageObj(obj))
            return;
        for (int i = 0; i < damageObj.Count; i++)
        {
            if (damageObj[i].Equals(obj))
            {
                damageObj.RemoveAt(i);
                break;
            }
        }
    }
    #endregion

    #region[공격시 처리]
    public void AttackEffectRun()
    {
        for (int i = 0; i < damageObj.Count; i++)
        {
            damageObj[i].animator.SetTrigger("Damage");
            DamageRun(damageObj[i].hpSystem, damageObj[i].damage);
        }
        PopAllDamageObj();
    }

    public void DamageRun(string s,int n)
    {
        if(s.Contains("적_하수인"))
        {
            int i = s[6] - '0';
            EnemyMinionField.instance.minions[i].hp -= n;
            if (EnemyMinionField.instance.minions[i].hp <= 0)
                EnemyMinionField.instance.minions[i].MinionDeath();
        }
        else if (s.Contains("아군_하수인"))
        {
            int i = s[7] - '0';
            MinionField.instance.minions[i].hp -= n;
            if (MinionField.instance.minions[i].hp <= 0)
                MinionField.instance.minions[i].MinionDeath();
        }
        else if (s.Contains("아군_영웅"))
        {
            HeroManager.instance.heroHpManager.nowPlayerHp -= n;
        }
        else if (s.Contains("적_영웅"))
        {
            HeroManager.instance.heroHpManager.nowEnemyHp -= n;
        }
        StartCoroutine(CameraVibrationEffect(0, n, 0.5f));
    }
    #endregion

    #region[공격시 이펙트(진동)]
    private IEnumerator CameraVibrationEffect(float waitTime, int n, float power = 1)
    {
        yield return new WaitForSeconds(waitTime);
        Vector3 v = Camera.main.transform.position;
        for (int i = 0; i < n; i++)
        {
            Camera.main.transform.position = v + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(1, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        Camera.main.transform.position = v;
    }
    #endregion
}
