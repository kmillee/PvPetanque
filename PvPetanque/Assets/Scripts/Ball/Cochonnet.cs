using UnityEngine;

public class Cochonnet : MonoBehaviour
{
    private void Start()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.SetCochonnet(this);
    }


    // private Rigidbody rb;

    // private void Awake()
    // {
    //     rb = GetComponent<Rigidbody>();
    // }

    // public bool HasStoppedMoving()
    // {
    //     return rb.velocity.magnitude < 0.05f;
    // }
}
