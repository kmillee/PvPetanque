using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableRadio : MonoBehaviour
{
    [SerializeField] private AudioSource radioAudioSource;
    private Rigidbody rb;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // unlock 3D audio source for spatial sound
        radioAudioSource.spatialBlend = 1f;

        isDragging = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        Vector3 mouseWorld = GetMouseWorldPosition();
        offset = transform.position - mouseWorld;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mouseWorld = GetMouseWorldPosition();
        Vector3 targetPos = mouseWorld + offset;

        rb.MovePosition(targetPos);
    }

    void OnMouseUp()
    {
        isDragging = false;
        rb.useGravity = true;

        // Optional: Apply throw force based on drag direction/speed
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        plane.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }
}
