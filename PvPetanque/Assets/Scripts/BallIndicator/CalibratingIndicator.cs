using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CalibratingIndicator : MonoBehaviour
{

    [SerializeField] private GameObject calibratingIndicatorGameObject;
    [SerializeField] private GameObject spring;
    [SerializeField] private GameObject tip;
    
    private float _widthx, _widthz;

    private void OnEnable()
    {
        _widthx = transform.localScale.x;
        _widthz = transform.localScale.z;
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
        calibratingIndicatorGameObject.transform.localScale = new Vector3(_widthx, length, _widthz);
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
