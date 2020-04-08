using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

public class JsonTerrainLayerSerializer : ITerrainLayerSerializer
{
    private ITexture2DSerializer texture2DSerializer;

    public JsonTerrainLayerSerializer(ITexture2DSerializer texture2DSerializer)
    {
        this.texture2DSerializer = texture2DSerializer;
    }
    
    class SerializableTerrainLayer
    {
        public string Name { get; set; }
        public float TileWidth { get; set; }
        public float TileHeight { get; set; }
        public string DiffuseTextureFileName { get; set; }
    }

    public string Serialize(TerrainLayer terrainLayer, string path)
    {
        var name = Guid.NewGuid().ToString();
        var diffuseTextureFileName = texture2DSerializer.Serialize(terrainLayer.diffuseTexture, path);
        
        var serializable = new SerializableTerrainLayer
        {
            Name = terrainLayer.name,
            TileWidth = terrainLayer.tileSize.x,
            TileHeight = terrainLayer.tileSize.y,
            DiffuseTextureFileName = diffuseTextureFileName,
        };

        var json = JsonConvert.SerializeObject(serializable);

        File.WriteAllBytes($"{path}/{name}.layer", Encoding.UTF8.GetBytes(json));

        return name;
    }

    public TerrainLayer Deserialize(string path, string filename)
    {
        var bytes = File.ReadAllBytes($"{path}/{filename}");
        var json = Encoding.UTF8.GetString(bytes);
        var serializable = JsonConvert.DeserializeObject<SerializableTerrainLayer>(json);

        var diffuseTexture = texture2DSerializer.Deserialize($"{path}/{serializable.DiffuseTextureFileName}");
        
        return new TerrainLayer
        {
            metallic = 0,
            smoothness = 0,
            name = serializable.Name,
            tileSize = new Vector2(serializable.TileWidth, serializable.TileHeight),
            diffuseTexture = diffuseTexture,
        };
    }
}
