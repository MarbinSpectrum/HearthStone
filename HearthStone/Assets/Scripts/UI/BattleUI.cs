using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public ShowPlayerText playerText;
    public ShowPlayerText enermyText;

    public Mulligan mulligan;

    private void Start()
    {
        StartCoroutine(ShowEnermyText(4, "내가 대자연을 수호하겠다!"));
        StartCoroutine(ShowMulligan(4, 1));
        StartCoroutine(ShowPlayerText(6, "자연은 반드시 보호해야한다!"));

    }

    #region[등장대사]
    private IEnumerator ShowPlayerText(float waitTime,string s)
    {
        yield return new WaitForSeconds(waitTime);
        playerText.show = true;
        playerText.text = s;
    }

    private IEnumerator ShowEnermyText(float waitTime, string s)
    {
        yield return new WaitForSeconds(waitTime);
        enermyText.show = true;
        enermyText.text = s;
    }
    #endregion

    #region[멀리건]
    private IEnumerator ShowMulligan(float waitTime,int n)
    {
        yield return new WaitForSeconds(waitTime);
        mulligan.animator.SetInteger("State", n);
    }
    #endregion

}
