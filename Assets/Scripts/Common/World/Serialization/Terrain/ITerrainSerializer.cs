using UnityEngine;

public interface ITerrainSerializer
{
    string Serialize(Terrain terrain, string path);
    GameObject Deserialize(string path, string filename);

    void AssertSerializationWorks(Terrain terrain, string path);
}
