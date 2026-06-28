using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    [Header("Driving")]
    public float speed        = 7f;
    public float turnSpeed    = 120f;
    public float waypointRadius = 3f;

    [Header("Grid — match RoadNetwork")]
    public int   gridWidth  = 5;
    public int   gridHeight = 5;
    public float blockSize  = 20f;

    Rigidbody rb;
    Vector3 target;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // move via MovePosition so nothing can stop it
        target = PickNextTarget(transform.position);
    }

    void FixedUpdate()
    {
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude < waypointRadius)
        {
            target = PickNextTarget(target);
            return;
        }

        // smoothly rotate toward target
        Quaternion desired = Quaternion.LookRotation(toTarget.normalized);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, desired, turnSpeed * Time.fixedDeltaTime));

        // move forward
        Vector3 move = transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    Vector3 PickNextTarget(Vector3 from)
    {
        float totalW = gridWidth  * blockSize;
        float totalH = gridHeight * blockSize;

        // snap to nearest intersection
        float cx = Mathf.Round((from.x + totalW / 2f) / blockSize) * blockSize - totalW / 2f;
        float cz = Mathf.Round((from.z + totalH / 2f) / blockSize) * blockSize - totalH / 2f;
        cx = Mathf.Clamp(cx, -totalW / 2f, totalW / 2f);
        cz = Mathf.Clamp(cz, -totalH / 2f, totalH / 2f);

        Vector3[] options =
        {
            new Vector3(cx + blockSize, from.y, cz),
            new Vector3(cx - blockSize, from.y, cz),
            new Vector3(cx, from.y, cz + blockSize),
            new Vector3(cx, from.y, cz - blockSize),
        };

        var valid = new System.Collections.Generic.List<Vector3>();
        foreach (var o in options)
        {
            if (o.x >= -totalW / 2f && o.x <= totalW / 2f &&
                o.z >= -totalH / 2f && o.z <= totalH / 2f)
                valid.Add(o);
        }

        return valid.Count > 0 ? valid[Random.Range(0, valid.Count)] : from;
    }
}
