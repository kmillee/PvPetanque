using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CalibratingIndicator : MonoBehaviour
{

    [SerializeField] private GameObject calibratingIndicatorGameObject;
    private float _widthx, _widthz;

    private void OnEnable()
    {
        _widthx = transform.localScale.x;
        _widthz = transform.localScale.z;
    }

    public void Display(bool display)
    {
        calibratingIndicatorGameObject.SetActive(display);
    }
    
    public void UpdateLength(float length)
    {
        calibratingIndicatorGameObject.transform.localScale = new Vector3(_widthx, length, _widthz);
    }

}
