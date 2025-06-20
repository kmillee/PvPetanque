using UnityEngine;
using UnityEngine.Serialization;

public class ReleasingIndicator : MonoBehaviour
{

    [SerializeField] private GameObject releasingIndicatorGameObject;
    
    public void Display(bool display)
    {
        releasingIndicatorGameObject.SetActive(display);
    }

    public void setLocalLength(float length)
    {
        releasingIndicatorGameObject.transform.localPosition = new Vector3(0, length, 0);
    }

    public Vector3 getPosition()
    {
        return releasingIndicatorGameObject.transform.position;
    }
    
}
