using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFade : MonoBehaviour
{
    public static GlobalFade instance;

    public Animator animator;

    #region[Awake]
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    public void FadeAni(float fadeStart,float fadeEnd)
    {
        StartCoroutine(Fade(fadeStart, fadeEnd));
    }

    private IEnumerator Fade(float fadeStart, float fadeEnd)
    {
        yield return new WaitForSeconds(fadeStart);
        animator.SetBool("Fade", true);
        yield return new WaitForSeconds(fadeEnd);
        animator.SetBool("Fade", false);
    }    
}
