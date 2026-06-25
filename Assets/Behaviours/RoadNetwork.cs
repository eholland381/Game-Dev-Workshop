using UnityEngine;

public class RoadNetwork : MonoBehaviour
{
    [Header("Layout")]
    public int gridWidth  = 5;   // number of blocks across
    public int gridHeight = 5;   // number of blocks deep
    public float blockSize = 20f; // distance between road centres
    public float roadWidth = 4f;

    [Header("Materials")]
    public Material roadMaterial;

    void Start()
    {
        if (roadMaterial == null)
            roadMaterial = new Material(Shader.Find("Standard")) { color = new Color(0.25f, 0.25f, 0.25f) };

        BuildGrid();
    }

    void BuildGrid()
    {
        float totalW = gridWidth  * blockSize;
        float totalH = gridHeight * blockSize;

        // horizontal road strips (run along X)
        for (int row = 0; row <= gridHeight; row++)
        {
            float z = row * blockSize - totalH / 2f;
            CreateSegment(
                new Vector3(0f, 0f, z),
                new Vector3(totalW + roadWidth, 0.05f, roadWidth),
                $"Road_H_{row}");
        }

        // vertical road strips (run along Z)
        for (int col = 0; col <= gridWidth; col++)
        {
            float x = col * blockSize - totalW / 2f;
            CreateSegment(
                new Vector3(x, 0f, 0f),
                new Vector3(roadWidth, 0.05f, totalH + roadWidth),
                $"Road_V_{col}");
        }
    }

    void CreateSegment(Vector3 position, Vector3 size, string segmentName)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = segmentName;
        go.transform.SetParent(transform);
        go.transform.localPosition = position;
        go.transform.localScale   = size;
        go.GetComponent<Renderer>().material = roadMaterial;
        Destroy(go.GetComponent<BoxCollider>());
    }
}
