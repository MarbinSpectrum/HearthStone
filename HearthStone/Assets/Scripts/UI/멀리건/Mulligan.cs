using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mulligan : MonoBehaviour
{
    public CardView[] cardView;
    public Animator cardAnimator;
    public GameObject[] cardGlow;
    public GameObject mulliganUI;

    public Animator coinAnimator;
    public Material coinMat;

    public void SetGoing(float n)
    {
        StartCoroutine(ShowMulligan(n));
        int r = Random.Range(0, 100) > 50 ? 1 : 2;
        StartCoroutine(SetCoin(n + 1.5f, r));
        StartCoroutine(CardGlow(n + (r == 2 ? 4.5f : 4f), r));
    }

    #region[멀리건]
    private IEnumerator ShowMulligan(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cardAnimator.SetInteger("State", 1);
    }
    #endregion

    #region[코인]
    private IEnumerator SetCoin(float waitTime,int n)
    {
        yield return new WaitForSeconds(waitTime);
        cardAnimator.SetInteger("State", n);
        coinAnimator.SetInteger("State", n);
        coinMat.SetFloat("Power", 0);
        yield return new WaitForSeconds(1.5f);
        float power = 0;
        for (int i = 0; i < 200; i++)
        {
            coinMat.SetFloat("Power", power);
            power += 0.07f;
            yield return new WaitForSeconds(0.01f);

        }
    }
    #endregion

    #region[카드 글로우]
    private IEnumerator CardGlow(float waitTime, int n)
    {
        yield return new WaitForSeconds(waitTime);
        cardGlow[n - 1].SetActive(true);
        mulliganUI.SetActive(true);
    }
    #endregion
}
