using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragObject : MonoBehaviour
{
    public static CardDragObject instance;
    [HideInInspector] public bool isDrag;

    public CardView cardView;
    public CardDrag cardDrag;

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if(isDrag)
        {

        }
        else
        {

        }
    }



}
