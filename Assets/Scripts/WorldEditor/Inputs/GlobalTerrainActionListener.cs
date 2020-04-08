using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTerrainActionListener : MonoBehaviour
{
    class TerrainMousePressCollision : IMousePressCollision {
        internal RaycastHit point;
        internal Vector2Int local;

        public TerrainMousePressCollision(RaycastHit point, Vector2Int local) {
            this.point = point;
            this.local = local;
        }

        public RaycastHit hit { get { return point; } }
        public Vector2Int localPosition { get { return local; } }
    }

    private List<IOnTerrainMouseAction> terrainMouseActionCallbacks = new List<IOnTerrainMouseAction>();

    public void RegisterTerrainMousePressCallback(IOnTerrainMouseAction callback) {
        terrainMouseActionCallbacks.Add(callback);
    }

    void Update() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

        if (!Physics.Raycast (ray, out hit)) return;

        Terrain terrain = hit.transform.gameObject.GetComponent<Terrain>();
        Vector2Int local = GetLocalPosition(hit.point, terrain);
        TerrainMousePressCollision tmpc = new TerrainMousePressCollision(hit, local);

        // Left mouse press
        if (Input.GetMouseButtonDown(0)) 
        {
            foreach (IOnTerrainMouseAction callback in terrainMouseActionCallbacks) {
                callback.OnPress(terrain, tmpc);
            }
        }

        // Left mouse hold
        if (Input.GetMouseButton(0)) 
        {
            foreach (IOnTerrainMouseAction callback in terrainMouseActionCallbacks) {
                callback.OnDrag(terrain, tmpc);
            }
        }

        // Left mouse release
        if (Input.GetMouseButtonUp(0)) 
        {
            foreach (IOnTerrainMouseAction callback in terrainMouseActionCallbacks) {
                callback.OnRelease(terrain, tmpc);
            }
        }
    }

    private Vector2Int GetLocalPosition(Vector3 position, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;

        Vector3 size = terrainData.size;
        Vector2 scaled = new Vector2((position.x - terrain.transform.position.x) / size.x, (position.z - terrain.transform.position.z) / size.z);

        return new Vector2Int((int)(scaled.x * terrainData.heightmapResolution), (int)(scaled.y * terrainData.heightmapResolution));
    }
}
