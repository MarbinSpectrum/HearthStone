using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CurlEf : MonoBehaviour
{
    public Transform _Parents;
    public Transform _Front;
    public Transform _Mask;
    public Transform _GradOutter;
    public Vector3 _Pos = new Vector3(-240.0f, -470.0f, 0.0f) * 0.01f;

    #region[LateUpdate]
    void LateUpdate()
    {
        transform.position = new Vector3(_Pos.x, _Pos.y, transform.position.z);
        transform.eulerAngles = Vector3.zero;
        float z;
        Vector3 pos = _Front.localPosition;
        float theta = Mathf.Atan2(pos.y, pos.x) * 180.0f / Mathf.PI + _Parents.eulerAngles.z;

        float deg = -(90.0f - theta) * 2.0f;
        _Front.eulerAngles = new Vector3(0.0f, 0.0f, deg);

        z = _Mask.position.z;
        _Mask.position = (transform.position + _Front.position) * 0.5f;
        _Mask.position = new Vector3(_Mask.position.x, _Mask.position.y, z);
        _Mask.eulerAngles = new Vector3(0.0f, 0.0f, deg * 0.5f);

        _GradOutter.position = _Mask.position;
        _GradOutter.eulerAngles = new Vector3(0.0f, 0.0f, deg * 0.5f + 90.0f);

        transform.position = new Vector3(_Pos.x, _Pos.y, transform.position.z);
        transform.eulerAngles = Vector3.zero;
    }
    #endregion
}

