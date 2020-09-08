using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : ObjectPool
{
    public static EffectManager instance;

    [SerializeField]
    private GameObject dragonBreath;
    [SerializeField]
    private GameObject fireField;
    [SerializeField]
    private GameObject coinEffect;
    [SerializeField]
    private GameObject swordEffect;
    [SerializeField]
    private GameObject portionEffect;
    [SerializeField]
    private GameObject energyEffect;
    [SerializeField]
    private GameObject iceballEffect;
    [SerializeField]
    private GameObject fireballEffect;
    [SerializeField]
    private GameObject curseEffect;
    [SerializeField]
    private GameObject magicEffect;
    [SerializeField]
    private GameObject iceEffect;
    [SerializeField]
    private GameObject fireEffect;
    [SerializeField]
    private GameObject explodeEffect;
    [SerializeField]
    private GameObject healEffect;
    [SerializeField]
    private GameObject waterEffect;
    [SerializeField]
    private GameObject cutEffect;
    [SerializeField]
    private GameObject tornadoEffect;
    [SerializeField]
    private GameObject spiralEffect;
    [SerializeField]
    private GameObject swipeTargetEffect;
    [SerializeField]
    private GameObject swipeEffect;
    [SerializeField]
    private GameObject heroExplodeEffect;
    [SerializeField]
    private GameObject breakSmokeEffect;

    public Transform playerDeckPos;
    public Transform enemyDeckPos;
    public Transform playerEffectPos;
    public Transform enemyEffectPos;

    #region[Awake]
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    #region[드래곤 브레스(알렉스트라자)]
    public void DragonBreath(Vector3 pos,float angle)
    {
        string objName = "DragonBreath";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(dragonBreath);
            AddPool(obj);
        }
        VibrationEffect(0, 100, 2);
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.name = objName;
        obj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    #endregion

    #region[필드에 불꽃(데스윙)]
    public void FireField()
    {
        string objName = "FireField";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(fireField);
            AddPool(obj);
        }
        VibrationEffect(0, 30,4);
        obj.SetActive(true);
        obj.transform.position = Vector3.zero;
        obj.transform.name = objName;
    }
    #endregion

    #region[코인이펙트(가젯잔 경매인)]
    public void CoinEffect(Vector3 pos)
    {
        string objName = "CoinEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(coinEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.name = objName;
    }
    #endregion

    #region[검이펙트(검 제작의 대가)]
    public void SwordEffect(Vector2 pos, Vector2 targetPos)
    {
        string objName = "SwordEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(swordEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.GetComponent<ThrowEffect>().startPos = obj.transform.position;
        obj.GetComponent<ThrowEffect>().targetPos = targetPos;
        obj.GetComponent<ThrowEffect>().angleSpeed = 180;
        obj.transform.name = objName;
    }
    #endregion

    #region[검이펙트(칼발부채)]
    public void FanofKnives(Vector2 pos,float baseAngle)
    {
        StartCoroutine(FanofKnives(pos, baseAngle, 25));
    }

    private IEnumerator FanofKnives(Vector2 pos, float baseAngle, int n)
    {
        for (int i = 0; i < n; i++)
        {
            string objName = "SwordEffect";
            GameObject obj = FindPool(objName);
            if (obj == null)
            {
                obj = Instantiate(swordEffect);
                AddPool(obj);
            }
            obj.SetActive(true);
            float angle = Random.Range(-80 + baseAngle, +80 + baseAngle);

            obj.transform.position = new Vector3(pos.x, pos.y, 900);
            obj.transform.eulerAngles = new Vector3(0, 0, angle);
            obj.GetComponent<ThrowEffect>().startPos = obj.transform.position;
            obj.GetComponent<ThrowEffect>().targetPos = obj.transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(0, 3000, 0);
            obj.GetComponent<ThrowEffect>().angleSpeed = 0;
            obj.transform.name = objName;
            yield return new WaitForSeconds(0.01f);
        }
    }
    #endregion

    #region[포션이펙트(광기의 연금술사)]
    public void PortionEffect(Vector2 pos, Vector2 targetPos)
    {
        string objName = "PortionEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(portionEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.GetComponent<ThrowEffect>().startPos = obj.transform.position;
        obj.GetComponent<ThrowEffect>().targetPos = targetPos;
        obj.transform.name = objName;
    }
    #endregion

    #region[별똥별]
    public void StarFall(bool enemy)
    {
        StartCoroutine(StarFall(enemy, 25));
    }

    private IEnumerator StarFall(bool enemy, int n)
    {
        for (int i = 0; i < n; i++)
        {
            string objName = "EnergyEffect";
            GameObject obj = FindPool(objName);
            if (obj == null)
            {
                obj = Instantiate(energyEffect);
                AddPool(obj);
            }
            obj.SetActive(true);
            if(enemy)
            {
                float r = Random.Range(-300, 300);
                obj.GetComponent<ThrowEffect>().startPos = enemyEffectPos.position + new Vector3(r,1000,0);
                obj.GetComponent<ThrowEffect>().targetPos = enemyEffectPos.position + new Vector3(r,60,0);
            }
            else
            {
                float r = Random.Range(-300, 300);
                obj.GetComponent<ThrowEffect>().startPos = playerEffectPos.position + new Vector3(r, 1000, 0);
                obj.GetComponent<ThrowEffect>().targetPos = playerEffectPos.position + new Vector3(r, 60, 0);
            }
            obj.transform.name = objName;
            yield return new WaitForSeconds(0.01f);
        }
    }
    #endregion

    #region[에너지 이펙트]
    public void EnergyEffect(Vector2 pos, Vector2 targetPos)
    {
        string objName = "EnergyEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(energyEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.GetComponent<ThrowEffect>().startPos = obj.transform.position;
        obj.GetComponent<ThrowEffect>().targetPos = targetPos;
        obj.transform.name = objName;
    }
    #endregion

    #region[아이스볼 이펙트]
    public void IceBallEffect(Vector2 pos, Vector2 targetPos)
    {
        string objName = "IceBallEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(iceballEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.GetComponent<ThrowEffect>().startPos = obj.transform.position;
        obj.GetComponent<ThrowEffect>().targetPos = targetPos;
        obj.transform.name = objName;
    }
    #endregion

    #region[파이어볼 이펙트]
    public void FireBallEffect(Vector2 pos, Vector2 targetPos)
    {
        string objName = "FireBallEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(fireballEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.GetComponent<ThrowEffect>().startPos = obj.transform.position;
        obj.GetComponent<ThrowEffect>().targetPos = targetPos;
        obj.transform.name = objName;
    }
    #endregion

    #region[저주 이펙트]
    public void CurseEffect(Vector2 pos, Vector2 targetPos)
    {
        string objName = "CurseEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(curseEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.GetComponent<ThrowEffect>().startPos = obj.transform.position;
        obj.GetComponent<ThrowEffect>().targetPos = targetPos;
        obj.transform.name = objName;
    }
    #endregion

    #region[마법이펙트]
    public void MagicEffect(Vector3 pos)
    {
        string objName = "MagicEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(magicEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.name = objName;
    }
    #endregion

    #region[파이어이펙트]
    public void FireEffect(Vector3 pos)
    {
        string objName = "FireEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(fireEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.name = objName;
    }
    #endregion

    #region[얼음이펙트]
    public void IceEffect(Vector3 pos)
    {
        string objName = "IceEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(iceEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.name = objName;
    }
    #endregion

    #region[폭발 이펙트]
    public void ExplodeEffect(Vector3 pos)
    {
        string objName = "ExplodeEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(explodeEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.name = objName;
    }
    #endregion

    #region[회복 이펙트]
    public void HealEffect(Vector3 pos,int n)
    {
        string objName = "HealEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(healEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.GetComponent<DamageNum>().damage = n;
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.transform.name = objName;
    }
    #endregion

    #region[물 이펙트]
    public void WaterEffect(Vector3 pos)
    {
        string objName = "WaterEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(waterEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.name = objName;
    }
    #endregion

    #region[베는 이펙트]
    public void CutEffect(Vector3 pos, Vector2 scale)
    {
        string objName = "CutEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(cutEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(50 * scale.x, 50 * scale.y, 50);
        obj.transform.name = objName;
    }
    #endregion

    #region[토네이도 이펙트]
    public void TornadoEffect(Vector3 pos)
    {
        string objName = "TornadoEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(tornadoEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x,pos.y,900);
        obj.transform.name = objName;
    }
    #endregion

    #region[스파이럴 이펙트]
    public void SpiralEffect(Vector3 pos)
    {
        string objName = "SpiralEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(spiralEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, 900);
        obj.transform.name = objName;
    }
    #endregion

    #region[휘둘러치기(대상) 이펙트]
    public void SwipeTargetEffect(Vector3 pos)
    {
        string objName = "SwipeTargetEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(swipeTargetEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, 900);
        obj.transform.name = objName;
    }
    #endregion

    #region[휘둘러치기 이펙트]
    public void SwipeEffect(bool enemy)
    {
        string objName = "SwipeEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(swipeEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        if(!enemy)
            obj.transform.position = enemyEffectPos.position;
        else
            obj.transform.position = playerEffectPos.position;
        obj.transform.name = objName;
    }
    #endregion

    #region[영웅폭발 이펙트]
    public void HeroExplodeEffect(Vector3 pos)
    {
        StartCoroutine(HeroExplodeEffect(1.5f, pos));
    }
    public IEnumerator HeroExplodeEffect(float waitTime, Vector3 pos)
    {
        yield return new WaitForSeconds(waitTime);
        string objName = "HeroExplodeEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(heroExplodeEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, 1200);
        obj.transform.name = objName;
    }
    #endregion

    #region[영웅폭발 이펙트]
    public void BreakSmokeEffect(Vector3 pos)
    {
        StartCoroutine(BreakSmokeEffect(0, pos));
    }
    public IEnumerator BreakSmokeEffect(float waitTime, Vector3 pos)
    {
        yield return new WaitForSeconds(waitTime);
        string objName = "BreakSmokeEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(breakSmokeEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, 1200);
        obj.transform.name = objName;
    }
    #endregion

    #region[이펙트(진동)]
    public void VibrationEffect(float waitTime, int n, float power = 1)
    {
        StartCoroutine(CameraVibrationEffect(waitTime, n, power));
    }

    private IEnumerator CameraVibrationEffect(float waitTime, int n, float power = 1)
    {
        yield return new WaitForSeconds(waitTime);
        Vector3 v = new Vector3(0, 0, -1000);
        float down = power / (float)n;
        for (int i = 0; i < n; i++)
        {
            Camera.main.transform.position = v + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(power, 0, 0);
            power -= down;
            yield return new WaitForSeconds(0.01f);
        }
        Camera.main.transform.position = v;
    }
    #endregion
}
