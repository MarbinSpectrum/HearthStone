using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEffect : MonoBehaviour
{
    [SerializeField]
    private float angleSpeed = 0.1f;
    float angle = 0;

    public Vector2 startPos;
    public Vector2 targetPos;
    private Vector2 lerpPos;

    protected float speed = 1;

    float lerpTime = 0;
    protected virtual void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
        angle += angleSpeed * Time.deltaTime;

        if(lerpTime <= 1)
        {
            lerpPos = Vector2.Lerp(startPos, targetPos, lerpTime);
            transform.position = lerpPos;
            lerpTime += Time.deltaTime * speed;
        }
        else
        {
            lerpTime = 0;
            EndEffect();
        }
    }

    protected virtual void EndEffect()
    {
        gameObject.SetActive(false);
    }
}
