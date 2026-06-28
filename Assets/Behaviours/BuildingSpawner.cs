using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Grid — match your RoadNetwork settings")]
    public int gridWidth  = 5;
    public int gridHeight = 5;
    public float blockSize = 20f;
    public float roadWidth = 4f;

    [Header("Buildings")]
    public int buildingsPerBlock = 3;
    public float minHeight = 3f;
    public float maxHeight = 15f;
    public float margin   = 1.5f;   // gap between building and road edge

    [Header("Materials")]
    public Material[] buildingMaterials;

    void Start()
    {
        if (buildingMaterials == null || buildingMaterials.Length == 0)
            buildingMaterials = new[] { new Material(Shader.Find("Standard")) { color = new Color(0.6f, 0.6f, 0.65f) } };

        SpawnBuildings();
    }

    void SpawnBuildings()
    {
        float totalW = gridWidth  * blockSize;
        float totalH = gridHeight * blockSize;

        float usable = blockSize - roadWidth - margin * 2f;
        if (usable <= 0f) return;

        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                float blockCentreX = col * blockSize - totalW / 2f + blockSize / 2f;
                float blockCentreZ = row * blockSize - totalH / 2f + blockSize / 2f;

                for (int i = 0; i < buildingsPerBlock; i++)
                {
                    float w = Random.Range(usable * 0.25f, usable * 0.6f);
                    float d = Random.Range(usable * 0.25f, usable * 0.6f);
                    float h = Random.Range(minHeight, maxHeight);

                    float halfSpan = usable / 2f;
                    float x = blockCentreX + Random.Range(-halfSpan + w / 2f, halfSpan - w / 2f);
                    float z = blockCentreZ + Random.Range(-halfSpan + d / 2f, halfSpan - d / 2f);

                    CreateBuilding(new Vector3(x, h / 2f, z), new Vector3(w, h, d));
                }
            }
        }
    }

    void CreateBuilding(Vector3 position, Vector3 size)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Building";
        go.transform.SetParent(transform);
        go.transform.localPosition = position;
        go.transform.localScale    = size;

        var mat = buildingMaterials[Random.Range(0, buildingMaterials.Length)];
        go.GetComponent<Renderer>().material = mat;
        // BoxCollider is kept so buildings are solid
    }
}
