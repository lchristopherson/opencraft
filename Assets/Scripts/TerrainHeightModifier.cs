using UnityEngine;

/**
 * TODO: Ensure coordinates are in bounds 
 * TODO: Configure brush sizes and weights
 */
public class TerrainHeightModifier : MonoBehaviour
{
    public Vector2Int modificationArea;
    public float heightDelta;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 point = GetPoint();

            UpdateTerrain(point);
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("Updating heightmap");

        GetTerrainData().SyncHeightmap();
    }

    private void UpdateTerrain(Vector3 point)
    {
        TerrainData terrainData = GetTerrainData();
        Vector2Int localPosition = GetLocalPosition(point);

        int x = localPosition.x;
        int z = localPosition.y;

        float[,] heights = terrainData.GetHeights(x, z, modificationArea.x, modificationArea.y);

        for (int i = 0; i < heights.GetLength(0); i++)
        {
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                heights[i, j] += heightDelta;
            }
        }

        terrainData.SetHeightsDelayLOD(x, z, heights);
    }

    private Vector3 GetPoint()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        GetComponent<TerrainCollider>().Raycast(ray, out hit, Mathf.Infinity);

        return hit.point;
    }

    private TerrainData GetTerrainData()
    {
        return GetComponent<Terrain>().terrainData;
    }

    private Vector2Int GetLocalPosition(Vector3 position)
    {
        TerrainData terrainData = GetTerrainData();

        Vector3 size = terrainData.size;
        Vector2 scaled = new Vector2(position.x / size.x, position.z / size.z);

        return new Vector2Int((int)(scaled.x * terrainData.heightmapResolution), (int)(scaled.y * terrainData.heightmapResolution));
    }
}
