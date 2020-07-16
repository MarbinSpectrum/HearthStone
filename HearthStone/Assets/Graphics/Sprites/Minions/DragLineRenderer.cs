using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum 타겟 { 아군하수인,아군영웅,적하수인,적영웅};

public class DragLineRenderer : MonoBehaviour
{
    public static DragLineRenderer instance;

    public LineRenderer lineRenderer;

    public float value = 50f;
    public float MinValue = 6;
    public float MaxValue = 12;

    public float pointDis = 12;

    public GameObject arrowEnd;
    public GameObject arrowImg;
    public GameObject arrowTarget;

    [HideInInspector] public bool selectTarget;
    [HideInInspector] public Vector2 startPos;
    [HideInInspector] public int targetMask;
    [HideInInspector] public Vector2 dragTargetPos;

    private void Awake()
    {
        instance = this;
        InitMask();
        //AddMask(타겟.아군하수인);
        //AddMask(타겟.아군);
        //AddMask(타겟.적하수인);
        //AddMask(타겟.적);
    }

    public void Update()
    {
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dic = v - startPos;
        dic = dic.normalized;
        v += dic * pointDis;

        #region[선의 좌표 설정]
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(startPos.x, startPos.y, transform.position.z));
        lineRenderer.SetPosition(1, new Vector3(v.x, v.y,transform.position.z));
        arrowEnd.transform.position = new Vector3(v.x, v.y, arrowEnd.transform.position.z);
        Vector2 v2 = v - startPos;
        float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        arrowEnd.transform.rotation = Quaternion.Euler(0,0, angle);
        arrowEnd.SetActive(lineRenderer.enabled);
        #endregion

        #region[화살표 설정]
        float sy = Vector2.Distance(v, startPos) / Mathf.Max(1,value);
        arrowImg.transform.localScale = new Vector3(18, Mathf.Max(Mathf.Min(MaxValue, MaxValue * sy), MinValue), 1);
        #endregion

        arrowTarget.SetActive(selectTarget);
    }

    public void InitMask()
    {
        targetMask = 0;
    }

    public void SubMask(타겟 a)
    {
        if (!CheckMask(a))
            return;
        targetMask = (targetMask - (1 << (int)a));
    }

    public void AddMask(타겟 a)
    {
        targetMask = (targetMask | (1 << (int)a));
    }

    public bool CheckMask(타겟 a)
    {
        return ((targetMask & (1 << (int)a)) == (1 << (int)a));
    }
}
