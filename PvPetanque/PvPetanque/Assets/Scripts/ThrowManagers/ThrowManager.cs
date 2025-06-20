using System.Collections;
using UnityEngine;

public abstract class ThrowManager : MonoBehaviour
{
    protected bool BallThrowIsRunning = false;
    
    protected Vector3 StartingPosition;
    protected GameObject CurrentBall;
    protected Rigidbody CurrentBallRb;
    protected Ball CurrentBallScript;

    protected bool useGlobalCamera;

    public IEnumerator BallThrowCoroutine(Ball ballScript)
    {
        if (BallThrowIsRunning)
        {
            Debug.Log("Launched a BallThrow coroutine but the previous one is not finished.");
            yield break;
        }
        BallThrowIsRunning = true;

        CurrentBallScript = ballScript;
        CurrentBall = ballScript.gameObject;
        StartingPosition = ballScript.transform.position;
        if (CurrentBall.TryGetComponent<Rigidbody>(out CurrentBallRb))
        {
            CurrentBallRb.useGravity = false;
        }
        else
        {
            Debug.Log("No RigidBody found in ball gameobject.");
            yield break;
        }
        
        SetUpThrow();
        
        yield return BallThrowSequence();

        BallThrowIsRunning = false;
        
        CleanUpThrow();
        
    }

    protected abstract void SetUpThrow();
    protected abstract void CleanUpThrow();
    protected abstract IEnumerator BallThrowSequence();

    public void onGlobalCameraButtonClick()
    {
        useGlobalCamera = !useGlobalCamera;
    }
}
