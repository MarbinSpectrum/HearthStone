using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMinionLineRenderer : MonoBehaviour
{
    public static DragMinionLineRenderer instance;

    public LineRenderer lineRenderer;
    [HideInInspector] public Vector2 startPos;

    public Transform arrowEnd;

    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(1, v);
        arrowEnd.position = v;
        Vector2 v2 = v - startPos;
        float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        arrowEnd.rotation = Quaternion.Euler(0,0, angle);
        arrowEnd.gameObject.SetActive(lineRenderer.enabled);

        float sy = Vector2.Distance(v, startPos) / 50f;

        arrowEnd.localScale = new Vector3(Mathf.Max(Mathf.Min(12, 12 * sy), 6), 12, 1);

    }
}
