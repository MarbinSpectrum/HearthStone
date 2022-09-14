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

    int useCardNum = 0;

    [Header("패 생성 위치")]
    public Transform drawCardPos;

    int exhaustion = 0;

    [Header("상대방 패")]
    public List<string> nowCard = new List<string>();

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

            //Debug.Log(card[i].transform.eulerAngles.z);
            //if (card[i].transform.eulerAngles.z < 0)
            //    card[i].transform.rotation = Quaternion.Euler(0, 0, card[i].transform.eulerAngles.z + 360);
        }
    }

    public void CardDrawAct()
    {
        StartCoroutine(CardDrawActRun());
    }

    private IEnumerator CardDrawActRun()
    {
        int index = -1;

        for (int i = 0; i < BattleUI.instance.enemyCardAni.Length; i++)
        {
            if (BattleUI.instance.enemyCardAni[i].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            while (GameEventManager.instance.GetEventValue() > 0.1f)
                yield return new WaitForSeconds(0.001f);

            exhaustion++;
            BattleUI.instance.exhaustionUI.SetActive(true);
            BattleUI.instance.exhaustionText.text = "카드가 없습니다! " + exhaustion + "의\n 피해를 입습니다.";

            GameEventManager.instance.EventAdd(2f);
            yield return new WaitForSeconds(2f);
            BattleUI.instance.exhaustionUI.SetActive(false);

            AttackManager.instance.PopAllDamageObj();
            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, exhaustion);
            AttackManager.instance.AttackEffectRun();
            GameEventManager.instance.EventAdd(0.5f);
        }
        else if (nowHandNum >= 10)
        {
            BattleUI.instance.enemyCardAni[index].SetTrigger("Draw");
            string s = DruidAI.instance.AIDeck[0];
            DruidAI.instance.AIDeck.RemoveAt(0);
            DrawCardRemove.instance.RemoveCard(s, true);
            GameEventManager.instance.EventAdd(0.5f);
        }
        else
        {
            nowHandNum++;
            CardMove(nowHandNum - 1, drawCardPos.transform.position, defaultSize, 0);
            BattleUI.instance.enemyCardAni[index].SetTrigger("Draw");
            SoundManager.instance.PlaySE("카드드로우");
            string s = DruidAI.instance.AIDeck[0];
            DruidAI.instance.AIDeck.RemoveAt(0);
            nowCard.Add(s);
            for (int j = 0; j < nowHandNum - 1; j++)
                CardMove(j, card[j].transform.position, defaultSize, card[j].transform.eulerAngles.z);
            CardMove(nowHandNum - 1, drawCardPos.transform.position, defaultSize, 0);
            GameEventManager.instance.EventAdd(0.5f);
        }

        yield return new WaitForSeconds(0.001f);
    }

    public void AddCard(string s)
    {
        if (nowHandNum >= 10)
            return;
        nowHandNum++;
        CardMove(nowHandNum - 1, drawCardPos.transform.position, defaultSize, 0);
        nowCard.Add(s);
        for (int j = 0; j < nowHandNum - 1; j++)
            CardMove(j, card[j].transform.position, defaultSize, card[j].transform.eulerAngles.z);
        CardMove(nowHandNum - 1, drawCardPos.transform.position, defaultSize, 0);
    }

    #region[카드 사용]
    public void UseCard(int n)
    {
        int cost = 0;
        string cardName = nowCard[n];
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(cardName));
        string cardType = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드종류");
        int cardCost = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");

        cost = cardCost;
        cost = cost < 0 ? 0 : cost;
        if (cardType == "무기")
        {
            ManaManager.instance.enemyNowMana -= cost;
            DragCardObject.instance.dropEffect.dropEffectCardView.cardCostOffset = 0;
            useCardNum++;
            SpellManager.instance.RunSpell(cardName);
            CardRemove(n);
        }
        else if (cardType == "주문")
        {
            ManaManager.instance.enemyNowMana -= cost;
            useCardNum++;
            CardViewManager.instance.CardShow(ref DragCardObject.instance.dropEffect.dropEffectCardView, cardName);
            DragCardObject.instance.dropEffect.dropEffectCardView.cardCostOffset = 0;
            CardViewManager.instance.UpdateCardView(0.001f);
            DragCardObject.instance.ShowDropEffectSpell(Camera.main.WorldToScreenPoint(BattleUI.instance.enemySpellViewPos.transform.position), Camera.main.WorldToScreenPoint(BattleUI.instance.enemySpellViewPos.transform.position), 1);
            SpellManager.instance.RunSpell(cardName,true);
            CardRemove(n);
        }
        else if (cardType == "하수인")
        {
            ManaManager.instance.enemyNowMana -= cost;
            DragCardObject.instance.dropEffect.dropEffectCardView.cardCostOffset = 0;
            useCardNum++;

            if (cardName.Equals("데스윙"))
                EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, cardName, true, 2);
            else if (cardName.Equals("그룰"))
                EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.mousePos, cardName, true, 3);
            else
                EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, cardName, true);
            CardRemove(n);
        }
    }
    #endregion

    public void DrawCard()
    {
        CardDrawAct();
    }

    #region[카드없애기]
    public void CardRemove(int n)
    {
        nowCard.RemoveAt(n);
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
            tempAngle -= (nowHandNum - 1 - i) * addAngle;
            //Vector3 destinationPos = Quaternion.Euler(0, 0, tempAngle + handRootCngle) * Vector3.up;
            //destinationPos = transform.position + (Vector3)destinationPos * range;

            int cardViewNum = (i >= n) ? i + 1 : i;
            CardMove(i, card[cardViewNum].transform.position, defaultSize, tempAngle + handRootCngle);

            CardViewManager.instance.UpdateCardView(0.001f);
        }
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
