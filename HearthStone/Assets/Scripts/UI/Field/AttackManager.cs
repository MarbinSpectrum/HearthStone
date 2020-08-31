﻿using System.Collections;
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
        if (s.Contains("적_하수인"))
        {
            int i = s[6] - '0';
            EnemyMinionField.instance.minions[i].final_hp -= n;
            MinionManager.instance.DamageMinionAbility(EnemyMinionField.instance.minions[i]);
            if (EnemyMinionField.instance.minions[i].final_hp <= 0)
                EnemyMinionField.instance.minions[i].MinionDeath();
        }
        else if (s.Contains("아군_하수인"))
        {
            int i = s[7] - '0';
            MinionField.instance.minions[i].final_hp -= n;
            MinionManager.instance.DamageMinionAbility(MinionField.instance.minions[i]);
            if (MinionField.instance.minions[i].final_hp <= 0)
                MinionField.instance.minions[i].MinionDeath();
        }
        else if (s.Contains("아군_영웅"))
        {
            if (HeroManager.instance.heroHpManager.playerShield > 0)
            {
                int temp = n;
                temp -= HeroManager.instance.heroHpManager.playerShield;
                HeroManager.instance.heroHpManager.playerShield -= n;
                temp = Mathf.Max(temp, 0);
                HeroManager.instance.heroHpManager.nowPlayerHp -= temp;
                if (HeroManager.instance.heroHpManager.playerShield <= 0)
                    HeroManager.instance.heroHpManager.playerShieldAni.SetBool("Break", true);
            }
            else
                HeroManager.instance.heroHpManager.nowPlayerHp -= n;
        }
        else if (s.Contains("적_영웅"))
        {
            if (HeroManager.instance.heroHpManager.enemyShield > 0)
            {
                int temp = n;
                temp -= HeroManager.instance.heroHpManager.enemyShield;
                HeroManager.instance.heroHpManager.enemyShield -= n;
                temp = Mathf.Max(temp, 0);
                HeroManager.instance.heroHpManager.nowEnemyHp -= temp;
                if (HeroManager.instance.heroHpManager.enemyShield <= 0)
                    HeroManager.instance.heroHpManager.enemyShieldAni.SetBool("Break", true);
            }
            else
                HeroManager.instance.heroHpManager.nowEnemyHp -= n;
        }
        StartCoroutine(CameraVibrationEffect(0, 10, Mathf.Min(15,n)));
    }
    #endregion

    #region[공격시 이펙트(진동)]
    private IEnumerator CameraVibrationEffect(float waitTime, int n, float power = 1)
    {
        yield return new WaitForSeconds(waitTime);
        //Vector3 v = Camera.main.transform.position;
        Vector3 v = new Vector3(0, 0, -1000);
        for (int i = 0; i < n; i++)
        {
            Camera.main.transform.position = v + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(power, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        Camera.main.transform.position = v;
    }
    #endregion
}
