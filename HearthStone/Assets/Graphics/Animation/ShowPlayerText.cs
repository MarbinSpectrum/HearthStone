using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShowPlayerText : MonoBehaviour
{

    [Header("텍스트 출력")]
    public Text showText;
    public string text;
    public bool show = false;

    [Header("----------------------------------------")]
    [Space(250)]

    public Animator animator;
    public GameObject text_obj;

    public void Update()
    {
        if(show)
        {
            text_obj.SetActive(true);
            show = false;
        }

        showText.text = text;

        if (animator.gameObject.activeSelf && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerText") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
            text_obj.SetActive(false);
    }

}
