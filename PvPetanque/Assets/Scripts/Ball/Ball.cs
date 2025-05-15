using UnityEngine;

public class Ball : MonoBehaviour
{
    public Team team; // Team of the ball
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        team = gm.currentTeam; // Set the team based on the current team in GameManager
        gm.RegisterBall(this);        
    }

}
