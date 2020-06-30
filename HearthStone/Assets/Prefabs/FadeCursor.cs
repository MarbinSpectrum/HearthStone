using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCursor : MonoBehaviour
{
    public float time = 0;

    public void Act(Quaternion quaternion)
    {
        transform.rotation = quaternion;
        time = 0.5f;
    }

    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
            gameObject.SetActive(false);
    }
}
