using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Engine")]
    public float maxMotorForce = 20f;
    public float brakeForce    = 30f;
    public float maxSpeed      = 20f;

    [Header("Steering")]
    public float turnTorque    = 6f;

    float throttle;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0.5f;
    }

    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        throttle = 0f;
        if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed)   throttle =  1f;
        if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) throttle = -1f;

        float speed = Vector3.Dot(rb.linearVelocity, transform.forward);

        // only apply motor force below max speed
        if (Mathf.Abs(speed) < maxSpeed)
            rb.AddForce(transform.forward * throttle * maxMotorForce);

        // braking: apply opposing force when slowing or reversing
        if (throttle == 0f)
            rb.AddForce(-rb.linearVelocity * brakeForce * Time.fixedDeltaTime);

        // steering scaled by speed so slow turns are tighter
        float steer = 0f;
        if (keyboard.leftArrowKey.isPressed  || keyboard.aKey.isPressed) steer = -1f;
        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) steer =  1f;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(
            0f, steer * turnTorque * Mathf.Sign(speed) * Time.fixedDeltaTime, 0f));
    }
}
