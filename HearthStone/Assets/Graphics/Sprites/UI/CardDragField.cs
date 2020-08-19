using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragField : MonoBehaviour
{
    public static bool InMouse;

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
