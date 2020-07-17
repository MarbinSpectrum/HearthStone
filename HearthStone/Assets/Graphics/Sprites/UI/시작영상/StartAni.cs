using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAni : MonoBehaviour
{
    public Animator animator;
    public GameObject mainMenu;
    public Animator main_animator;
    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            mainMenu.SetActive(true);
            gameObject.SetActive(false);
            main_animator.enabled = true;
        }
    }
}
