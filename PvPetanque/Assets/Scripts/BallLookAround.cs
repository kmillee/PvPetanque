using UnityEngine;

public class BallLookAround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    float rotationX = 0f;
    float rotationY = 0f;
    public float sensitivity = 1f;

    // Update is called once per frame
    void Update()
    {
        rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX += Input.GetAxis("Mouse Y") * -1 * sensitivity;
        if (rotationX > 10)
        {
            rotationX = 10;
        }
        else if (rotationX < -10)
        {
            rotationX = -10;
        }
        if (rotationY > 45)
        {
            rotationY = 45;
        }
        else if (rotationY < -45)
        {
            rotationY = -45;
        }
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }
}
