using System.Collections;
using UnityEngine;

public abstract class ThrowManager : MonoBehaviour
{
    protected bool _ballThrowIsRunning = false;
    
    protected Vector3 _startingPosition;
    protected GameObject _currentBall;
    protected Rigidbody _currentBallRb;

    public IEnumerator BallThrowCoroutine(GameObject ball)
    {
        if (_ballThrowIsRunning)
        {
            Debug.Log("Launched a BallThrow coroutine but the previous one is not finished.");
            yield break;
        }
        _ballThrowIsRunning = true;
        
        _currentBall = ball;
        _startingPosition = ball.transform.position;
        if (_currentBall.TryGetComponent<Rigidbody>(out _currentBallRb))
        {
            _currentBallRb.useGravity = false;
        }
        else
        {
            Debug.Log("No RigidBody found in ball gameobject.");
            yield break;
        }
        
        SetUpBall();

        yield return BallThrowSequence();

        _ballThrowIsRunning = false;
    }

    protected abstract void SetUpBall();
    protected abstract IEnumerator BallThrowSequence();
}
