using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    private Texture2D gridImage;
    private float borderSize = 0.1f;
    private Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        gridImage = new Texture2D((int)collider.bounds.size.x, (int)collider.bounds.size.z);
    }

    private void Start()
    {
        List<Vector2> walkableTest = new List<Vector2>()
        {
            { new Vector2(-8.5f, 8) },
            { new Vector2(-8.5f, 7) },
            { new Vector2(-8.5f, 6) },
            { new Vector2(-8.5f, 5) },
            { new Vector2(-8.5f, 4) },
            { new Vector2(-8.5f, 3) },
            { new Vector2(-8.5f, 2) },
            { new Vector2(-8.5f, 1) },
            { new Vector2(-8.5f, 0) },
        };
        GenerateGrid(walkableTest);
        ActivateGrid();
    }

    void ActivateGrid()
    {
        MeshRenderer floorRenderer = GetComponent<MeshRenderer>();
        floorRenderer.material.mainTexture = gridImage;
        floorRenderer.material.mainTextureScale = new Vector2(collider.bounds.size.x, collider.bounds.size.z);
        floorRenderer.material.mainTextureOffset = new Vector2(.5f, .5f);
    }

    void GenerateGrid(List<Vector2> walkableTiles)
    {
        Color gridColor = Color.green;
        Color walkableColor = Color.cyan;
        Color borderColor = Color.black;
        Collider floorCollider = GetComponent<Collider>();
        for (int x = 0; x < gridImage.width; x++)
        {
            for (int y = 0; y < gridImage.height; y++)
            {
                if (walkableTiles.Contains(new Vector2(x, y)))
                {
                    gridImage.SetPixel(x, y, new Color(walkableColor.r, walkableColor.g, walkableColor.b, 255));
                }
                else if (x < borderSize || x > gridImage.width - borderSize || y < borderSize || y > gridImage.height - borderSize)
                {
                    gridImage.SetPixel(x, y, new Color(borderColor.r, borderColor.g, borderColor.b, 50));
                }
                else gridImage.SetPixel(x, y, new Color(gridColor.r, gridColor.g, gridColor.b, 50));
            }
            gridImage.Apply();
        }

    }
}
