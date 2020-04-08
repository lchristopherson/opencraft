using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

public class WorldSerializer
{
    private ITerrainLayerSerializer terrainLayerSerializer;
    private ITerrainSerializer  terrainSerializer;
    private string path;

    public WorldSerializer(
        ITerrainLayerSerializer terrainLayerSerializer,
        ITerrainSerializer  terrainSerializer,
        string path
    )
    {
        this.terrainLayerSerializer = terrainLayerSerializer;
        this.terrainSerializer = terrainSerializer;
        this.path = path;
    }
    
    public void Serialize(GameObject world)
    {
        WorldManager worldManager = world.GetComponent<WorldManager>();

        SerializeTerrainLayers();
        SerializeTerrain(worldManager);
    }

    public void Test(GameObject world) 
    {
        WorldManager worldManager = world.GetComponent<WorldManager>();
        Terrain terrain = worldManager.GetTerrain(new Vector2Int());

        terrainSerializer.AssertSerializationWorks(terrain, $"{path}/test");
    }

    class SerializableTerrainWrapper 
    {
        public string TerrainFilename  { get; set; }
        public int X  { get; set; }
        public int Y { get; set; }
    }

    class SerializableWorldWrapper
    {
        public SerializableTerrainWrapper[] Terrains { get; set; }
    }

    private void SerializeTerrain(WorldManager worldManager)
    {
        Terrain terrain = worldManager.GetTerrain(new Vector2Int());

        Directory.CreateDirectory($"{path}/terrain");

        var terrains = worldManager.Keys()
            .Select(key => new SerializableTerrainWrapper {
                X = key.x,
                Y = key.y,
                TerrainFilename = terrainSerializer.Serialize(worldManager.GetTerrain(key), $"{path}/terrain"),
            }).ToArray();
            
        var serializable = new SerializableWorldWrapper { Terrains = terrains };
        var json = JsonConvert.SerializeObject(serializable);
        var name = Guid.NewGuid().ToString();

        File.WriteAllBytes($"{path}/{name}.world", Encoding.UTF8.GetBytes(json));
    }

    private void SerializeTerrainLayers()
    {
        GameObject globals = GameObject.Find("Globals");
        TerrainLayerManager terrainLayerManager = globals.GetComponent<TerrainLayerManager>();

        Directory.CreateDirectory($"{path}/layers");
        
        foreach(string name in terrainLayerManager.GetNames())
        {
            terrainLayerSerializer.Serialize(terrainLayerManager.GetTerrainLayer(name), $"{path}/layers");
        }
    }

    public void Deserialize(GameObject world)
    {
        WorldManager worldManager = world.GetComponent<WorldManager>();

        DeserializeTerrainLayers();
        DeserializeTerrain(worldManager);
    }

    private void DeserializeTerrainLayers()
    {
        GameObject globals = GameObject.Find("Globals");
        TerrainLayerManager terrainLayerManager = globals.GetComponent<TerrainLayerManager>();

        var layersPath = $"{path}/layers";

        terrainLayerManager.Reset();

        var terrainLayers = Directory.EnumerateFiles(layersPath, "*.layer")
            .Select(file => file.Split('/').Last())
            .Select(name => terrainLayerSerializer.Deserialize(layersPath, name))
            .ToList();

        foreach (TerrainLayer layer in terrainLayers)
        {
            terrainLayerManager.AddTerrainLayer(layer.name, layer);
        }
    }

    private void DeserializeTerrain(WorldManager worldManager)
    {
        var worldFiles = Directory.EnumerateFiles(path, "*.world")
            .Select(file => file.Split('/').Last())
            .ToList();

        if (worldFiles.Count != 1) 
        {
            throw new Exception("Missing .world file");
        }
        
        var bytes = File.ReadAllBytes($"{path}/{worldFiles[0]}");
        var json = Encoding.UTF8.GetString(bytes);
        var serializable = JsonConvert.DeserializeObject<SerializableWorldWrapper>(json);

        foreach (var terrainWrapper in serializable.Terrains)
        {
            GameObject terrainGameObject = terrainSerializer.Deserialize($"{path}/terrain", terrainWrapper.TerrainFilename);
            var position = new Vector2Int(terrainWrapper.X, terrainWrapper.Y);

            var terrain = terrainGameObject.GetComponent<Terrain>();

            worldManager.AddTerrain(terrain, position);

            terrainGameObject.transform.position = new Vector3(position.x * terrain.terrainData.size.x, 0, position.y * terrain.terrainData.size.z);
        }
    }
}
