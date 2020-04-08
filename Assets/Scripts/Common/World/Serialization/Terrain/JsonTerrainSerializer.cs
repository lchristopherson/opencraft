using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Text;

using UnityEditor;
using UnityEngine;


public class JsonTerrainSerializer : ITerrainSerializer
{
    private ITexture2DSerializer texture2DSerializer;
    private TerrainLayerManager terrainLayerManager;
    private Material terrainMaterial;

    public JsonTerrainSerializer(
        ITexture2DSerializer texture2DSerializer,
        TerrainLayerManager terrainLayerManager,
        Material terrainMaterial
    )
    {
        this.texture2DSerializer = texture2DSerializer;
        this.terrainLayerManager = terrainLayerManager;
        this.terrainMaterial = terrainMaterial;
    }

    class SerializableTerrain
    {
        public int HeightmapResolution { get; set; }
        public int BaseMapResolution { get; set; }
        public int AlphamapResolution { get; set; }
        public int DetailResolution { get; set; }
        // X
        public float Width { get; set; }
        // Z
        public float Length { get; set; }
        // Y
        public float Height { get; set; }
        public string[] LayerNames { get; set; }
        public string HeightmapName { get; set; }
        public string AlphamapsName { get; set; }
    }

    public string Serialize(Terrain terrain, string path)
    {
        var terrainData = terrain.terrainData;
        var terrainLayers = terrainData.terrainLayers;

        var layerNames = terrainLayers.ToList()
            .Select(layer => layer.name)
            .ToArray();

        var heightmapName = SerializeHeightmap(terrainData, path);
        var alphamapsName = SerializeAlphamaps(terrainData, path);

        var serializable = new SerializableTerrain
        {
            HeightmapResolution = terrainData.heightmapResolution,
            BaseMapResolution = terrainData.baseMapResolution,
            AlphamapResolution = terrainData.alphamapResolution,
            Width = terrainData.size.x,
            Height = terrainData.size.y,
            Length = terrainData.size.z,
            LayerNames = layerNames,
            HeightmapName = heightmapName,
            AlphamapsName = alphamapsName,
        };

        var name = $"{Guid.NewGuid().ToString()}.terrain";
        var json = JsonConvert.SerializeObject(serializable);

        File.WriteAllBytes($"{path}/{name}", Encoding.UTF8.GetBytes(json));

        return name;
    }

    public GameObject Deserialize(string path, string filename)
    {
        var bytes = File.ReadAllBytes($"{path}/{filename}");
        var json = Encoding.UTF8.GetString(bytes);
        var serializable = JsonConvert.DeserializeObject<SerializableTerrain>(json);

        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = serializable.HeightmapResolution,
            baseMapResolution = serializable.BaseMapResolution,
            alphamapResolution = serializable.AlphamapResolution,
            terrainLayers = terrainLayerManager.GetTerrainLayers(serializable.LayerNames),
            size = new Vector3(serializable.Width, serializable.Height, serializable.Length),
        };

        terrainData.SetDetailResolution(serializable.DetailResolution, terrainData.detailResolutionPerPatch);

        DeserializeHeightmap(terrainData, serializable, path);
        DeserializeAlphamaps(terrainData, serializable, path);
        
        GameObject terrainGameObject = Terrain.CreateTerrainGameObject(terrainData);
        
        Terrain terrain = terrainGameObject.GetComponent<Terrain>();
        terrain.materialTemplate = terrainMaterial;

        return terrainGameObject;
    }

    
    public void AssertSerializationWorks(Terrain terrain, string path) 
    {
        var terrainData = terrain.terrainData;
        var alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution);

        var alphamapsCopy = (float[,,])alphamaps.Clone();

        var name = Serialize(terrain, path);
        var terrainGO = Deserialize(path, name);

        var newTerrainData = terrainGO.GetComponent<Terrain>().terrainData;
        var newAlphamaps = newTerrainData.GetAlphamaps(0, 0, newTerrainData.alphamapResolution, newTerrainData.alphamapResolution);
        
        Debug.Log(alphamaps.GetLength(0) + ", " + alphamaps.GetLength(1) + ", " + alphamaps.GetLength(2));
        Debug.Log(newAlphamaps.GetLength(0) + ", " + newAlphamaps.GetLength(1) + ", " + newAlphamaps.GetLength(2));

        for (var k = 0; k < 2; k++) 
        {
            for (var i = 0; i < 512; i++) 
            {
                for (var j = 0; j < 512; j++) 
                {
                    if (alphamapsCopy[i, j, k] != newAlphamaps[i, j, k])
                    {
                        Debug.Log("Difference at: [" + i + ", " + j + ", " + k + "]");
                    }
                }
            }
        }
        
        // Debug.Log("Alphamaps equal: " + string.Equals(alphamaps.Cast<String>(), newAlphamaps.Cast<String>()));
    }

    private string SerializeHeightmap(TerrainData terrainData, string path)
    {
        var name = Guid.NewGuid().ToString();
        var heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution - 1, terrainData.heightmapResolution - 1);

        FileStream fs = new FileStream($"{path}/{name}", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, heights);

        return name;
    }

    private string SerializeAlphamaps(TerrainData terrainData, string path)
    {
        var name = Guid.NewGuid().ToString();
        var alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        var sum0 = 0.0f;
        var sum1 = 0.0f;
        for (var i = 0; i < 512; i++) 
        {
            for (var j = 0; j < 512; j++) 
            {
                sum0 += alphamaps[i, j, 0];
                sum1 += alphamaps[i, j, 1];
            }
        }
        Debug.Log("Sum0: " + sum0);
        Debug.Log("Sum1: " + sum1);

        FileStream fs = new FileStream($"{path}/{name}", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, alphamaps);

        return name;
    }

    private void DeserializeHeightmap(TerrainData terrainData, SerializableTerrain serializable, string path)
    {
        FileStream fs = new FileStream($"{path}/{serializable.HeightmapName}", FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        var heights = (float[,]) formatter.Deserialize(fs);

        terrainData.SetHeights(0, 0, heights);
    }

    private void DeserializeAlphamaps(TerrainData terrainData, SerializableTerrain serializable, string path)
    {        
        FileStream fs = new FileStream($"{path}/{serializable.AlphamapsName}", FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        var alphamaps = (float[,,]) formatter.Deserialize(fs);

        // for (var k = 0; k < 2; k++) 
        // {
        //     float sum = 0;
                
        //     for (var i = 0; i < 512; i++) 
        //     {
        //         for (var j = 0; j < 512; j++) 
        //         {
        //             // sum += alphamaps[i, j, k];
        //             if (k == 0) alphamaps[i, j, k] = 0;
        //             if (k == 1) alphamaps[i, j, k] = 1;
        //         }
        //     }
            
        //     // Debug.Log($"{k}: {sum}");
        // }

        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
}
