using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.XR;

public class PlayerArcher : UnitBaseClass
{
    GameObject CurrentWaypoint;
    public Vector2Int Pos;
    float Speed = 2f;
    float RotateSpeed = 4f;
    public bool StartFindingPath = false;
    

    private void Awake()
    {
        Initiative = 5;
        PlayerUnit = true;
        MovementPoints = 4;
    }

    public override void UpdateLoop()
    {
        if(Moved)return;
        if (Input.GetKeyDown(KeyCode.Space)) StartFindingPath = true;
        if (StartFindingPath && !GameManager.Main.AStar.Done && !GameManager.Main.AStar.Incomplete) GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, MovementPoints);
        if (GameManager.Main.AStar.Done && !GameManager.Main.AStar.Pathway)
        {
            MovementPointsUsed = GameManager.Main.AStar.GetPathway();
            GameManager.Main.AStar.Pathway = true;
            CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
        }

        Move();
    }

    public void Move()
    {
        if (!GameManager.Main.AStar.Pathway) return;
        if (Vector3.Distance(this.transform.position, CurrentWaypoint.transform.position + new Vector3(0, 2, -1)) < 0.2 && GameManager.Main.AStar.waypoint.Count > 0)
        {
             Pos = GameManager.Main.AStar.coords.Pop();
             CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
        }
        else if (Vector3.Distance(this.transform.position, CurrentWaypoint.transform.position + new Vector3(0, 2, -1)) < 0.2)
        {
             Pos = GameManager.Main.AStar.coords.Pop();
             GameManager.Main.AStar.RemoveAllMarkers();
             GameManager.Main.AStar.Pathway = false;
             GameManager.Main.AStar.Done = false;
             StartFindingPath = false;
             Moved = true;
        }

        //this.transform.LookAt(CurrentWaypoint.transform);
        Quaternion LookAtWP = Quaternion.LookRotation(CurrentWaypoint.transform.position + new Vector3(0, 2, -1) - this.transform.position);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, LookAtWP, RotateSpeed * Time.deltaTime);

        this.transform.Translate(0, 0, Speed * Time.deltaTime);
    }






}
