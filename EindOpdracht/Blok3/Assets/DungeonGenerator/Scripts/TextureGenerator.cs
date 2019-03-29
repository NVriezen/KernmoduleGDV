#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using System.IO;
[CreateAssetMenu(fileName = "TextureGenerator", menuName = "CreateTextureGenerator")]
public class TextureGenerator : ScriptableObject
{
    public int width, height, noiseScale;
    public Color[] colors;
    private Texture2D tex;

    public void GenerateTexture()
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Color col = colors[Random.Range(0, colors.Length)];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.Lerp(Color.black, col, Mathf.PerlinNoise((float)x/width * noiseScale, (float)y/ height * noiseScale)));
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        SaveTexture(tex, "Assets/Resources/Textures/GeneratedTexture.png");
    }

    void SaveTexture(Texture2D texture, string filePath)
    {
        byte[] bytes = texture.EncodeToPNG();
        FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        BinaryWriter writer = new BinaryWriter(stream);
        for (int i = 0; i < bytes.Length; i++)
        {
            writer.Write(bytes[i]);
        }
        writer.Close();
        stream.Close();
        DestroyImmediate(texture);
        AssetDatabase.Refresh();
        Texture2D newTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D));
        newTexture.wrapMode = TextureWrapMode.Clamp;
        newTexture.filterMode = FilterMode.Point;
        AssetDatabase.Refresh();
    }
}
#endif