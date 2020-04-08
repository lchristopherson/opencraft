using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class TerrainLayerManager : MonoBehaviour
{
    private Dictionary<string, TerrainLayer> terrainLayerMap = new Dictionary<string, TerrainLayer>();

    public TerrainLayer[] layers;

    // void Start()
    // {
    //     TerrainLayer dirtLayer = Resources.Load<TerrainLayer>("TerrainLayers/DirtLayer");
    //     TerrainLayer grassLayer = Resources.Load<TerrainLayer>("TerrainLayers/GrassLayer");

    //     terrainLayerMap.Add(dirtLayer.name, dirtLayer);
    //     terrainLayerMap.Add(grassLayer.name, grassLayer);
    // }

    public void AddTerrainLayer(string name, TerrainLayer terrainLayer)
    {
        terrainLayerMap.Add(name, terrainLayer);

        layers = terrainLayerMap.Values.ToArray();
    }

    public TerrainLayer GetTerrainLayer(string name)
    {
        return terrainLayerMap[name];
    }

    public TerrainLayer[] GetTerrainLayers(string[] names)
    {
        return names.Select(name => GetTerrainLayer(name)).ToArray();
    }

    public List<string> GetNames()
    {
        return terrainLayerMap.Keys.ToList();
    }

    public void Reset()
    {
        // TODO: destroy existing TerrainLayers
        
        terrainLayerMap = new Dictionary<string, TerrainLayer>();
    }
}
