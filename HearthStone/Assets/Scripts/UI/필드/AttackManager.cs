using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;

    public List<DamageNum> damageObj = new List<DamageNum>();

    void Awake()
    {
        instance = this;
    }

    public bool CheckDamageObj(DamageNum obj)
    {
        for (int i = 0; i < damageObj.Count; i++)
            if (damageObj[i].Equals(obj))
                return true;
        return false;
    }

    public void AddDamageObj(DamageNum obj,int n)
    {
        if (CheckDamageObj(obj))
            return;
        obj.damage = n;
        damageObj.Add(obj);
    }

    public void PopAllDamageObj()
    {
        damageObj.Clear();
    }

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
        }
        else if (s.Contains("아군_하수인"))
        {
            int i = s[7] - '0';
            MinionField.instance.minions[i].hp -= n;
        }
        else if (s.Contains("아군_영웅"))
        {
            HeroManager.instance.heroHpManager.nowPlayerHp -= n;
        }
        else if (s.Contains("적_영웅"))
        {
            HeroManager.instance.heroHpManager.nowEnemyHp -= n;
        }
    }
}
