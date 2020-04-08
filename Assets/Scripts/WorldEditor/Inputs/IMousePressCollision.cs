using UnityEngine;

public interface IMousePressCollision {
    RaycastHit hit { get; }
    Vector2Int localPosition { get; }
}