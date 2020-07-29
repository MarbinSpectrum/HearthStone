using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CopyImg : MonoBehaviour
{
    public Image image_Copy;
    public SpriteRenderer spriteRenderer_Copy;

    public Image image_Paste;
    public SpriteRenderer spriteRenderer_Paste;

    void Update()
    {
        Sprite sprite = null;
        Color color = Color.white;
        if(image_Copy)
        {
            sprite = image_Copy.sprite;
            color = image_Copy.color;
        }
        else if(spriteRenderer_Copy)
        {
            sprite = spriteRenderer_Copy.sprite;
            color = spriteRenderer_Copy.color;
        }

        if(sprite != null)
        {
            if(image_Paste)
            {
                image_Paste.sprite = sprite;
                image_Paste.color = color;
            }
            if(spriteRenderer_Paste)
            {
                spriteRenderer_Paste.sprite = sprite;
                spriteRenderer_Paste.color = color;
            }
        }
    }
}
