using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FollowWaypoints : MonoBehaviour
{
    GameObject CurrentWaypoint;
    public Vector2Int Pos;
    float Speed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Pos = GameManager.Main.PlayerCoords;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !GameManager.Main.AStar.Done) GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos);
        if (GameManager.Main.AStar.Done && !GameManager.Main.AStar.Pathway)
        {
            GameManager.Main.AStar.GetPathway();
            GameManager.Main.AStar.Pathway = true;
            CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
        }

        if (GameManager.Main.AStar.Pathway)
        {
            if (Vector3.Distance(this.transform.position, CurrentWaypoint.transform.position) < 0.2 && GameManager.Main.AStar.waypoint.Count > 0)
            {
                GameManager.Main.PlayerCoords = GameManager.Main.AStar.coords.Pop();
                Pos = GameManager.Main.PlayerCoords;
                CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
            }
            else if (Vector3.Distance(this.transform.position, CurrentWaypoint.transform.position) < 0.2)
            {
                GameManager.Main.PlayerCoords = GameManager.Main.AStar.coords.Pop();
                Pos = GameManager.Main.PlayerCoords;
                GameManager.Main.AStar.Pathway = false;
                GameManager.Main.AStar.Done = false;

            }

            this.transform.LookAt(CurrentWaypoint.transform);
            this.transform.Translate(0, 0, Speed * Time.deltaTime);
            
        }
    }
}
