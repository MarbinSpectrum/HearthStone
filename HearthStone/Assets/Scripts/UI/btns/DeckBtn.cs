using UnityEngine;
using UnityEngine.UI;

public class DeckBtn : MonoBehaviour
{
    public GameObject newDeckBtn;
    public GameObject characterDeckBtn;
    public Text deckNameTxt;
    public Image[] characterImg;
    Image thisImg;

    public bool hasDeck;
    public bool hide;
    public int nowCharacter;

    [HideInInspector] public RectTransform newDeckRect;
    [HideInInspector] public RectTransform characterDeckRect;

    public void Awake()
    {
        newDeckRect = newDeckBtn.GetComponent<RectTransform>();
        characterDeckRect = characterDeckBtn.GetComponent<RectTransform>();
        thisImg = transform.GetComponent<Image>();
    }

    public void Update()
    {
        ImageUpdate();

    }

    public void ImageUpdate()
    {
        thisImg.enabled = !hide;

        if (hide)
        {
            newDeckBtn.SetActive(false);
            characterDeckBtn.SetActive(false);
            for (int i = 0; i < characterImg.Length; i++)
                characterImg[i].enabled = false;
            return;
        }


        newDeckBtn.SetActive(!hasDeck);
        characterDeckBtn.SetActive(hasDeck);
        for (int i = 0; i < characterImg.Length; i++)
            characterImg[i].enabled = (i == nowCharacter);
    }
}
