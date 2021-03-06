﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTester : MonoBehaviour
{
    public Material terrainMaterial;
    public GameObject globals;
    
    WorldSerializer worldSerializer;

    void Start()
    {
        var texture2DSerializer = new BinaryTexture2DSerializer();
        
        var terrainLayerSerializer = new JsonTerrainLayerSerializer(
            texture2DSerializer
        );

        var terrainSerializer = new JsonTerrainSerializer(
            texture2DSerializer,
            globals.GetComponent<TerrainLayerManager>(),
            terrainMaterial
        );
        
        worldSerializer = new WorldSerializer(
            terrainLayerSerializer,
            terrainSerializer,
            "/Users/lchristopherson/tmp/opencraft"
        );
    }

    public void Test()
    {
        worldSerializer.Test(gameObject);
    }
}
