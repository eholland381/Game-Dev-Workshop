using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -8f);
    public float positionSmoothTime = 0.3f;
    public float rotationSmoothTime = 0.15f;

    Vector3 positionVelocity;
    Quaternion currentRotation;

    void Start()
    {
        if (target == null)
        {
            var car = GameObject.Find("Car");
            if (car != null) target = car.transform;
        }

        if (target != null)
        {
            currentRotation = target.rotation;
            transform.position = target.position + currentRotation * offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        currentRotation = Quaternion.Slerp(currentRotation, target.rotation, rotationSmoothTime / Time.deltaTime * Time.deltaTime);

        Vector3 desiredPosition = target.position + currentRotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, positionSmoothTime);
        transform.LookAt(target.position + Vector3.up * 0.5f);
    }
}
