using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerArcher : MonoBehaviour
{
    GameObject CurrentWaypoint;
    public Vector2Int Pos;
    float Speed = 2f;
    float RotateSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {

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

        Move();
        
    }

    public void Move()
    {
        if (GameManager.Main.AStar.Pathway)
        {
            if (Vector3.Distance(this.transform.position, CurrentWaypoint.transform.position) < 0.2 && GameManager.Main.AStar.waypoint.Count > 0)
            {
                Pos = GameManager.Main.AStar.coords.Pop();
                GameManager.Main.PlayerArcher.Pos = Pos;
                //Debug.Log(Pos);
                //Debug.Log(GameManager.Main.PlayerArcher.Pos);
                CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
            }
            else if (Vector3.Distance(this.transform.position, CurrentWaypoint.transform.position) < 0.2)
            {
                Pos = GameManager.Main.AStar.coords.Pop();
                GameManager.Main.PlayerArcher.Pos = Pos;
                //Debug.Log(Pos);
                //Debug.Log(GameManager.Main.PlayerArcher.Pos);
                GameManager.Main.AStar.Pathway = false;
                GameManager.Main.AStar.Done = false;

            }

            //this.transform.LookAt(CurrentWaypoint.transform);
            Quaternion LookAtWP = Quaternion.LookRotation(CurrentWaypoint.transform.position - this.transform.position);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, LookAtWP, RotateSpeed * Time.deltaTime);

            this.transform.Translate(0, 0, Speed * Time.deltaTime);

        }
    }






}