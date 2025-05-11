using UnityEngine;

public class MouseLookAround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    float rotationX = 0f;
    float rotationY = 0f;
    public float sensitivity = 5f;

    // Update is called once per frame
    void Update()
    {
        rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX += Input.GetAxis("Mouse Y") * -1 * sensitivity;
        if(rotationX>90)
        {
            rotationX = 90;
        } else if(rotationX<-90)
        {
            rotationX = -90;
        }
        if (rotationY > 90)
        {
            rotationY = 90;
        }
        else if (rotationY < -90)
        {
            rotationY = -90;
        }
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }
}
