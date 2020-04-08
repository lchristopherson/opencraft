using UnityEngine;

public interface ITexture2DSerializer
{
    string Serialize(Texture2D texture, string path);
    Texture2D Deserialize(string path);
}