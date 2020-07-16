using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVibration : MonoBehaviour
{
    public bool vibrationTrigger;

    [Header("강도")]
    public float power;

    [Header("주기")]
    public float cycle;
    float time = 0;

    [Header("진동축")]
    public bool x;
    public bool y;
    public bool z;

    [Space(20)]
    [Header("-------------------------------")]

    public Transform vibration_obj;
    
    bool flag;



    void Update()
    {
        cycle = Mathf.Max(0, cycle);
        power = Mathf.Max(0, power);
        if (!vibration_obj)
            return;

        if(vibrationTrigger)
        {
            flag = false;
            time += Time.deltaTime;
            if(time > cycle)
            {
                time = 0;
                vibration_obj.localPosition = Vector3.zero + Quaternion.Euler(x ? Random.Range(0, 360) : 0, y ? Random.Range(0, 360) : 0, z ? Random.Range(0, 360) : 0) * new Vector3(1, 0, 0) * power;
            }
        }
        else
        {
            if(!flag)
            {
                flag = true;
                vibration_obj.localPosition = Vector3.zero;
                time = 0;
            }
        }
    }
}
