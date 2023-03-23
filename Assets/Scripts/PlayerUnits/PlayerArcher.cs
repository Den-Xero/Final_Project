using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.XR;

public class PlayerArcher : UnitBaseClass
{

    private void Awake()
    {
        ArcherSetUp();
        PlayerUnit = true;
    }

    public override void UpdateLoop()
    {
        if(Action)return;
        if (Moved) 
        { 
            if(!GameManager.Main.AStar.Done && Attacking)
            {
                Attack();
            }
            return; 
        }
        if (Input.GetKeyDown(KeyCode.Space)) StartFindingPath = true;
        if (StartFindingPath && !GameManager.Main.AStar.Done && !GameManager.Main.AStar.Incomplete) GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, MovementPoints, true);
        if (GameManager.Main.AStar.Done && !GameManager.Main.AStar.Pathway)
        {
            MovementPointsUsed = GameManager.Main.AStar.GetPathway();
            GameManager.Main.AStar.Pathway = true;
            CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
        }

        if(!Moved) Move();

    }


}
