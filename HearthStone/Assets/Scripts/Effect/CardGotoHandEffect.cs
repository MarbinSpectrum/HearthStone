using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGotoHandEffect : MonoBehaviour
{
    public static CardGotoHandEffect instance;

    [SerializeField] private List<GoToHandEffect> gotoHandList 
        = new List<GoToHandEffect>();

    #region[Awake]
    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region[RunEffect]
    public static void RunEffect(Vector2 pos, string s, bool enemy)
    {
        for (int i = 0; i < instance.gotoHandList.Count; i++)
        {
            if (instance.gotoHandList[i].cardHide)
            {
                instance.gotoHandList[i].cardHide = false;
                instance.gotoHandList[i].transform.position = pos;
                CardViewManager.instance.CardShow(
                    ref instance.gotoHandList[i].dropEffectCardView, s);
                CardViewManager.instance.UpdateCardView(0.001f);
                if (enemy ||
                    CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("ÆÐÈ®´ë"))
                    instance.gotoHandList[i].dropEffectAni.SetTrigger("GoHand");
                else
                    instance.gotoHandList[i].dropEffectAni.SetTrigger("GoHand_Small");
                break;
            }
        }
    }
    #endregion
}
