using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCamera : MonoBehaviour
{

    private RenderTexture texture;

    [Header("Cameras")]
    public Camera camera;
    public Material overlayMaterial;

    public void Awake()
    {
        texture = new RenderTexture(1920, 1080, 0);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture tempRenderTexture = RenderTexture.GetTemporary(source.width, source.height);

        Graphics.Blit(source, destination, overlayMaterial);


    }
}
