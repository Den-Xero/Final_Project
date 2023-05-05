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
    public GameObject Warrior;
    public GameObject Rogue;
    public GameObject Tank;
    public GameObject Mage;
    public GameObject AiWarrior;
    public GameObject AiArcher;
    public GameObject AiRogue;
    public GameObject AiTank;
    public GameObject AiMage;
    int PlayerSpawned = 0;
    int EnemiesSpawned = 0;

    private void Awake()
    {
        Clear();
        DrawGrid();
    }

    private void Start()
    {
        Random.InitState(Random.Range(0, Int32.MaxValue));
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
        //Draws the grid with all the player and AI units on a random range of spawn tiles.
        PlayerSpawned = 0;
        EnemiesSpawned = 0;
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

                if(x == GridSize.x - 2 && y == 1 && PlayerSpawned < 4)
                {
                    PlayerTypeToSpawn(x, y);
                }
                else if (x == GridSize.x - 3 && y == 1 && PlayerSpawned < 3)
                {
                    PlayerTypeToSpawn(x, y);
                }
                else if (x == GridSize.x - 4 && y == 1 && PlayerSpawned < 2)
                {
                    PlayerTypeToSpawn(x, y);
                }
                else if (x == GridSize.x - 5 && y == 1 && PlayerSpawned < 1)
                {
                    PlayerTypeToSpawn(x, y);
                }
                else if(x > 2 && x < GridSize.x - 2 && y < 2 && PlayerSpawned < 4)
                {
                    int spawn = Random.Range(0, 5);
                    if (spawn == 2)
                    {
                        PlayerTypeToSpawn(x, y);
                    }
                }

                if(x == GridSize.x - 2 && y == GridSize.y - 1 && EnemiesSpawned < 4)
                {
                    AITypeToSpawn(x, y);
                }
                else if (x == GridSize.x - 3 && y == GridSize.y - 1 && EnemiesSpawned < 3)
                {
                    AITypeToSpawn(x, y);
                }
                else if (x == GridSize.x - 4 && y == GridSize.y - 1 && EnemiesSpawned < 2)
                {
                    AITypeToSpawn(x, y);
                }
                else if (x == GridSize.x - 5 && y == GridSize.y - 1 && EnemiesSpawned < 1)
                {
                    AITypeToSpawn(x, y);
                }
                else if (x > 2 && x < GridSize.x - 2 && y > GridSize.y - 3 && EnemiesSpawned < 4)
                {
                    int spawn = Random.Range(0, 5);
                    if (spawn == 0)
                    {
                        AITypeToSpawn(x, y);
                    }
                }

                Tile.transform.SetParent(transform, true);
                
                GameObject waypoint = new GameObject($"WayPoint {x},{y}", typeof(DetectClick), typeof(SphereCollider));
                waypoint.transform.position = GetPositionFromCoordinate(new Vector2Int(x, y));
                waypoint.transform.SetParent(Tile.transform, true);

            }
        }
        GameManager.Main.MakeIntOrder();
        GameManager.Main.UISetUp();
    }

    public Vector3 GetPositionFromCoordinate(Vector2Int Coordinate)
    {
        //This uses the vector 2 that all the game objects store and makes them in to vector 3 for translation of in game changes.
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
        //Clears the grid.
        for (var i = this.transform.childCount; i > 0; i-- )
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            };
        }
    }

    void AITypeToSpawn(int x, int y)
    {
        //Randomly chooses a AI type to spawn at the triggered spawn location.
        int spawn = Random.Range(0, 5);
        switch(spawn)
        {
            case 0:
                var temp = Instantiate(AiArcher, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp.transform.SetParent(transform, true);
                UnitBaseClass unit = temp.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit);
                unit.Pos = new Vector2Int(x, y);
                EnemiesSpawned++;
                unit.AIID = EnemiesSpawned;
                break;
            case 1:
                var temp1 = Instantiate(AiWarrior, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp1.transform.SetParent(transform, true);
                UnitBaseClass unit1 = temp1.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit1);
                unit1.Pos = new Vector2Int(x, y);
                EnemiesSpawned++;
                unit1.AIID = EnemiesSpawned;
                break;
            case 2:
                var temp2 = Instantiate(AiRogue, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp2.transform.SetParent(transform, true);
                UnitBaseClass unit2 = temp2.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit2);
                unit2.Pos = new Vector2Int(x, y);
                EnemiesSpawned++;
                unit2.AIID = EnemiesSpawned;
                break;
            case 3:
                var temp3 = Instantiate(AiTank, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp3.transform.SetParent(transform, true);
                UnitBaseClass unit3 = temp3.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit3);
                unit3.Pos = new Vector2Int(x, y);
                EnemiesSpawned++;
                unit3.AIID = EnemiesSpawned;
                break;
            case 4:
                var temp4 = Instantiate(AiMage, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp4.transform.SetParent(transform, true);
                UnitBaseClass unit4 = temp4.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit4);
                unit4.Pos = new Vector2Int(x, y);
                EnemiesSpawned++;
                unit4.AIID = EnemiesSpawned;
                break;
            default:
                var temp5 = Instantiate(AiWarrior, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp5.transform.SetParent(transform, true);
                UnitBaseClass unit5 = temp5.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit5);
                unit5.Pos = new Vector2Int(x, y);
                EnemiesSpawned++;
                unit5.AIID = EnemiesSpawned;
                break;
        }
    }

    void PlayerTypeToSpawn(int x, int y)
    {
        //Randomly chooses a Player unit type to spawn at the triggered spawn location.
        int spawn = Random.Range(0, 5);
        switch (spawn)
        {
            case 0:
                var temp = Instantiate(Archer, GetPositionFromCoordinate(new Vector2Int(x, y)) + new Vector3(0, 2, -1), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp.transform.SetParent(transform, true);
                UnitBaseClass unit = temp.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit);
                unit.Pos = new Vector2Int(x, y);
                PlayerSpawned++;
                unit.PlayerID = PlayerSpawned;
                break;
            case 1:
                var temp1 = Instantiate(Warrior, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp1.transform.SetParent(transform, true);
                UnitBaseClass unit1 = temp1.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit1);
                unit1.Pos = new Vector2Int(x, y);
                PlayerSpawned++;
                unit1.PlayerID = PlayerSpawned;
                break;
            case 2:
                var temp2 = Instantiate(Rogue, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp2.transform.SetParent(transform, true);
                UnitBaseClass unit2 = temp2.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit2);
                unit2.Pos = new Vector2Int(x, y);
                PlayerSpawned++;
                unit2.PlayerID = PlayerSpawned;
                break;
            case 3:
                var temp3 = Instantiate(Tank, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp3.transform.SetParent(transform, true);
                UnitBaseClass unit3 = temp3.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit3);
                unit3.Pos = new Vector2Int(x, y);
                PlayerSpawned++;
                unit3.PlayerID = PlayerSpawned;
                break;
            case 4:
                var temp4 = Instantiate(Mage, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp4.transform.SetParent(transform, true);
                UnitBaseClass unit4 = temp4.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit4);
                unit4.Pos = new Vector2Int(x, y);
                PlayerSpawned++;
                unit4.PlayerID = PlayerSpawned;
                break;
            default:
                var temp5 = Instantiate(Warrior, GetPositionFromCoordinate(new Vector2Int(x, y)), Quaternion.Euler(new Vector3(0, 180, 0)));
                temp5.transform.SetParent(transform, true);
                UnitBaseClass unit5 = temp5.GetComponent<UnitBaseClass>();
                GameManager.Main.UnitSetUp(unit5);
                unit5.Pos = new Vector2Int(x, y);
                PlayerSpawned++;
                unit5.PlayerID = PlayerSpawned;
                break;
        }
    }

    public GameObject FindWaypoint(Vector2Int pos)
    {
        //Finds the waypoint at the vector 2 location inputted.
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
        //Finds the movement cost of the hex at the vector 2 location inputted.
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
        //Finds the hex at the vector 2 location inputted.
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
