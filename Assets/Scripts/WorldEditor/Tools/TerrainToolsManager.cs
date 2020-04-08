using UnityEngine;

public class TerrainToolsManager : MonoBehaviour
{
    public GameObject heightTool;
    public GameObject textureTool;
    public GameObject rectangleTool;

    public GameObject globalTerrainActionListener;

    public void Start() 
    {
        GlobalTerrainActionListener listener = globalTerrainActionListener.GetComponent<GlobalTerrainActionListener>();

        listener.RegisterTerrainMousePressCallback(heightTool.GetComponent<IOnTerrainMouseAction>());
        listener.RegisterTerrainMousePressCallback(textureTool.GetComponent<IOnTerrainMouseAction>());
        listener.RegisterTerrainMousePressCallback(rectangleTool.GetComponent<IOnTerrainMouseAction>());
    }

    public void SetHeightToolActive() 
    {
        heightTool.GetComponent<TerrainHeightModifier>().Enable();
        textureTool.GetComponent<TerrainTextureModifier>().Disable();
        rectangleTool.GetComponent<RectangleDrawer>().Disable();
    }

    public void SetTextureToolActive()
    {
        textureTool.GetComponent<TerrainTextureModifier>().Enable();
        heightTool.GetComponent<TerrainHeightModifier>().Disable();
        rectangleTool.GetComponent<RectangleDrawer>().Disable();
    }

    public void SetRectangleToolActive()
    {
        rectangleTool.GetComponent<RectangleDrawer>().Enable();
        heightTool.GetComponent<TerrainHeightModifier>().Disable();
        textureTool.GetComponent<TerrainTextureModifier>().Disable();
    }
}
