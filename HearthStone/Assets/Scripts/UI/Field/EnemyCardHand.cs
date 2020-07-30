using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class EnemyCardHand : MonoBehaviour
{
    public static EnemyCardHand instance;

    [Header("패 오브젝트")]
    public List<RectTransform> card = new List<RectTransform>();
    List<Image> cardImg = new List<Image>();
    List<Vector3> cardStartPos = new List<Vector3>();
    List<Vector2> cardStartSize = new List<Vector2>();
    List<float> cardStartAngle = new List<float>();
    List<float> handLerp = new List<float>();

    [Header("패의 크기")]
    public Vector2 defaultSize;

    [Header("패 사이의 각도")]
    [Range(0, 90)]
    public float angle;

    [Header("패의 중심 각도")]
    [Range(0, 360)]
    public float handRootCngle;

    [Header("최대 패의 각도")]
    [Range(30, 180)]
    public float maxAngle;

    [Header("중심적까지의 거리")]
    public float range;

    [Header("현재패의수")]
    [Range(0, 10)]
    public int nowHandNum;

    [Header("패 생성 위치")]
    public Transform drawCardPos;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < card.Count; i++)
        {
            cardImg.Add(card[i].GetComponent<Image>());
            cardStartPos.Add(Vector4.zero);
            cardStartSize.Add(Vector2.zero);
            cardStartAngle.Add(0);
            handLerp.Add(1);
        }
    }

    void Update()
    {
        UpdateCardHand();
    }

    public void UpdateCardHand()
    {
        for (int i = 0; i < card.Count; i++)
            card[i].gameObject.SetActive(i < nowHandNum);

        for (int i = 0; i < nowHandNum; i++)
        {
            float fullAngle;
            float addAngle;
            if ((nowHandNum - 1) * angle > maxAngle)
            {
                fullAngle = maxAngle;
                if (nowHandNum <= 1)
                    addAngle = 0;
                else
                    addAngle = maxAngle / (nowHandNum - 1);
            }
            else
            {
                fullAngle = (nowHandNum - 1) * angle;
                addAngle = angle;
            }
            float tempAngle = fullAngle / 2f;
            tempAngle -= (nowHandNum - 1 - i) * addAngle;
            Vector3 destinationPos = Quaternion.Euler(0, 0, tempAngle + handRootCngle) * Vector3.up;
            destinationPos = transform.position + (Vector3)destinationPos * range;

            if (!Application.isPlaying)
            {
                card[i].transform.position = destinationPos;
                card[i].transform.rotation = Quaternion.Euler(0, 0, tempAngle + handRootCngle);
            }
            else
            {
                Vector3 nowPos = Vector3.Lerp(cardStartPos[i], destinationPos, handLerp[i]);
                float nowAngle = Mathf.Lerp(cardStartAngle[i], tempAngle + handRootCngle, handLerp[i]);
                Vector2 nowSize = Vector2.Lerp(cardStartSize[i], defaultSize, handLerp[i]);

                if (handLerp[i] < 1)
                    handLerp[i] += Time.deltaTime * 2;
                else
                {
                    cardStartPos[i] = destinationPos;
                    cardStartAngle[i] = tempAngle;
                    cardStartSize[i] = defaultSize;
                    handLerp[i] = 1;
                }

                card[i].transform.position = nowPos;
                card[i].transform.rotation = Quaternion.Euler(0, 0, nowAngle);
                card[i].localScale = new Vector3(nowSize.x, nowSize.y, 0);
            }
        }
    }

    public void DrawCard()
    {
        if (nowHandNum >= 10)
            return;
        nowHandNum++;
        CardMove(nowHandNum - 1, drawCardPos.transform.position, defaultSize, 0);
        for (int i = 0; i < BattleUI.instance.playerCardAni.Length; i++)
        {
            if (BattleUI.instance.enemyCardAni[i].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
            {
                BattleUI.instance.enemyCardAni[i].SetTrigger("Draw");
                for (int j = 0; j < nowHandNum - 1; j++)
                {
                    CardMove(j, card[j].transform.position,
                        defaultSize,
                        card[j].transform.eulerAngles.z);
                }
                CardMove(nowHandNum - 1, drawCardPos.transform.position, defaultSize, 0);
                break;
            }
        }
    }

    #region[카드없애기]
    public void CardRemove(int n)
    {
        nowHandNum--;
        for (int i = 0; i < nowHandNum; i++)
        {
            float fullAngle;
            float addAngle;
            if ((nowHandNum - 1) * angle > maxAngle)
            {
                fullAngle = maxAngle;
                if (nowHandNum <= 1)
                    addAngle = 0;
                else
                    addAngle = maxAngle / (nowHandNum - 1);
            }
            else
            {
                fullAngle = (nowHandNum - 1) * angle;
                addAngle = angle;
            }
            float tempAngle = fullAngle / 2f;
            tempAngle -= i * addAngle;
            Vector3 destinationPos = Quaternion.Euler(0, 0, tempAngle) * Vector3.up;
            destinationPos = transform.position + (Vector3)destinationPos * range;
            int cardViewNum = (i >= n) ? i + 1 : i;
            CardMove(cardViewNum, card[cardViewNum].transform.position, defaultSize, tempAngle);
        }
        CardViewManager.instance.UpdateCardView(0.001f);
    }
    #endregion
    public void CardMove(int n, Vector3 pos, Vector2 size, float angle = 0)
    {
        handLerp[n] = 0;
        cardStartPos[n] = pos;
        cardStartSize[n] = size;
        cardStartAngle[n] = angle;
    }
}
