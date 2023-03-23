using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBaseClass : MonoBehaviour
{
    public int Initiative = 0;
    protected int AttackRange = 0;
    protected int Health = 0;
    protected int MaxHealth = 100;
    public int AttackDamage = 0;
    public bool PlayerUnit = false;
    public bool Moved = false;
    public bool Action = false;
    public bool Attacking = false;
    public bool Alive = true;
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
        if (Health <= 0)
        {
            for (int i = 0; i < GameManager.Main.UnitIntOrder.Count; i++)
            {
                if (GameManager.Main.UnitIntOrder[i] == this)
                {
                    GameManager.Main.UnitIntOrder.RemoveAt(i);
                    break;
                }
            }
            Alive = false;
            this.gameObject.SetActive(false);
        }
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


    protected void TankSetUp()
    {
        Initiative = 1;
        MovementPoints = 2;
        AttackRange = 1;
        AttackDamage = 10;
        MaxHealth = 250;
        Health = MaxHealth;
        
    }

    protected void RogueSetUp()
    {
        Initiative = 7;
        MovementPoints = 8;
        AttackRange = 1;
        AttackDamage = 30;
        MaxHealth = 100;
        Health = MaxHealth;
    }

    protected void MageSetUp()
    {
        Initiative = 2;
        MovementPoints = 3;
        AttackRange = 8;
        AttackDamage = 25;
        MaxHealth = 80;
        Health = MaxHealth;

    }

    protected void WarriorSetUp()
    {
        Initiative = 3;
        MovementPoints = 4;
        AttackRange = 2;
        AttackDamage = 15;
        MaxHealth = 150;
        Health = MaxHealth;
    }

    protected void ArcherSetUp()
    {
        Initiative = 5;
        MovementPoints = 6;
        AttackRange = 4;
        AttackDamage = 20;
        MaxHealth = 100;
        Health = MaxHealth;
    }

}
