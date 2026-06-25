using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Grid — match RoadNetwork")]
    public int   gridWidth  = 5;
    public int   gridHeight = 5;
    public float blockSize  = 20f;
    public float roadWidth  = 4f;

    [Header("Traffic")]
    public int carCount = 8;

    public Material[] carMaterials;

    void Start()
    {
        if (carMaterials == null || carMaterials.Length == 0)
        {
            carMaterials = new Material[]
            {
                MakeMat(Color.white),
                MakeMat(Color.grey),
                MakeMat(new Color(0.1f, 0.3f, 0.8f)),
                MakeMat(new Color(0.8f, 0.1f, 0.1f)),
                MakeMat(new Color(0.9f, 0.6f, 0.0f)),
            };
        }

        SpawnTraffic();
    }

    void SpawnTraffic()
    {
        float totalW = gridWidth  * blockSize;
        float totalH = gridHeight * blockSize;

        for (int i = 0; i < carCount; i++)
        {
            // pick a random road intersection to spawn at
            int col = Random.Range(0, gridWidth  + 1);
            int row = Random.Range(0, gridHeight + 1);
            float x = col * blockSize - totalW / 2f;
            float z = row * blockSize - totalH / 2f;

            Vector3 spawnPos = new Vector3(x, 1.5f, z);

            GameObject car = BuildCar(carMaterials[Random.Range(0, carMaterials.Length)]);
            car.name = $"TrafficCar_{i}";
            car.transform.position = spawnPos;
            car.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            var traffic = car.AddComponent<TrafficCar>();
            traffic.gridWidth  = gridWidth;
            traffic.gridHeight = gridHeight;
            traffic.blockSize  = blockSize;
            traffic.speed      = Random.Range(4f, 9f);
        }
    }

    GameObject BuildCar(Material mat)
    {
        var root = new GameObject();
        root.transform.SetParent(transform);

        var rb = root.AddComponent<Rigidbody>();
        rb.mass = 1f;

        // single collider on the root to keep the car above ground
        var col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.3f, 0f);
        col.size   = new Vector3(1.8f, 0.6f, 3.6f);

        // chassis
        AddBlock(root, Vector3.zero,                  new Vector3(1.8f, 0.28f, 3.6f), mat);
        // cabin
        AddBlock(root, new Vector3(0, 0.43f,  0.05f), new Vector3(1.3f, 0.58f, 1.7f), DarkenMat(mat));
        // hood
        AddBlock(root, new Vector3(0, 0.18f,  1.35f), new Vector3(1.6f, 0.14f, 0.9f), mat);
        // trunk
        AddBlock(root, new Vector3(0, 0.20f, -1.3f),  new Vector3(1.55f, 0.2f, 0.85f), mat);
        // front bumper
        AddBlock(root, new Vector3(0, 0.08f,  1.88f), new Vector3(1.82f, 0.28f, 0.16f), mat);
        // rear bumper
        AddBlock(root, new Vector3(0, 0.08f, -1.88f), new Vector3(1.82f, 0.28f, 0.16f), mat);

        // wheels (visual only)
        var darkMat = MakeMat(new Color(0.15f, 0.15f, 0.15f));
        AddWheel(root, new Vector3(-1f, 0f,  1f), darkMat);
        AddWheel(root, new Vector3( 1f, 0f,  1f), darkMat);
        AddWheel(root, new Vector3(-1f, 0f, -1f), darkMat);
        AddWheel(root, new Vector3( 1f, 0f, -1f), darkMat);

        return root;
    }

    void AddBlock(GameObject parent, Vector3 localPos, Vector3 size, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = localPos;
        go.transform.localScale    = size;
        go.GetComponent<Renderer>().material = mat;
        Destroy(go.GetComponent<BoxCollider>());
    }

    void AddWheel(GameObject parent, Vector3 localPos, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        go.transform.localScale    = new Vector3(0.35f, 0.2f, 0.35f);
        go.GetComponent<Renderer>().material = mat;
        Destroy(go.GetComponent<Collider>());
    }

    Material DarkenMat(Material src)
    {
        var m = new Material(src);
        m.color = src.color * 0.4f;
        return m;
    }

    Material MakeMat(Color c) =>
        new Material(Shader.Find("Standard")) { color = c };
}
