using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandCheck : MonoBehaviour
{
    public static CardHandCheck instance;

    public CardView checkCard;

    public void Awake()
    {
        instance = this;
        checkCard.hide = true;
    }
}
