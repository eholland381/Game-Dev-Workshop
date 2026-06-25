using System.Collections;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Grid — match your RoadNetwork settings")]
    public int gridWidth   = 5;
    public int gridHeight  = 5;
    public float blockSize = 20f;
    public float roadWidth = 4f;

    [Header("Total objects to attempt spawning")]
    public int bushCount = 150;
    public int treeCount = 60;
    public int rockCount = 80;

    [Header("Materials (auto-created if empty)")]
    public Material leafMaterial;
    public Material trunkMaterial;
    public Material rockMaterial;
    public Material bushMaterial;

    void Start()
    {
        leafMaterial  = leafMaterial  ?? MakeMat(new Color(0.1f, 0.55f, 0.05f));
        bushMaterial  = bushMaterial  ?? MakeMat(new Color(0.2f, 0.7f,  0.1f));
        trunkMaterial = trunkMaterial ?? MakeMat(new Color(0.4f, 0.25f, 0.1f));
        rockMaterial  = rockMaterial  ?? MakeMat(new Color(0.5f, 0.48f, 0.45f));

        // wait one frame so BuildingSpawner has finished placing buildings
        StartCoroutine(SpawnAfterBuildings());
    }

    IEnumerator SpawnAfterBuildings()
    {
        yield return null;

        for (int i = 0; i < bushCount; i++)
            TrySpawn(SpawnBush);

        for (int i = 0; i < treeCount; i++)
            TrySpawn(SpawnTree);

        for (int i = 0; i < rockCount; i++)
            TrySpawn(SpawnRock);
    }

    void TrySpawn(System.Action<Vector3> spawner)
    {
        float totalW = gridWidth  * blockSize;
        float totalH = gridHeight * blockSize;

        Vector3 candidate = new Vector3(
            Random.Range(-totalW / 2f, totalW / 2f),
            0f,
            Random.Range(-totalH / 2f, totalH / 2f));

        if (IsOnRoad(candidate)) return;

        // skip if a building collider already occupies this spot
        if (Physics.OverlapSphere(candidate + Vector3.up * 1f, 0.6f).Length > 0) return;

        spawner(candidate);
    }

    bool IsOnRoad(Vector3 pos)
    {
        float totalW = gridWidth  * blockSize;
        float totalH = gridHeight * blockSize;
        float half   = roadWidth / 2f;

        // check horizontal roads
        for (int row = 0; row <= gridHeight; row++)
        {
            float roadZ = row * blockSize - totalH / 2f;
            if (Mathf.Abs(pos.z - roadZ) < half) return true;
        }

        // check vertical roads
        for (int col = 0; col <= gridWidth; col++)
        {
            float roadX = col * blockSize - totalW / 2f;
            if (Mathf.Abs(pos.x - roadX) < half) return true;
        }

        return false;
    }

    void SpawnBush(Vector3 pos)
    {
        var root = new GameObject("Bush");
        root.transform.SetParent(transform);

        float size = Random.Range(0.6f, 1.3f);
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.SetParent(root.transform);
        sphere.transform.localPosition = new Vector3(0, size * 0.4f, 0);
        sphere.transform.localScale    = new Vector3(size, size * 0.7f, size);
        sphere.GetComponent<Renderer>().material = bushMaterial;
        Destroy(sphere.GetComponent<Collider>());

        root.transform.position = pos;
    }

    void SpawnTree(Vector3 pos)
    {
        var root = new GameObject("Tree");
        root.transform.SetParent(transform);

        float trunkH = Random.Range(1.5f, 3.5f);
        float crownR = Random.Range(1.0f, 2.2f);

        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(root.transform);
        trunk.transform.localPosition = new Vector3(0, trunkH / 2f, 0);
        trunk.transform.localScale    = new Vector3(0.25f, trunkH / 2f, 0.25f);
        trunk.GetComponent<Renderer>().material = trunkMaterial;
        Destroy(trunk.GetComponent<Collider>());

        var crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        crown.transform.SetParent(root.transform);
        crown.transform.localPosition = new Vector3(0, trunkH + crownR * 0.6f, 0);
        crown.transform.localScale    = new Vector3(crownR, crownR * 1.1f, crownR);
        crown.GetComponent<Renderer>().material = leafMaterial;
        Destroy(crown.GetComponent<Collider>());

        root.transform.position = pos;
    }

    void SpawnRock(Vector3 pos)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Rock";
        go.transform.SetParent(transform);
        go.transform.position = pos + Vector3.up * 0.15f;
        go.transform.rotation = Quaternion.Euler(
            Random.Range(-20f, 20f), Random.Range(0f, 360f), Random.Range(-20f, 20f));
        float s = Random.Range(0.3f, 0.8f);
        go.transform.localScale = new Vector3(
            s * Random.Range(0.8f, 1.4f),
            s * Random.Range(0.5f, 0.9f),
            s * Random.Range(0.8f, 1.4f));
        go.GetComponent<Renderer>().material = rockMaterial;
    }

    Material MakeMat(Color color) =>
        new Material(Shader.Find("Standard")) { color = color };
}
