using UnityEngine;

public class RectangleDrawer : MonoBehaviour, IOnTerrainMouseAction, IEnableable
{
    private Vector3 start;
    private Vector3 end;

    private bool enabled = false;

    public void Enable()
    {
        enabled = true;
    }

    public void Disable() 
    {
        // Clear lines
        GetComponent<LineRenderer>().positionCount = 0;
        
        enabled = false;
    }

    public void OnPress(Terrain terrain, IMousePressCollision collision) 
    {
        if (!enabled) return;

        start = collision.hit.point;
    }
    
    public void OnDrag(Terrain terrain, IMousePressCollision collision) {
        // 
    }
    
    public void OnRelease(Terrain terrain, IMousePressCollision collision) {
        if (!enabled) return;

        end = collision.hit.point;

        DrawRect(terrain);
    }

    private void DrawRect(Terrain terrain)
    {
        Vector3 mid0 = GetPointOnTerrain(terrain, new Vector3(start.x, 0, end.z));
        Vector3 mid1 = GetPointOnTerrain(terrain, new Vector3(end.x, 0, start.z));

        Vector3[] adjustedPositions = {
            new Vector3(start.x + 0.2f, start.y + 0.2f, start.z + 0.2f),
            new Vector3(mid0.x + 0.2f, mid0.y + 0.2f, mid0.z + 0.2f),
            new Vector3(end.x + 0.2f, end.y + 0.2f, end.z + 0.2f),
            new Vector3(mid1.x + 0.2f, mid1.y + 0.2f, mid1.z + 0.2f),
        };

        GetComponent<LineRenderer>().positionCount = adjustedPositions.Length;
        GetComponent<LineRenderer>().SetPositions(adjustedPositions);
    }

    private Vector3 GetPointOnTerrain(Terrain terrain, Vector3 sample)
    {
        float y = terrain.SampleHeight(sample);

        return new Vector3(sample.x, y, sample.z);
    }
}
