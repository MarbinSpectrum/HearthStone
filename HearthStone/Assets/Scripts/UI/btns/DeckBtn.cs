using UnityEngine;
using UnityEngine.UI;

public class DeckBtn : MonoBehaviour
{
    public GameObject newDeckBtn;
    public GameObject characterDeckBtn;
    public Text deckNameTxt;
    public Image[] characterImg;

    public bool hasDeck;
    public int nowCharacter;

    public void Update()
    {
        newDeckBtn.SetActive(!hasDeck);
        characterDeckBtn.SetActive(hasDeck);
        for (int i = 0; i < characterImg.Length; i++)
            characterImg[i].enabled = (i == nowCharacter);

    }
}
