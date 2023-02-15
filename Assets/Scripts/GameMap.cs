using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMap : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int GridSize;

    [Header("Tile Settings")]
    public float OuterSize = 1f;
    public float InnerSize = 0f;
    public float Height = 1f;
    public bool FlatTop;
    public List<Material> Mat;

    private void OnEnable()
    {
        DrawGrid();
    }

    private void OnValidate()
    {
        Random.InitState(Random.Range(0, Int32.MaxValue));
        Clear();
        DrawGrid();
    }

    public void DrawGrid()
    {

        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                int RandomNumber = Random.Range(0, 5);
                GameObject Tile = new GameObject($"Hex {x},{y}", typeof(Hex));
                Tile.transform.position = GetPositionFromCoordinate(new Vector2Int(x, y));

                Hex HexDrawer = Tile.GetComponent<Hex>();
                HexDrawer.FlatTop = FlatTop;
                HexDrawer.Height = Height;
                HexDrawer.OuterSize = OuterSize;
                HexDrawer.InnerSize = InnerSize;
                HexDrawer.SetMesh(Mat[RandomNumber]);
                HexDrawer.DrawMesh();

                
                

                Tile.transform.SetParent(transform, true);

            }
        }
    }

    public Vector3 GetPositionFromCoordinate(Vector2Int Coordinate)
    {
        int Column = Coordinate.x;
        int Row = Coordinate.y;
        float Width;
        float Height;
        float XPosition;
        float YPosition;
        bool OffsetNeeded;
        float HorizontalDistance;
        float VerticalDistance;
        float Offset;
        float Size = OuterSize;

        if(!FlatTop)
        {
            OffsetNeeded = (Row % 2) == 0;
            Width = Mathf.Sqrt(3f) * Size;
            Height = 2f * Size;

            HorizontalDistance = Width;
            VerticalDistance = Height * (3f / 4f);

            Offset = OffsetNeeded ? Width / 2 : 0;

            XPosition = (Column * HorizontalDistance) + Offset;
            YPosition = (Row * VerticalDistance);
        }
        else
        {
            OffsetNeeded = (Column % 2) == 0;
            Width = 2f * Size;
            Height = Mathf.Sqrt(3f) * Size;

            HorizontalDistance = Width * (3f / 4f);
            VerticalDistance = Height;

            Offset = OffsetNeeded ? Height / 2 : 0;
            XPosition = Column * HorizontalDistance;
            YPosition = (Row * VerticalDistance) - Offset;
        }

        return new Vector3(XPosition, 0, -YPosition);
    }


    public void Clear()
    {
        for (var i = this.transform.childCount; i > 0; i-- )
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            };
        }
    }

}
