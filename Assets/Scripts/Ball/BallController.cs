using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public enum ELaunchDirection
    {
        Random,
        TowardsPlayer1,
        TowardsPlayer2
    }

    public enum ELastTouchedBy
    {
        NoOne,
        Player1,
        Player2
    }

    [SerializeField] ELaunchDirection LaunchDirection = ELaunchDirection.Random;
    [SerializeField] float InitialSpeed = 30f;
    [SerializeField] float MinimumSpeed = 20f;
    [SerializeField] float MaximumSpeed = 50f;
    [SerializeField] float MaxLaunchAngle = 30f;
    [SerializeField] float PaddleVelocityWeight = 1f;
    [SerializeField] Rigidbody BallRB;

    [SerializeField] bool AutoLaunch = true;

    public ELastTouchedBy LastTouchedBy { get; private set; }= ELastTouchedBy.NoOne;

    public float NormalisedVelocity => Mathf.Clamp01(CurrentVelocity.magnitude / MaximumSpeed);

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 CurrentVelocity;
    void FixedUpdate()
    {
        // perform auto launch?
        if (AutoLaunch)
        {
            AutoLaunch = false;
            Launch();
        }

        // we're sleeping - do nothing
        if (BallRB.IsSleeping())
            return;

        // clamp the ball velocity if needed
        if (BallRB.velocity.magnitude < MinimumSpeed || BallRB.velocity.magnitude > MaximumSpeed)
            BallRB.velocity = BallRB.velocity.normalized * Mathf.Clamp(BallRB.velocity.magnitude, MinimumSpeed, MaximumSpeed);

        CurrentVelocity = BallRB.velocity;
    }

    public void Launch()
    {
        // pick the launch vector
        Vector3 launchVector = Vector3.zero;
        if (LaunchDirection == ELaunchDirection.Random)
            launchVector = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right;
        else if (LaunchDirection == ELaunchDirection.TowardsPlayer1)
            launchVector = Vector3.left;
        else
            launchVector = Vector3.right;

        // pick a launch angle
        launchVector = Quaternion.Euler(0f, Random.Range(-MaxLaunchAngle, MaxLaunchAngle), 0f) * launchVector;

        // launch the ball
        BallRB.velocity = launchVector * InitialSpeed;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player1Paddle"))
        {
            LastTouchedBy = ELastTouchedBy.Player1;

            OnHitPaddle(other);
        }
        else if (other.gameObject.CompareTag("Player2Paddle"))
        {
            LastTouchedBy = ELastTouchedBy.Player2;

            OnHitPaddle(other);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            OnHitWall(other);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1Goal"))
        {
            GameManager.Instance.OnPlayer1GoalHit();
        }
        else if (other.CompareTag("Player2Goal"))
        {
            GameManager.Instance.OnPlayer2GoalHit();
        }
    }

    void OnHitPaddle(Collision other)
    {
        AudioManager.Instance.OnBallHitPaddle();

        // reflect the velocity
        Vector3 reflectedVelocity = Vector3.Reflect(CurrentVelocity, other.GetContact(0).normal);        

        // blend with the paddle velocity
        Vector3 paddleVelocity = other.gameObject.GetComponent<PaddleController>().SimulatedVelocity;
        BallRB.velocity = reflectedVelocity + paddleVelocity * PaddleVelocityWeight;
    }

    void OnHitWall(Collision other)
    {
        AudioManager.Instance.OnBallHitWall();

        // reflect the velocity
        BallRB.velocity = Vector3.Reflect(CurrentVelocity, other.GetContact(0).normal);        
    }
}
