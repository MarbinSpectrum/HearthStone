using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewManager : MonoBehaviour
{
    public static CardViewManager instance;

   [HideInInspector] public List<CardView> cardview = new List<CardView>();

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

    #region[UpdateCardView]
    public void UpdateCardView()
    {
        for (int i = 0; i < cardview.Count; i++)
            cardview[i].updateCard = true;
    }
    #endregion

}
