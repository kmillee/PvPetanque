using UnityEngine;

public class Ball : MonoBehaviour
{
    public Team team; // Team of the ball
    private bool isMoving = false;
    private bool isDisqualified = false;
    private float timer = 0f;
    [SerializeField] private float maxTimer = 30f; // Maximum time before the ball is disqualified

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        team = gm.currentTeam; // Set the team based on the current team in GameManager
        gm.RegisterBall(this);        
    }

    void Update() {
        if(isDisqualified) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("Rigidbody component not found on the Ball object.");
            return;
        }

        if(!isMoving && rb.linearVelocity.magnitude >= 0.0005f) {
            // Debug.Log($"Ball {gameObject.name} has started moving.");
            isMoving = true; 
            timer = 0f; // Reset timer when the ball starts moving
        }

        if(isMoving) {
            timer += Time.deltaTime;
            // Debug.Log($"Ball {gameObject.name} is moving. Timer: {timer:F2}s");
            if(rb.linearVelocity.magnitude < 0.0005f && timer > 0.1f) {
                isMoving = false;
                timer = 0f; // Reset timer when the ball stops moving
                Debug.Log($"Ball {gameObject.name} has stopped moving.");
            }

            if (timer >= maxTimer) {
                Disqualify();
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null) {
                    gm.OnBallDisqualified(this);
                } else {
                    Debug.LogWarning("GameManager not found. Cannot call OnBallDisqualified.");
                }
            }
        }   
    }

    public void StartThrownTimer() {
        isMoving = true;
        timer = 0f; 
    }

    private void Disqualify() {
        isDisqualified = true;
        Debug.Log($"Ball {gameObject.name} has been disqualified.");
    }
}
