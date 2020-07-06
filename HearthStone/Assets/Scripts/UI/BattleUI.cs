using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public ShowPlayerText playerText;
    public ShowPlayerText enermyText;

    public Mulligan mulligan;

    public GameObject playerSetEffect;

    public GameObject cameraObject;

    private void Start()
    {
        StartCoroutine(PlayerSetEffect(4));
        StartCoroutine(CameraVibrationEffect(4, 12,0.5f));
        StartCoroutine(ShowEnermyText(4, "내가 대자연을 수호하겠다!"));
        mulligan.SetGoing(4.5f);
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

    #region[플레이어 배치 이펙트]
    private IEnumerator PlayerSetEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerSetEffect.SetActive(true);
    }
    #endregion

    #region[플레이어 배치 이펙트]
    private IEnumerator CameraVibrationEffect(float waitTime, int n, float power = 1)
    {
        yield return new WaitForSeconds(waitTime);
        Vector3 v = cameraObject.transform.position;
        for (int i = 0; i < n; i++)
        {
            cameraObject.transform.position = v + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(1, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        cameraObject.transform.position = v;
    }
    #endregion

}
