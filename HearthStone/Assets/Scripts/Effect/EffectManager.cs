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

    public Transform playerDeckPos;
    public Transform enemyDeckPos;

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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        obj.transform.name = objName;
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
    }
    #endregion

    #region[회복 이펙트]
    public void HealEffect(Vector3 pos)
    {
        string objName = "HealEffect";
        GameObject obj = FindPool(objName);
        if (obj == null)
        {
            obj = Instantiate(healEffect);
            AddPool(obj);
        }
        obj.SetActive(true);
        obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        obj.transform.name = objName;
        AddPool(obj);
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
        AddPool(obj);
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
        AddPool(obj);
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
        for (int i = 0; i < n; i++)
        {
            Camera.main.transform.position = v + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(power, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        Camera.main.transform.position = v;
    }
    #endregion
}
