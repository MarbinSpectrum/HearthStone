using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragField : MonoBehaviour
{
    public static bool InMouse;

    public Collider2D collider2D;

    public void Update()
    {
        collider2D.enabled = Input.GetMouseButton(0);
    }

    void OnMouseEnter()
    {
        InMouse = true;
    }

    void OnMouseDrag()
    {
        InMouse = true;
    }

    void OnMouseExit()
    {
        InMouse = false;
    }
}
