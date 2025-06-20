using Unity.Mathematics;
using UnityEngine;

public class ball_controller_test : MonoBehaviour
{

    private Rigidbody _rb;
    [SerializeField] private float strength = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("."))
        {
            _rb.AddForce(Vector3.forward * strength);
        }

        if (Input.GetKey("e"))
        {
            _rb.AddForce(Vector3.back * strength);
        }

        if (Input.GetKey("u"))
        {
            _rb.AddForce(Vector3.right * strength);
        }

        if (Input.GetKey("o"))
        {
            _rb.AddForce(Vector3.left * strength);
        }

        // transform.Rotate(new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f), strength, Space.World);
        transform.rotation *= Quaternion.Euler(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f);
    }
}
