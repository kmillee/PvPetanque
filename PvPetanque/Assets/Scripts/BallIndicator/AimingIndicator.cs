using System;
using UnityEngine;

public class AimingIndicator : MonoBehaviour
{

    [SerializeField] private GameObject aimingIndicatorGameObject;

    public void Display(bool display)
    {
        aimingIndicatorGameObject.SetActive(display);
    }

}
