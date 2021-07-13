using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PaddleController : MonoBehaviour
{
    public enum EMovementMode
    {
        Inertialess,
        Inertia
    }

    [SerializeField] EMovementMode Movement = EMovementMode.Inertialess;
    [SerializeField] float Speed = 1f;
    [SerializeField] float MaxMovement = 3.5f;
    [SerializeField] float SpeedSlewRate = 1f;
    [SerializeField] bool IsAIControlled = false;

    IPaddleAI PaddleAI = null;

    Vector3 OriginPoint;

    Rigidbody PaddleRB;

    public Vector3 SimulatedVelocity
    {
        get
        {
            // at the edge - no movement
            if (Mathf.Abs(transform.position.y) >= MaxMovement)
                return Vector3.zero;

            if (Movement == EMovementMode.Inertia)
                return transform.forward * Speed * Input_Move;

            if (Movement == EMovementMode.Inertia)
                return transform.forward * Speed * Input_Move_WithInertia;

            return Vector3.zero;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PaddleRB = GetComponent<Rigidbody>();
        PaddleAI = GetComponent<IPaddleAI>();

        OriginPoint = transform.position;
    }

    // Update is called once per frame
    float Input_Move_WithInertia;
    void Update()
    {
        if (IsAIControlled)
            Input_Move = Mathf.Clamp(PaddleAI.CalculatePaddleVelocity(), -1f, 1f);

        if (Mathf.Sign(Input_Move) != Mathf.Sign(Input_Move_WithInertia))
            Input_Move_WithInertia = 0f;
            
        Input_Move_WithInertia = Mathf.MoveTowards(Input_Move_WithInertia, Input_Move, SpeedSlewRate * Time.deltaTime);
    }

    float Input_Move;
    void OnMove(InputValue moveInput)
    {
        if (!IsAIControlled)
            Input_Move = moveInput.Get<Vector2>().y;
    }

    void FixedUpdate()
    {
        if (Movement == EMovementMode.Inertialess)
        {
            // calculate the clamped new position
            float newZPosition = transform.position.z + (Input_Move * Speed * Time.deltaTime);
            newZPosition = Mathf.Clamp(newZPosition, -MaxMovement, MaxMovement);

            // update the position
            PaddleRB.MovePosition(OriginPoint + new Vector3(0, 0, newZPosition));
        }
        else if (Movement == EMovementMode.Inertia)
        {
            // calculate the clamped new position
            float newZPosition = transform.position.z + (Input_Move_WithInertia * Speed * Time.deltaTime);
            newZPosition = Mathf.Clamp(newZPosition, -MaxMovement, MaxMovement);

            // update the position
            PaddleRB.MovePosition(OriginPoint + new Vector3(0, 0, newZPosition));
        }
    }

    public void SetIsAIControlled(bool newIsAIControlled)
    {
        IsAIControlled = newIsAIControlled;
    }
}
