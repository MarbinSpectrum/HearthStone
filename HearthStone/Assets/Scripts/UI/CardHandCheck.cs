using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandCheck : MonoBehaviour
{
    public static CardHandCheck instance;


    public CardView checkCard;
    public Transform glow;
    public Vector2 size;
    public enum c { sv,sw,vs,vw,ws,wv,df}
    public c a;
    public void Awake()
    {
        instance = this;
        checkCard.hide = true;
    }

    public void Update()
    {
        glow.gameObject.SetActive(!checkCard.hide);
        Vector3 v =  Vector3.zero;
        switch (a)
        {
            case c.sv:
                v = Camera.main.ScreenToViewportPoint(transform.position);
                break;
            case c.sw:
                v = Camera.main.ScreenToWorldPoint(transform.position);
                break;
            case c.vs:
                v = Camera.main.ViewportToScreenPoint(transform.position);
                break;
            case c.vw:
                v = Camera.main.ViewportToWorldPoint(transform.position);
                break;
            case c.ws:
                v = Camera.main.WorldToScreenPoint(transform.position);
                break;
            case c.wv:
                v = Camera.main.WorldToViewportPoint(transform.position);
                break;
            default:
                v = transform.position;
                break;
        }

        glow.position = new Vector3(v.x * size.x, v.y * size.y, -100);
    }

}
