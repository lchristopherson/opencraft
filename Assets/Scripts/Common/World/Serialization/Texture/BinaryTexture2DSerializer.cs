using System;
using System.IO;

using UnityEngine;

public class BinaryTexture2DSerializer : ITexture2DSerializer
{
    // public string Serialize(Texture2D texture, string path)
    // {
    //     var name = Guid.NewGuid().ToString();
    //     var bytes = texture.GetRawTextureData();

    //     File.WriteAllBytes($"{path}/{name}", bytes);

    //     return name;
    // }

    public string Serialize(Texture2D texture, string path)
    {
        var decompressed = DeCompress(texture);
        
        var name = Guid.NewGuid().ToString();
        var bytes = decompressed.EncodeToPNG();

        File.WriteAllBytes($"{path}/{name}", bytes);

        return name;
    }

    // public Texture2D Deserialize(string path, Vector2Int size)
    // {
    //     var bytes = File.ReadAllBytes(path);

    //     var texture = new Texture2D(size.x, size.y);
    //     texture.LoadRawTextureData(bytes);
    //     texture.Apply();

    //     return texture;
    // }

    public Texture2D Deserialize(string path)
    {
        var bytes = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(2, 2);
        ImageConversion.LoadImage(tex, bytes);

        return tex;
    }

    public static Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
