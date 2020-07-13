using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAni : MonoBehaviour
{
    public Animator animator;
    public GameObject battleMenu;
    public GameObject myCollectionMenu;
    public GameObject mainMenu;

    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
        {
            battleMenu.SetActive(true);
            myCollectionMenu.SetActive(true);
            mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
