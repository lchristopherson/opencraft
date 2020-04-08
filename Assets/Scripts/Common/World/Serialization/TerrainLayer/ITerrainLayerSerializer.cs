using UnityEngine;

public interface ITerrainLayerSerializer
{
    string Serialize(TerrainLayer terrainLayer, string path);
    TerrainLayer Deserialize(string path, string filename);
}
