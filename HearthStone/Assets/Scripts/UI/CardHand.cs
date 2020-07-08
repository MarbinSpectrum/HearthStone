using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CardHand : MonoBehaviour
{
    public static CardHand instance;

    [Header("패 오브젝트")]
    public List<RectTransform> card = new List<RectTransform>();
    CardView[] handCardView;
    List<Vector3> cardStartPos = new List<Vector3>();
    List<Vector2> cardStartSize = new List<Vector2>();
    List<float> cardStartAngle = new List<float>();
    List<float> handLerp = new List<float>();

    [Header("패의 크기")]
    public Vector2 defaultSize;

    [Header("패 사이의 각도")]
    [Range(0,90)]
    public float angle;

    [Header("최대 패의 각도")]
    [Range(30, 180)]
    public float maxAngle;

    [Header("중심적까지의 거리")]
    public float range;

    [Header("현재패의수")]
    [Range(0, 10)]
    public int nowHandNum;

    private void Awake()
    {
        instance = this;
        if (card.Count <= 0)
            return;
        handCardView = new CardView[card.Count];
        for (int i = 0; i < card.Count; i++)
        {
            cardStartPos.Add(Vector4.zero);
            cardStartSize.Add(Vector2.zero);
            cardStartAngle.Add(0);
            handCardView[i] = card[i].transform.Find("Card").GetComponent<CardView>();
            handLerp.Add(1);
        }
    }

    public void Update()
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
            tempAngle -= i * addAngle;
            Vector3 destinationPos = Quaternion.Euler(0, 0, tempAngle) * Vector3.up;
            destinationPos = transform.position + (Vector3)destinationPos * range;

            if (!Application.isPlaying)
            {
                card[i].transform.position = destinationPos;
                card[i].transform.rotation = Quaternion.Euler(0, 0, tempAngle);
            }
            else 
            {
                Vector3 nowPos = Vector3.Lerp(cardStartPos[i], destinationPos, handLerp[i]);
                float nowAngle = Mathf.Lerp(cardStartAngle[i], tempAngle, handLerp[i]);
                Vector2 nowSize = Vector2.Lerp(cardStartSize[i], defaultSize, handLerp[i]);

                if (handLerp[i] < 1)
                    handLerp[i] += Time.deltaTime * 2;
                else
                {
                    cardStartPos[i] = Vector3.zero;
                    handLerp[i] = 1;
                }

                card[i].transform.position = nowPos;         
                card[i].transform.rotation = Quaternion.Euler(0, 0, nowAngle);
                card[i].localScale = new Vector3(nowSize.x, nowSize.y,0);
            }
        }
    }

    public void CardMove(int n, Vector3 pos, Vector2 size, float angle = 0)
    {
        handLerp[n] = 0;
        cardStartPos[n] = pos;
        cardStartSize[n] = size;
        cardStartAngle[n] = angle;
    }

    public void CardMove(string name, int n, Vector3 pos, Vector2 size, float angle = 0)
    {
        handLerp[n] = 0;
        cardStartPos[n] = pos;
        cardStartSize[n] = size;
        cardStartAngle[n] = angle;
        CardViewManager.instance.CardShow(ref handCardView[n], name);
        CardViewManager.instance.UpdateCardView();
    }

    public void CardMove(CardView cardView, int n, Vector3 pos, Vector2 size, float angle = 0)
    {
        handLerp[n] = 0;
        cardStartPos[n] = pos;
        cardStartSize[n] = size;
        cardStartAngle[n] = angle;
        CardViewManager.instance.CardShow(ref handCardView[n], cardView);
        CardViewManager.instance.UpdateCardView();
    }

    #region[OnDrawGizmosSelected]
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < nowHandNum; i++)
            Gizmos.DrawLine(transform.position, card[i].transform.position);

        Gizmos.color = Color.green;

        Vector3 v1 = Quaternion.Euler(0, 0, -maxAngle / 2f) * Vector3.up * range;
        Gizmos.DrawLine(transform.position, transform.position + v1);
        Vector3 v2 = Quaternion.Euler(0, 0, maxAngle / 2f) * Vector3.up * range;
        Gizmos.DrawLine(transform.position, transform.position + v2);
    }
    #endregion

}
