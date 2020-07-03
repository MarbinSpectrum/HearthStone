using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public ShowPlayerText playerText;
    public ShowPlayerText enermyText;

    private void Start()
    {
        StartCoroutine(ShowEnermyText(4, "내가 대자연을 수호하겠다!"));
        StartCoroutine(ShowPlayerText(6, "자연은 반드시 보호해야한다!"));
    }

    #region[대사보여주기]
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

}
