using UnityEngine;
using System;

public class CollisionListener : MonoBehaviour
{
    public Action onHitTraffic;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.StartsWith("TrafficCar"))
            onHitTraffic?.Invoke();
    }
}
