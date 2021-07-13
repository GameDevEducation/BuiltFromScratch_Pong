using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleAI_Simple : MonoBehaviour, IPaddleAI
{
    [SerializeField] float SlewRate = 5f;
    [SerializeField] float MaxPositionDelta = 1f;
    [SerializeField] AnimationCurve BallSpeedModifier;
    [SerializeField] AnimationCurve VelocityOutput;
    [SerializeField] float ReactionDistance = 10f;

    float TargetVelocity = 0f;
    float CurrentVelocity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool IsBallOnOurSide()
    {
        if (GameManager.Instance.ActiveBall != null)
            return Mathf.Abs(GameManager.Instance.ActiveBall.transform.position.x - transform.position.x) <= ReactionDistance;

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        // no ball active?
        if (GameManager.Instance.ActiveBall == null)
        {
            CurrentVelocity = TargetVelocity = 0f;
            return;
        }

        if (IsBallOnOurSide())
            Update_MoveTowardsBall();
        else
            Update_MoveTowardsCentre();

        // slew the current velocity
        CurrentVelocity = Mathf.MoveTowards(CurrentVelocity, TargetVelocity, SlewRate * Time.deltaTime);
    }

    void Update_MoveTowardsCentre()
    {
        // calculate the position delta of the paddle relative to the centre
        float paddleDeltaZ = -transform.position.z;
        float normalisedPaddleDeltaZ = Mathf.Clamp01(Mathf.Abs(paddleDeltaZ / MaxPositionDelta));
        
        // calculate our target velocity
        TargetVelocity = VelocityOutput.Evaluate(normalisedPaddleDeltaZ) * Mathf.Sign(paddleDeltaZ);
    }

    void Update_MoveTowardsBall()
    {
        // calculate the position delta of the ball relative to us
        float ballDeltaZ = GameManager.Instance.ActiveBall.transform.position.z - transform.position.z;
        float normalisedBallDeltaZ = Mathf.Clamp01(Mathf.Abs(ballDeltaZ / MaxPositionDelta));
        
        // calculate our target velocity
        TargetVelocity = VelocityOutput.Evaluate(normalisedBallDeltaZ) * Mathf.Sign(ballDeltaZ);

        // modify based on ball speed
        float ballSpeedModifier = BallSpeedModifier.Evaluate(GameManager.Instance.ActiveBall.NormalisedVelocity);
        TargetVelocity *= (1f + ballSpeedModifier);
    }

    public float CalculatePaddleVelocity()
    {
        return CurrentVelocity;
    }
}
