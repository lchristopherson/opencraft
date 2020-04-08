using UnityEngine;

public interface IOnTerrainMouseAction {
    void OnPress(Terrain terrain, IMousePressCollision collision);
    void OnDrag(Terrain terrain, IMousePressCollision collision);
    void OnRelease(Terrain terrain, IMousePressCollision collision);
}