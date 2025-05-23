using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallThrower : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Angle settings")]
    public int minAngle = 0;
    public int maxAngle = 45;
    public int angleStep = 1;

    private int currentAngle = 20;

    [Header("Drag settings")]
    public float maxDrag = 100f; // in screen pixels
    public float maxForce = 20f;

    private bool isDragging = false, thrown = false;
    private Vector2 dragStartPos;
    private Vector3 launchDirection;
    private float launchForce;

    private LineRenderer line; 

    void handleAngleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAngle = Mathf.Min(currentAngle + angleStep, maxAngle);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAngle = Mathf.Max(currentAngle - angleStep, minAngle);
    }

    void drawPreview()
    {
        line.enabled = true;
        Vector3 origin = transform.position;

        float elevationRad = Mathf.Deg2Rad * currentAngle;
        Vector3 dir3D = launchDirection * Mathf.Cos(elevationRad) + Vector3.up * Mathf.Sin(elevationRad);

        //acount for mass
        float mass = rb.mass;
        Vector3 impulse = dir3D * launchForce * maxForce;
        Vector3 velocity = impulse / mass; 

        float timeStep = 0.1f;
        float totalTime = (2 * velocity.y) / -Physics.gravity.y; // basic projectile time

        // draw the size according to the time of flight
        int points = Mathf.Clamp(Mathf.CeilToInt(totalTime / timeStep), 2, 100);
        line.positionCount = points;

        for (int i = 0; i < points; i++)
        {
            float t = i * timeStep;
            Vector3 pos = origin + velocity * t + Physics.gravity * t * t / 2f;
            line.SetPosition(i, pos);
        }

    }

    void clearPreview()
    {
        line.enabled = false;
    }

    void LaunchBall()
    {
        float elevationRad = Mathf.Deg2Rad * currentAngle;
        Vector3 dir3D = launchDirection * Mathf.Cos(elevationRad) + Vector3.up * Mathf.Sin(elevationRad);
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(dir3D * launchForce * maxForce, ForceMode.Impulse); // velocity = impulse / mass
        // rb.linearVelocity = (dir3D * launchForce * maxForce) / rb.mass; // velocity = impulse / mass
        thrown = true;

        // -- Spawn a new ball --
        var spawner = FindFirstObjectByType<BallSpawner>();
        if (spawner != null)
        {
            spawner.Invoke(nameof(spawner.spawnBall), 1f); // small delay
        }
        else
        {
            Debug.LogWarning("No BallSpawner found in the scene.");
        }
    }

    // ---- Mouse callbacks ----
    void OnMouseDown()
    {
        if (thrown) return;
        isDragging = true;
        dragStartPos = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        if(!isDragging || thrown) return;

        Vector2 currentPos = Input.mousePosition;
        Vector2 delta =  dragStartPos - currentPos;

        // Direction in XZ plane -> map screen space to world space
        Vector3 camRight = Camera.main.transform.right;
        Vector3 camForward = Vector3.Cross(camRight, Vector3.up).normalized;

        launchDirection = (delta.x * camRight + delta.y * camForward).normalized;

        launchForce = Mathf.Clamp(delta.magnitude/maxDrag, 0f, 1f) ;
        drawPreview();  
    }

    void OnMouseUp()
    {
        if (!isDragging || thrown) return;
        isDragging = false;
        clearPreview();
        LaunchBall();
    }

    
    void Awake()
    {
        line = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        line.positionCount = 10;
        line.enabled = false;
        print("BallThrower started");
    }

    // Update is called once per frame
    void Update()
    {
        if (thrown) return;
        handleAngleInput();
    }
}
