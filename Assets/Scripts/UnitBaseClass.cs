using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBaseClass : MonoBehaviour
{
    public int Initiative = 0;
    protected int AttackRange = 0;
    protected int Health = 100;
    public bool PlayerUnit = false;
    public bool Moved = false;
    public bool Action = false;
    public bool Attacking = false;
    protected int MovementPoints = 4;
    protected int MovementPointsUsed;
    public Vector2Int Pos;
    protected float Speed = 2f;
    protected float RotateSpeed = 4f;
    public UnitBaseClass AttackTarget;
    protected GameObject CurrentWaypoint;
    public bool StartFindingPath = false;
    // Start is called before the first frame update

    public virtual void UpdateLoop() { }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    protected void Move()
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

    public bool CanAttack()
    {
        if (MovementPointsUsed > MovementPoints - 2)
        {
            return false;
        }

        return true;
    }


    public void Attack()
    {
        GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, MovementPoints, false);

        if (!GameManager.Main.AStar.Done) return;

        GameManager.Main.AStar.GetPathway();
        if (GameManager.Main.AStar.waypoint.Count > AttackRange)
        {
            GameManager.Main.AStar.waypoint.Clear();
            GameManager.Main.AStar.RemoveAllMarkers();
            Attacking = false;
            GameManager.Main.AStar.Done = false;
            print("Unit not in attack range.");
            return;
        }
        GameManager.Main.AStar.RemoveAllMarkers();
        AttackTarget.TakeDamage(10);
        print("Target hit");
        Attacking = false;
        Action = true;
        GameManager.Main.AStar.Done = false;
    }

}
