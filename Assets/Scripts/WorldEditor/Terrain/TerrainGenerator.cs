using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainManager;

    public Material materialTemplate;
    public TerrainLayer[] terrainLayers;

    public int heightmapResolution;
    public int basemapResolution;
    public int alphamapResolution;
    public int detailResolution;
    public Vector3Int size;

    public float noiseScale;

    //
    //        Top    +z
    //
    // Left -x         Right +x
    //      
    //        Bottom -z
    //

    public Terrain GenerateTerrain(Vector2Int origin)
    {
        GameObject terrainGameObject = CreateTerrainGameObject();
        terrainGameObject.transform.position = new Vector3(origin.x, 0, origin.y);

        Terrain terrain = terrainGameObject.GetComponent<Terrain>();

        terrain.materialTemplate = materialTemplate;

        return terrain;
    }

    public GameObject CreateTerrainGameObject()
    {
        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = heightmapResolution,
            baseMapResolution = basemapResolution,
            alphamapResolution = alphamapResolution,
            terrainLayers = terrainLayers,
            size = size,
        };

        terrainData.SetDetailResolution(detailResolution, terrainData.detailResolutionPerPatch);

        SetTexture(terrainData);
        // SetHeights(terrainData);

        return Terrain.CreateTerrainGameObject(terrainData);
    }

    private void SetTexture(TerrainData terrainData)
    {
        float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        // for (int x = 0; x < terrainData.alphamapWidth; x++)
        // {
        //     for (int y = 0; y < terrainData.alphamapHeight; y++)
        //     {
        //         float a0 = alphamaps[x, y, 0];
        //         float a1 = alphamaps[x, y, 1];

        //         a0 += Random.value * noiseScale;
        //         a1 += Random.value * noiseScale;

        //         float total = a0 + a1;

        //         alphamaps[x, y, 0] = a0 / total;
        //         alphamaps[x, y, 1] = a1 / total;
        //     }
        // }

        terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    private void SetHeights(TerrainData terrainData)
    {
        float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution - 1, heightmapResolution - 1);

        for (int i = 0; i < heightmapResolution - 1; i++)
        {
            for (int j = 0; j < heightmapResolution - 1; j++)
            {
                heights[i, j] = Mathf.Sin(i + j) * 0.1f;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}