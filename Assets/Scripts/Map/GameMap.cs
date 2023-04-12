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

    [Header("Entities")]
    public GameObject Archer;
    public GameObject AiWarrior;
    bool PlayerSpawned = false;

    private void Awake()
    {
        Clear();
        DrawGrid();
    }

    private void Start()
    {
        Random.InitState(Random.Range(0, Int32.MaxValue));
    }

    private void OnValidate()
    {
        //Random.InitState(Random.Range(0, Int32.MaxValue));
        //Clear();
        //DrawGrid();
    }
    [Header("Offsets")]
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


    public void DrawGrid()
    {
        PlayerSpawned = false;
        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                int RandomNumber = Random.Range(0, 6);
                GameObject Tile = new GameObject($"Hex {x},{y}", typeof(Hex));
                Tile.transform.position = GetPositionFromCoordinate(new Vector2Int(x, y));

                Hex Hex = Tile.GetComponent<Hex>();
                Hex.FlatTop = FlatTop;
                Hex.Height = Height;
                Hex.OuterSize = OuterSize;
                Hex.InnerSize = InnerSize;
                Hex.Coords = new Vector2Int(x, y);
                string MatName = Mat[RandomNumber].name;
                Hex.Effect = MatName;
                switch(MatName)
                {
                    case "Sand":
                        Hex.MovementCost = 2;
                        break;

                    case "Water":
                        Hex.MovementCost = 5;
                        break;

                    case "Mountain":
                        Hex.MovementCost = 3;
                        break;

                    default:
                        Hex.MovementCost = 1;
                        break;
                }
                Hex.SetMesh(Mat[RandomNumber]);
                Hex.DrawMesh();

                if(x == GridSize.x - 3 && y == 1 && !PlayerSpawned)
                {
                    var temp = Instantiate(Archer, GetPositionFromCoordinate(new Vector2Int(x, y)) + new Vector3(0, 2, -1), Quaternion.Euler(new Vector3(0, 180, 0)));
                    temp.transform.SetParent(transform, true);
                    GameManager.Main.SetPlayerArcher(temp);
                    GameManager.Main.PlayerArcher.Pos = new Vector2Int(x, y);
                    //UnityEngine.Debug.Log(GameManager.Main.PlayerArcher.Pos);
                    PlayerSpawned = true;
                }
                else if(x > 2 && x < GridSize.x - 2 && y < 2 && !PlayerSpawned)
                {
                    int spawn = Random.Range(0, 5);
                    if (spawn == 2)
                    {
                        var temp = Instantiate(Archer, GetPositionFromCoordinate(new Vector2Int(x, y)) + new Vector3(0, 2, -1), Quaternion.Euler(new Vector3(0, 180, 0)));
                        temp.transform.SetParent(transform, true);
                        GameManager.Main.SetPlayerArcher(temp);
                        GameManager.Main.PlayerArcher.Pos = new Vector2Int(x, y);
                        //UnityEngine.Debug.Log(GameManager.Main.PlayerArcher.Pos);
                        PlayerSpawned = true;
                    }
                }

                if(x == 5 && y == 5)
                {
                    var temp = Instantiate(AiWarrior, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                    temp.transform.SetParent(transform, true);
                    GameManager.Main.SetAIWarrior(temp);
                    GameManager.Main.AiWarrior.Pos = new Vector2Int(x, y);
                }

                Tile.transform.SetParent(transform, true);
                
                GameObject waypoint = new GameObject($"WayPoint {x},{y}", typeof(DetectClick), typeof(SphereCollider));
                waypoint.transform.position = GetPositionFromCoordinate(new Vector2Int(x, y));
                waypoint.transform.SetParent(Tile.transform, true);

            }
        }
        GameManager.Main.MakeIntOrder();
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


    public GameObject FindWaypoint(Vector2Int pos)
    {
        Hex[] hexes = this.transform.GetComponentsInChildren<Hex>();
        foreach (Hex h in hexes)
        {
            if (h.Coords != pos) continue;
            return h.transform.GetChild(0).gameObject;
        }
        UnityEngine.Debug.LogError("WayPoint not found");
        return null;
    }

    public int GetMovementCost(Vector2Int pos)
    {
        Hex[] hexes = this.transform.GetComponentsInChildren<Hex>();
        foreach (Hex h in hexes)
        {
            if (h.Coords != pos) continue;
            return h.MovementCost;
        }
        UnityEngine.Debug.LogError("Movement cost not found");
        return 0;
    }

    public Hex FindHex(Vector2Int pos)
    {
        Hex[] hexes = this.transform.GetComponentsInChildren<Hex>();
        foreach (Hex h in hexes)
        {
            if (h.Coords != pos) continue;
            return h;
        }
        UnityEngine.Debug.LogError("WayPoint not found");
        return null;
    }


}
