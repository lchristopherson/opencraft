using UnityEngine;

public class RectangleDrawer : MonoBehaviour
{
    public bool enabled;

    private Vector3 start;
    private Vector3 end;

    private void OnMouseDown()
    {
        start = GetPoint();
    }

    private void OnMouseUp()
    {
        end = GetPoint();

        if (enabled)
        {
            DrawRect();
        }
    }

    private void OnMouseDrag()
    {
        // Debug.Log("MOUSE DRAG");
    }

    private void DrawRect()
    {
        Vector3[] positions = {
            start,
            GetPointOnTerrain(new Vector3(start.x, 0, end.z)),
            end,
            GetPointOnTerrain(new Vector3(end.x, 0, start.z))
        };

        Vector3 mid0 = GetPointOnTerrain(new Vector3(start.x, 0, end.z));
        Vector3 mid1 = GetPointOnTerrain(new Vector3(end.x, 0, start.z));

        Vector3[] adjustedPositions = {
            new Vector3(start.x + 0.2f, start.y + 0.2f, start.z + 0.2f),
            new Vector3(mid0.x + 0.2f, mid0.y + 0.2f, mid0.z + 0.2f),
            new Vector3(end.x + 0.2f, end.y + 0.2f, end.z + 0.2f),
            new Vector3(mid1.x + 0.2f, mid1.y + 0.2f, mid1.z + 0.2f),
        };

        GetComponent<LineRenderer>().positionCount = adjustedPositions.Length;
        GetComponent<LineRenderer>().SetPositions(adjustedPositions);
    }

    private Vector3 GetPointOnTerrain(Vector3 sample)
    {
        float y = GetComponent<Terrain>().SampleHeight(sample);

        return new Vector3(sample.x, y, sample.z);
    }

    private Vector3 GetPoint()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        GetComponent<TerrainCollider>().Raycast(ray, out hit, Mathf.Infinity);

        return hit.point;
    }
}
