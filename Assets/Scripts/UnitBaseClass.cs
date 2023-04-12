using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBaseClass : MonoBehaviour
{
    public int Initiative = 0;
    protected int AttackRange = 0;
    protected int Health = 0;
    protected int MaxHealth = 100;
    public string UnitType = "";
    public string TileEffect = "";
    /* Effect list:
        Cobblestone, Dirt, Grassland = no effect.
        Sand = Range units have 2 less range, melee units take more damage or get stunned, Rogue has 25% change to ignore damage.
        Water = Unit can not attack.
        Mountain = Range units have 2 more range, melee units have a 25% change to ignore damage.
     */
    public int AttackDamage = 0;
    public bool PlayerUnit = false;
    public bool Moved = false;
    public bool Action = false;
    public bool Attacking = false;
    public bool Alive = true;
    public bool Ranged;
    int DamageReduction = 0;
    public int MovementPoints = 4;
    public int MovementPointsUsed;
    public Vector2Int Pos;
    protected float Speed = 2f;
    protected float RotateSpeed = 4f;
    float PercentChance = 0.25f;
    public UnitBaseClass AttackTarget;
    public GameObject CurrentWaypoint;
    public bool StartFindingPath = false;


    public virtual void UpdateLoop() { }

    public void TakeDamage(int damage)
    {
        if (!Ranged && TileEffect == "Mountain" || UnitType == "Rogue" && TileEffect == "Sand")
        {
            if(Random.value <= PercentChance)
            {
                if(UnitType == "Rogue")
                {
                    print("Attack Ignored");
                    return;
                }
                else
                {
                    DamageReduction = damage / 2;
                }
                
            }
        }
        if(!Ranged && TileEffect == "Sand" && UnitType != "Rogue")
        {
            Health -= damage + (damage/2);
        }
        else
        {
            Health -= damage - DamageReduction;
            DamageReduction = 0;
        }
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

    protected void MoveSetUp()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartFindingPath = true;
        if (StartFindingPath && !GameManager.Main.AStar.Done && !GameManager.Main.AStar.Incomplete) GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, MovementPoints, true);
        if (GameManager.Main.AStar.Done && !GameManager.Main.AStar.Pathway)
        {
            MovementPointsUsed = GameManager.Main.AStar.GetPathway();
            GameManager.Main.AStar.Pathway = true;
            CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
        }
    }

    public TreeNodes.Status Move()
    {
        if (!GameManager.Main.AStar.Pathway) return TreeNodes.Status.FAILURE;
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
            GameManager.Main.AStar.SearchStarted = false;
            StartFindingPath = false;
            Moved = true;
            //this.transform.LookAt(CurrentWaypoint.transform);
            Quaternion Last = Quaternion.LookRotation(CurrentWaypoint.transform.position + new Vector3(0, 2, -1) - this.transform.position);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Last, RotateSpeed * Time.deltaTime);

            this.transform.Translate(0, 0, Speed * Time.deltaTime);

            return TreeNodes.Status.SUCCESS;
        }

        //this.transform.LookAt(CurrentWaypoint.transform);
        Quaternion LookAtWP = Quaternion.LookRotation(CurrentWaypoint.transform.position + new Vector3(0, 2, -1) - this.transform.position);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, LookAtWP, RotateSpeed * Time.deltaTime);

        this.transform.Translate(0, 0, Speed * Time.deltaTime);

        return TreeNodes.Status.RUNNING;
    }

    public bool CanAttack()
    {
        if(TileEffect == "Water") return false;
        if (MovementPointsUsed > MovementPoints - 2)
        {
            return false;
        }

        return true;
    }


    public TreeNodes.Status Attack()
    {
        GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, MovementPoints, false);

        if (!GameManager.Main.AStar.Done) return TreeNodes.Status.RUNNING;

        GameManager.Main.AStar.GetPathway();
        if(Ranged && TileEffect == "Sand")
        {
            if (GameManager.Main.AStar.waypoint.Count > AttackRange - 2)
            {
                GameManager.Main.AStar.waypoint.Clear();
                GameManager.Main.AStar.RemoveAllMarkers();
                Attacking = false;
                GameManager.Main.AStar.Done = false;
                print("Unit not in attack range.");
                return TreeNodes.Status.FAILURE;
            }
        }
        else if (Ranged && TileEffect == "Mountain")
        {
            if (GameManager.Main.AStar.waypoint.Count > AttackRange + 2)
            {
                GameManager.Main.AStar.waypoint.Clear();
                GameManager.Main.AStar.RemoveAllMarkers();
                Attacking = false;
                GameManager.Main.AStar.Done = false;
                print("Unit not in attack range.");
                return TreeNodes.Status.FAILURE;
            }
        }
        else
        {
            if (GameManager.Main.AStar.waypoint.Count > AttackRange)
            {
                GameManager.Main.AStar.waypoint.Clear();
                GameManager.Main.AStar.RemoveAllMarkers();
                Attacking = false;
                GameManager.Main.AStar.Done = false;
                print("Unit not in attack range.");
                return TreeNodes.Status.FAILURE;
            }
        }

        GameManager.Main.AStar.RemoveAllMarkers();
        AttackTarget.TakeDamage(AttackDamage);
        print("Target hit");
        Attacking = false;
        Action = true;
        GameManager.Main.AStar.Done = false;
        return TreeNodes.Status.SUCCESS;
    }


    protected void TankSetUp()
    {
        Initiative = 1;
        MovementPoints = 2;
        AttackRange = 1;
        AttackDamage = 10;
        MaxHealth = 250;
        Health = MaxHealth;
        Ranged = false;
        UnitType = "Tank";
    }

    protected void RogueSetUp()
    {
        Initiative = 7;
        MovementPoints = 8;
        AttackRange = 1;
        AttackDamage = 30;
        MaxHealth = 100;
        Health = MaxHealth;
        Ranged = false;
        UnitType = "Rogue";
    }

    protected void MageSetUp()
    {
        Initiative = 2;
        MovementPoints = 3;
        AttackRange = 8;
        AttackDamage = 25;
        MaxHealth = 80;
        Health = MaxHealth;
        Ranged = true;
        UnitType = "Mage";
    }

    protected void WarriorSetUp()
    {
        Initiative = 3;
        MovementPoints = 4;
        AttackRange = 2;
        AttackDamage = 15;
        MaxHealth = 150;
        Health = MaxHealth;
        Ranged = false;
        UnitType = "Warrior";
    }

    protected void ArcherSetUp()
    {
        Initiative = 5;
        MovementPoints = 6;
        AttackRange = 4;
        AttackDamage = 20;
        MaxHealth = 100;
        Health = MaxHealth;
        Ranged = true;
        UnitType = "Archer";
    }

}
