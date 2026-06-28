using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    public float wheelRadius = 0.4f;

    Transform[] wheels;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        wheels = new Transform[]
        {
            transform.Find("Wheel FL"),
            transform.Find("Wheel FR"),
            transform.Find("Wheel RL"),
            transform.Find("Wheel RR"),
        };
    }

    void Update()
    {
        float speed = Vector3.Dot(rb.linearVelocity, transform.forward);
        float degreesPerSecond = (speed / (2f * Mathf.PI * wheelRadius)) * 360f;

        foreach (var wheel in wheels)
        {
            if (wheel != null)
                wheel.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.Self);
        }
    }
}
