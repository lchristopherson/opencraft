using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // public TerrainGenerator terrainGenerator;
    public Vector2Int size;

    private Dictionary<Vector2Int, Terrain> terrainMap = new Dictionary<Vector2Int, Terrain>();

    // void Start()
    // {
    //     CreateTerrain(new Vector2Int(0, 0));
    //     CreateTerrain(new Vector2Int(0, 1));
    //     CreateTerrain(new Vector2Int(1, 0));
    //     CreateTerrain(new Vector2Int(1, 1));
    // }

    // public Terrain CreateTerrain(Vector2Int position) 
    // {
    //     if (GetTerrain(position) != null) 
    //     {
    //         throw new SystemException("Terrain already exists at position: " + position);
    //     }

    //     Terrain terrain = terrainGenerator.GenerateTerrain(ToGlobalPosition(position)); 
       
    //     RegisterTerrain(terrain, position);

    //     return terrain;
    // }

    public void AddTerrain(Terrain terrain, Vector2Int position)
    {
        RegisterTerrain(terrain, position);
    }

    public Terrain GetTerrain(Vector2Int position) 
    {
        if (terrainMap.ContainsKey(position)) 
        {
            return terrainMap[position];
        }

        return null;
    }

    public List<Vector2Int> Keys()
    {
        return terrainMap.Keys.ToList();
    }

    public GameObject GetTerrainFromGlobal(Vector2Int globalPosition) 
    {
        throw new NotImplementedException();
    }

    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
     *                          Private Methods                            *
     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

    private void RegisterTerrain(Terrain terrain, Vector2Int position) 
    {
        terrainMap.Add(position, terrain);

        RegisterNeighbors(terrain);
    }

    private Vector2Int ToGlobalPosition(Vector2Int position) 
    {
        return new Vector2Int(position.x * size.x, position.y * size.y);
    }

    private Vector2Int FromGlobalPosition(Vector2Int position)
    {
        return new Vector2Int(position.x / size.x, position.y / size.y);
    }

    /*
     * Registers neighbors a Terrain at a given position.
     *
     * Registers neighbors for the Terrain at `position` as well as the neighbors of
     * `position`.
     */
    private void RegisterNeighbors(Terrain terrain) 
    {
        // Set neighbors of target terrain
        Terrain[] neighbors = GetNeighbors(terrain);
        terrain.SetNeighbors(neighbors[0], neighbors[1], neighbors[2], neighbors[3]);
        
        // Set neighbors of neighbor terrains
        foreach (Terrain neighbor in GetNeighbors(terrain))  
        {
            if (neighbor == null) continue;

            neighbors = GetNeighbors(neighbor);
            neighbor.SetNeighbors(neighbors[0], neighbors[1], neighbors[2], neighbors[3]);
        }
    }

    private Terrain[] GetNeighbors(Terrain terrain)
    {
        Vector3 globalPosition  = terrain.transform.position;
        Vector2Int position = FromGlobalPosition(new Vector2Int((int) globalPosition.x, (int) globalPosition.z));

        return GetNeighbors(position);
    }

    private Terrain[] GetNeighbors(Vector2Int position) 
    {
        return new Terrain[]{
            GetTerrain(new Vector2Int(position.x - 1, position.y)),
            GetTerrain(new Vector2Int(position.x, position.y + 1)),
            GetTerrain(new Vector2Int(position.x + 1, position.y)),
            GetTerrain(new Vector2Int(position.x, position.y - 1)),
        };
    }
}
