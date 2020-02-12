using UnityEngine;

public class MyTerrain : MonoBehaviour
{
    public Material materialTemplate;
    public TerrainLayer[] terrainLayers;

    readonly int HEIGHTMAP_RESOLUTION = 513;
    readonly int BASE_MAP_RESOLUTION = 1024;
    readonly Vector3 SIZE = new Vector3(2000, 50, 2000);

    // Start is called before the first frame update
    void Start()
    {
        TerrainData terrainData = new TerrainData
        {
            size = SIZE,
            heightmapResolution = HEIGHTMAP_RESOLUTION,
            baseMapResolution = BASE_MAP_RESOLUTION,
        };

        terrainData.SetDetailResolution(BASE_MAP_RESOLUTION, 512); // terrainData.detailResolutionPerPatch

        // SetHeights(terrainData);

        GetComponent<Terrain>().terrainData = terrainData;
        GetComponent<TerrainCollider>().terrainData = terrainData;

        GetComponent<Terrain>().materialTemplate = materialTemplate;

        // GetComponent<Terrain>()

        DoTerrainTextures();
    }

    private void DoTerrainTextures()
    {
        TerrainData terrainData = GetComponent<Terrain>().terrainData;
        terrainData.terrainLayers = terrainLayers;

        

        Debug.Log(terrainData.alphamapWidth);
        Debug.Log(terrainData.alphamapHeight);
        Debug.Log(terrainData.alphamapLayers);


        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int i = 0; i < HEIGHTMAP_RESOLUTION - 1; i++)
        {
            for (int j = 0; j < HEIGHTMAP_RESOLUTION - 1; j++)
            {
                splatmapData[i, j, 0] = 1.0f;
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private void SetHeights(TerrainData terrainData)
    {
        float[,] heights = terrainData.GetHeights(0, 0, HEIGHTMAP_RESOLUTION - 1, HEIGHTMAP_RESOLUTION - 1);

        float max = (float)(2 * HEIGHTMAP_RESOLUTION);

        for (int i = 0; i < HEIGHTMAP_RESOLUTION - 1; i++)
        {
            for (int j = 0; j < HEIGHTMAP_RESOLUTION - 1; j++)
            {
                heights[i, j] = Mathf.Sin(i + j);
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}
