using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CalibratingIndicator : MonoBehaviour
{

    [SerializeField] private GameObject calibratingIndicatorGameObject;
    [SerializeField] private GameObject spring;
    [SerializeField] private GameObject tip;
    
    private float _widthx, _widthz;
    [SerializeField] private float canonLength;
    private float Cx, Cz;

    private void OnEnable()
    {
        _widthx = calibratingIndicatorGameObject.transform.localScale.x;
        _widthz = calibratingIndicatorGameObject.transform.localScale.z;

        Cx = _widthx * canonLength;
        Cz = _widthz * canonLength;

    }

    public void SetWidth(float widthx, float widthz)
    {
        _widthx = widthx;
        _widthz = widthz;
    }

    public void Display(bool display)
    {
        calibratingIndicatorGameObject.SetActive(display);
    }
    
    public void UpdateLength(float length)
    {
        if (length > 0)
        {
            float wx = Mathf.Min(_widthx, Cx / length);
            float wz = Mathf.Min(_widthz, Cz / length);

            calibratingIndicatorGameObject.transform.localScale = new Vector3(wx, length, wz);
        }
        else
        {
            calibratingIndicatorGameObject.transform.localScale = new Vector3(_widthx, length, _widthz);
        }

    }

    public GameObject GetTip()
    {
        return tip;
    }

    public GameObject GetSpring()
    {
        return spring;
    }

}
