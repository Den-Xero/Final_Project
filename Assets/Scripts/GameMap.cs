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

    public List<Vector2Int> PointTopNoOffsetNeighbours = new List<Vector2Int>() { 
        new Vector2Int(0,1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(1,0)};

    public List<Vector2Int> PointTopOffsetNeighbours = new List<Vector2Int>() {
        new Vector2Int(1,1),
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
        new Vector2Int(1,-1),
        new Vector2Int(1,0)};

    public List<Vector2Int> FlatTopNoOffsetNeighbours = new List<Vector2Int>() {
        new Vector2Int(0,1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(1,1)};

    public List<Vector2Int> FlatTopOffsetNeighbours = new List<Vector2Int>() {
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(1,-1),
        new Vector2Int(1,0)};



    void GetNeighbours(Hex hex)
    {
        List<Hex> Neighbours = new List<Hex>();

        Vector3Int[] NeighboursCoords = new Vector3Int[]
        {
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(0, -1, 1),
        };

        foreach(Vector3Int NeighboursCoord in NeighboursCoords)
        {
            Vector3 hexcoord = hex.transform.position;

            //check if hex exists.
        }

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

                Hex Hex = Tile.GetComponent<Hex>();
                Hex.FlatTop = FlatTop;
                Hex.Height = Height;
                Hex.OuterSize = OuterSize;
                Hex.InnerSize = InnerSize;
                Hex.Coords = new Vector2Int(x, y);
                Hex.SetMesh(Mat[RandomNumber]);
                Hex.DrawMesh();

                
                

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
            YPosition = Row * VerticalDistance;
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
