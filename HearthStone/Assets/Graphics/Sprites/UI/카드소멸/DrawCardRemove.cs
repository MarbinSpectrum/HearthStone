using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardRemove : MonoBehaviour
{
    public static DrawCardRemove instance;

    public List<Animator> removeCardAni = new List<Animator>();
    public CardView []removeCardView;
    bool []removeCardRun;

    void Awake()
    {
        instance = this;
        removeCardRun = new bool[removeCardAni.Count];
    }

    public void Update()
    {
        for (int i = 0; i < removeCardAni.Count; i++)
            if (!removeCardRun[i] && removeCardAni[i].GetCurrentAnimatorStateInfo(0).IsName("ReMoveDrawHandHide"))
                removeCardRun[i] = true;
    }

    public void RemoveCard(string s, bool enemy)
    {
        for (int i = 0; i < removeCardRun.Length; i++)
        {
            if (removeCardRun[i])
            {
                removeCardRun[i] = false;
                CardViewManager.instance.CardShow(ref removeCardView[i], s);
                CardViewManager.instance.UpdateCardView(0.001f);
                removeCardAni[i].SetTrigger(enemy ? "ReMoveEnemy" : "ReMove");
                break;
            }
        }
    }
}
