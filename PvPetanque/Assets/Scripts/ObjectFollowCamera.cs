using UnityEngine;

public class ObjectFollowCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x,0.8f,0.8f);
        pos.y = Mathf.Clamp(pos.y,0.2f,0.2f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
