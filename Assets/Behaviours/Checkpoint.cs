using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;
    public bool isCoin;

    Renderer[] renderers;
    float bobOffset;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        bobOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.time * 2f + bobOffset) * 0.003f;
        transform.Rotate(Vector3.up, 60f * Time.deltaTime);
    }

    public void SetColor(Color c)
    {
        foreach (var r in renderers)
            r.material.color = c;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.CompareTag("Player")) return; // tag check handled in manager

        var manager = FindFirstObjectByType<ChallengeManager>();
        if (manager != null)
            manager.OnCheckpointHit(this, other.attachedRigidbody.gameObject);
    }
}
