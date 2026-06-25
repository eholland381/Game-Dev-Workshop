using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float moveForce = 10f;
    public float turnTorque = 5f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float forward = 0f;
        if (keyboard.upArrowKey.isPressed)   forward += 1f;
        if (keyboard.downArrowKey.isPressed) forward -= 1f;

        float turn = 0f;
        if (keyboard.rightArrowKey.isPressed) turn += 1f;
        if (keyboard.leftArrowKey.isPressed)  turn -= 1f;

        rb.AddForce(transform.forward * forward * moveForce);

        float speed = Vector3.Dot(rb.linearVelocity, transform.forward);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * turnTorque * Mathf.Sign(speed) * Time.fixedDeltaTime, 0f));
    }
}
