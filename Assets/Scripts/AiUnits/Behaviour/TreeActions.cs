using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TreeActions : MonoBehaviour
{
    int CurrentMovementCost = 0;
    int TempCurrentMovementCost = 0;
    List<GameObject> TempWaypointList = new List<GameObject>();
    Stack<Vector2Int> TargetNeighbour = new Stack<Vector2Int>();
    Vector2Int NeighbourPoint;
    List<Hex> InRange = new List<Hex>();
    Stack<Hex> BestHexOrdered = new Stack<Hex>();
    bool DoOnce = false;
    bool DoOncePath = false;
    bool Finished = false;
    //Condition leaf.
    protected TreeNodes.Status FConditionLeaf()
    {
        if (true) return TreeNodes.Status.SUCCESS;
        //return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FDistanceToTarget()
    {
        Debug.Log("Find distance to target " + GameManager.Main.CurrentActiveUnit.name);
        if (!DoOnce)
        {
            GameManager.Main.CurrentActiveUnit.Moved = true;
            GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
            GameManager.Main.CurrentActiveUnit.Moved = false;
            DoOnce = true;
        }
        TreeNodes.Status status;
        if (GameManager.Main.CurrentActiveUnit.Ranged)
            status = GameManager.Main.CurrentActiveUnit.RangeCheckToTarget();
        else
            status = GameManager.Main.CurrentActiveUnit.MeleeCheckToTarget();

        if (status != TreeNodes.Status.RUNNING) DoOnce = false;
        return status;
    }


    protected TreeNodes.Status FFindClosestTarget()
    {
        Debug.Log("Start Find target " + GameManager.Main.CurrentActiveUnit.name);
        UnitBaseClass currentClosest = null;
        float currentClosestDistance = 100;
        float tempDistance;
        foreach(UnitBaseClass unit in GameManager.Main.UnitIntOrder)
        {
            if(!unit.PlayerUnit) continue;
            if(currentClosest == null)
            {
                currentClosest = unit;
                currentClosestDistance = Vector3.Distance(unit.transform.position, transform.position);
                continue;
            }

            tempDistance = Vector3.Distance(unit.transform.position, transform.position);
            if(tempDistance < currentClosestDistance)
            {
                currentClosest = unit;
                currentClosestDistance = tempDistance;
            }
        }

        if (currentClosest != null)
        {
            GameManager.Main.CurrentActiveUnit.AttackTarget = currentClosest;
            Debug.Log("Successful in finding target " + GameManager.Main.CurrentActiveUnit.name);
            return TreeNodes.Status.SUCCESS;
        }
        Debug.Log("Failed at finding target " + GameManager.Main.CurrentActiveUnit.name);
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FCanAttackWithEndTurn()
    {
        Debug.Log("Can attack with end turn " + GameManager.Main.CurrentActiveUnit.name);
        if (GameManager.Main.CurrentActiveUnit.CanAttack() == TreeNodes.Status.SUCCESS)
        {
            Debug.Log("Can attack success " + GameManager.Main.CurrentActiveUnit.name);
            return TreeNodes.Status.SUCCESS;
        }
        Debug.Log("Can attack failed ending turn " + GameManager.Main.CurrentActiveUnit.name);
        GameManager.Main.EndTurn();
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FCanAttackWithoutEndTurn()
    {
        Debug.Log("Can attack without end turn " + GameManager.Main.CurrentActiveUnit.name);
        if (GameManager.Main.CurrentActiveUnit.CanAttack() == TreeNodes.Status.SUCCESS)
        {
            Debug.Log("Can attack success " + GameManager.Main.CurrentActiveUnit.name);
            return TreeNodes.Status.SUCCESS;
        }
        Debug.Log("Can attack failed " + GameManager.Main.CurrentActiveUnit.name);
        return TreeNodes.Status.FAILURE;
    }


    protected TreeNodes.Status FMoveSoCanAttack()
    {
        Debug.Log("Move So Can Attack " + GameManager.Main.CurrentActiveUnit.name);
        GameManager.Main.CurrentActiveUnit.Moved = true;
        GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
        GameManager.Main.CurrentActiveUnit.Moved = false;
        return TreeNodes.Status.SUCCESS;
    }

    protected TreeNodes.Status FFindGoodHexToMoveTo()
    {
        //Need to change this so that it see if unit has enough movement to get to the hex.
        Debug.Log("Start Find good hex to move to " + GameManager.Main.CurrentActiveUnit.name);
        if (GameManager.Main.CurrentActiveUnit.AttackTargetDistance > GameManager.Main.CurrentActiveUnit.MovementPoints - 2 + GameManager.Main.CurrentActiveUnit.AttackRange)
        {
            Debug.Log("Too far need max movement " + GameManager.Main.CurrentActiveUnit.name);
            return TreeNodes.Status.FAILURE;
        }
        Hex ThisHex = GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos);
        if (!GameManager.Main.GameBoard.FlatTop)
        {
            if (ThisHex.Coords.y % 2 == 0)
            {
                foreach (Vector2Int dir in GameManager.Main.GameBoard.PointTopOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.Coords;
                    TargetNeighbour.Push(neighbour);

                }

            }
            else
            {
                foreach (Vector2Int dir in GameManager.Main.GameBoard.PointTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.Coords;
                    TargetNeighbour.Push(neighbour);

                }

            }
        }
        else
        {
            if (ThisHex.Coords.x % 2 == 0)
            {
                foreach (Vector2Int dir in GameManager.Main.GameBoard.FlatTopOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.Coords;
                    TargetNeighbour.Push(neighbour);

                }

            }
            else
            {
                foreach (Vector2Int dir in GameManager.Main.GameBoard.FlatTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.Coords;
                    TargetNeighbour.Push(neighbour);

                }

            }
        }

        if (TargetNeighbour.Count > 0)
        {
            NeighbourPoint = TargetNeighbour.Pop();
            GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(NeighbourPoint));
            Debug.Log("found good hex to move to " + GameManager.Main.CurrentActiveUnit.name);
            return TreeNodes.Status.SUCCESS;
        }
        Debug.Log("Failed to find a hex to move to " + GameManager.Main.CurrentActiveUnit.name);
        return TreeNodes.Status.FAILURE;
    }

    //the Actions leafs.
    protected TreeNodes.Status FAction1()
    {
        //return Movement(GameObject1.transform.position);
        return TreeNodes.Status.SUCCESS;
    }

    protected TreeNodes.Status FMoveAwayFromTarget()
    {
        Debug.Log("Start Move away " + GameManager.Main.CurrentActiveUnit.name);
        if (!GameManager.Main.AStar.SearchStarted)
        {
            GameManager.Main.AStar.MoveAwayStart();
            TreeNodes.Status status = GameManager.Main.AStar.FindMoveAway(GameManager.Main.AStar.LastPos, GameManager.Main.CurrentActiveUnit.MovementPoints - 2, true);
            if (status == TreeNodes.Status.RUNNING) return TreeNodes.Status.RUNNING;
            Debug.Log("Not moving away from target " + GameManager.Main.CurrentActiveUnit.name);
            if (status == TreeNodes.Status.FAILURE) return TreeNodes.Status.FAILURE;
        }
        Debug.Log("Moving away from target " + GameManager.Main.CurrentActiveUnit.name);
        return Move();

    }

    protected TreeNodes.Status FAttackAndSetEndTurn()
    {
        Debug.Log("Attack and end turn " + GameManager.Main.CurrentActiveUnit.name);
        GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
        return GameManager.Main.CurrentActiveUnit.Attack();
    }

    protected TreeNodes.Status FSetAsMoved()
    {
        Debug.Log("Set as moved " + GameManager.Main.CurrentActiveUnit.name);
        return GameManager.Main.SetAsMoved();
    }

    protected TreeNodes.Status FMove()
    {
        Debug.Log("Moving " + GameManager.Main.CurrentActiveUnit.name);
        return FindHexesNextToTargetThatAreInMovementRange();
    }

    protected TreeNodes.Status FWorkBackFromTargetToFindHex()
    {
        if (!GameManager.Main.AStar.SearchStarted)
        {
            Debug.Log("Work back from hex " + GameManager.Main.CurrentActiveUnit.name);
            GameManager.Main.CurrentActiveUnit.Moved = true;
            GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
            GameManager.Main.CurrentActiveUnit.Moved = false;
        }
        return FindWorkBackHex();
    }

    protected TreeNodes.Status FReadyEndTurn()
    {
        Debug.Log("Ready end turn " + GameManager.Main.CurrentActiveUnit.name);
        return GameManager.Main.EndTurn();
    }

    protected TreeNodes.Status FAction2()
    {
        //return Movement(GameObject2.transform.position);
        return TreeNodes.Status.SUCCESS;
    }


    
    TreeNodes.Status FindWorkBackHex()
    {
        Debug.Log("Find work back hex " + GameManager.Main.CurrentActiveUnit.name);
        if (GameManager.Main.AStar.Pathway) return Move();
        GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, 1000, true);

        if (!GameManager.Main.AStar.Done) return TreeNodes.Status.RUNNING;
        if (!DoOncePath)
        {
            GameManager.Main.AStar.GetPathway();
            DoOncePath = true;
        }

        GameObject point = null;

        if (!Finished)
        {
            Debug.Log("In waypoint stack " + GameManager.Main.AStar.waypoint.Count);
            Debug.Log("In temp waypoint list " + TempWaypointList.Count);
            point = GameManager.Main.AStar.waypoint.Pop();
            Hex tempHex = point.GetComponentInParent<Hex>();
            TempCurrentMovementCost += tempHex.MovementCost;
        }
        
        if(TempCurrentMovementCost > GameManager.Main.CurrentActiveUnit.MovementPoints)
        {
            Finished = true;
            Debug.LogError("In waypoint stack Final " + GameManager.Main.AStar.waypoint.Count);
            Debug.LogError("In temp waypoint list Final " + TempWaypointList.Count);
            GameManager.Main.AStar.waypoint.Clear();
            GameManager.Main.AStar.RemoveAllMarkers();
            Debug.LogError("In waypoint stack after clear " + GameManager.Main.AStar.waypoint.Count);

            for (int i = TempWaypointList.Count - 1; i >= 0; i--)
                GameManager.Main.AStar.waypoint.Push(TempWaypointList[i]);
          

            Debug.LogError("In waypoint stack New " + GameManager.Main.AStar.waypoint.Count);
            Debug.LogError("In temp waypoint list New " + TempWaypointList.Count);

            GameManager.Main.CurrentActiveUnit.MovementPointsUsed = CurrentMovementCost;
            GameManager.Main.AStar.Pathway = true;
            GameManager.Main.CurrentActiveUnit.CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
            DoOncePath = false;
            Finished = false;
            
            TempWaypointList.Clear();
            TempCurrentMovementCost = 0;
            CurrentMovementCost = 0;
            return Move();
        }
        CurrentMovementCost = TempCurrentMovementCost;
        TempWaypointList.Add(point);
        return TreeNodes.Status.RUNNING;
    }

    public TreeNodes.Status FMoveToAttack()
    {
        Debug.Log("Move to attack " + GameManager.Main.CurrentActiveUnit.name);
        if (GameManager.Main.AStar.Pathway) return Move();
        if (!GameManager.Main.AStar.Done)
        {
            GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, 10000, true);
            return TreeNodes.Status.RUNNING;
        }

        if (!DoOncePath)
        {
            GameManager.Main.AStar.GetPathway();
            DoOncePath = true;
        }

        GameObject point = null;

        if (!Finished)
        {
            Debug.Log("In waypoint stack " + GameManager.Main.AStar.waypoint.Count);
            Debug.Log("In temp waypoint list " + TempWaypointList.Count);

            point = GameManager.Main.AStar.waypoint.Pop();

            Hex tempHex = point.GetComponentInParent<Hex>();

            TempCurrentMovementCost += tempHex.MovementCost;
            
        }
        

        if (TempCurrentMovementCost > GameManager.Main.CurrentActiveUnit.MovementPoints - 2)
        {
            Finished = true;
            Debug.LogError("In waypoint stack Final " + GameManager.Main.AStar.waypoint.Count);
            Debug.LogError("In temp waypoint list Final " + TempWaypointList.Count);

            GameManager.Main.AStar.waypoint.Clear();
            GameManager.Main.AStar.RemoveAllMarkers();

            if (!DoOnce)
            {
                GameManager.Main.AStar.BeginSearchWithStartpoint(TempWaypointList[TempWaypointList.Count - 1].GetComponentInParent<Hex>(), GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
                DoOnce = true;
            }

            TreeNodes.Status status = GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, GameManager.Main.CurrentActiveUnit.AttackRange, false);
            if (status == TreeNodes.Status.RUNNING) return TreeNodes.Status.RUNNING;
            if (status == TreeNodes.Status.FAILURE)
            {
                Debug.Log("Not close enough to attack" + GameManager.Main.CurrentActiveUnit.name);
                GameManager.Main.AStar.Incomplete = false;
                DoOncePath = false;
                DoOnce = false;
                Finished = false;
                TempWaypointList.Clear();
                return TreeNodes.Status.FAILURE;
            }
            Debug.Log("close enough to attack " + GameManager.Main.CurrentActiveUnit.name);
            for (int i = TempWaypointList.Count - 1; i >= 0; i--)
                GameManager.Main.AStar.waypoint.Push(TempWaypointList[i]);
            
            GameManager.Main.CurrentActiveUnit.MovementPointsUsed = CurrentMovementCost;
            GameManager.Main.AStar.Pathway = true;
            GameManager.Main.CurrentActiveUnit.CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
            DoOncePath = false;
            DoOnce = false;
            Finished = false;
            Debug.LogError("In waypoint stack New " + GameManager.Main.AStar.waypoint.Count);
            Debug.LogError("In temp waypoint list New " + TempWaypointList.Count);
            TempWaypointList.Clear();
            TempCurrentMovementCost = 0;
            CurrentMovementCost = 0;
            return Move();
        }
        CurrentMovementCost = TempCurrentMovementCost;
        TempWaypointList.Add(point);
        return TreeNodes.Status.RUNNING;
    }

    TreeNodes.Status FindHexesNextToTargetThatAreInMovementRange()
    {
        Debug.Log("Find hexes next to target that are in range " + GameManager.Main.CurrentActiveUnit.name);
        if (GameManager.Main.AStar.Pathway) return Move();
        TreeNodes.Status result = GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, GameManager.Main.CurrentActiveUnit.MovementPoints, true);
        if(result == TreeNodes.Status.RUNNING) return result;
        if(TargetNeighbour.Count > 0)
        {
            if (result == TreeNodes.Status.FAILURE)
            {
                NeighbourPoint = TargetNeighbour.Pop();
                GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(NeighbourPoint));
                return TreeNodes.Status.RUNNING;
            }

            if(result == TreeNodes.Status.SUCCESS)
            {
                InRange.Add(GameManager.Main.GameBoard.FindHex(NeighbourPoint));
                NeighbourPoint = TargetNeighbour.Pop();
                GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(NeighbourPoint));
                return TreeNodes.Status.RUNNING;
            }
        }

        if (result == TreeNodes.Status.SUCCESS)
        {
            InRange.Add(GameManager.Main.GameBoard.FindHex(NeighbourPoint));
        }

        if(InRange.Count == 0)
        {
            return TreeNodes.Status.FAILURE;
        }

        if(GameManager.Main.CurrentActiveUnit.UnitType == "Rogue")
        {
            for (int i = 1; i < InRange.Count; i++)
            {
                if (InRange[i].Effect == "Water") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
            }

            if (InRange.Count == 0)
            {
                GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
                return Move();
            }

            for (int i = 1; i < InRange.Count; i++)
            {
                if (InRange[i].Effect == "Cobblestone" || InRange[i].Effect == "Dirt" || InRange[i].Effect == "Grassland") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
            }

            if (InRange.Count == 0)
            {
                GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
                return Move();
            }

            for (int i = 1; i < InRange.Count; i++)
            {
                if (InRange[i].Effect == "Sand") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
            }

            if (InRange.Count == 0)
            {
                GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
                return Move();
            }

            for (int i = 1; i < InRange.Count; i++)
            {
                if (InRange[i].Effect == "Mountain") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
            }
        }

        for(int i = 1; i < InRange.Count; i++)
        {
            if (InRange[i].Effect == "Water") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
        }

        if (InRange.Count == 0)
        {
            GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
            return Move();
        }

        for (int i = 1; i < InRange.Count; i++)
        {
            if (InRange[i].Effect == "Sand") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
        }

        if (InRange.Count == 0)
        {
            GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
            return Move();
        }

        for (int i = 1; i < InRange.Count; i++)
        {
            if (InRange[i].Effect == "Cobblestone" || InRange[i].Effect == "Dirt" || InRange[i].Effect == "Grassland") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
        }

        if (InRange.Count == 0)
        {
            GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
            return Move();
        }

        for (int i = 1; i < InRange.Count; i++)
        {
            if (InRange[i].Effect == "Mountain") BestHexOrdered.Push(InRange[i]); InRange.RemoveAt(i);
        }

        GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
        return Move();
    }

    TreeNodes.Status Move()
    {
        Debug.Log("The movement private function " + GameManager.Main.CurrentActiveUnit.name);

        if (GameManager.Main.AStar.Pathway) return GameManager.Main.CurrentActiveUnit.Move();

        
        TreeNodes.Status result = TreeNodes.Status.SUCCESS;
        if (!GameManager.Main.AStar.Done && !GameManager.Main.AStar.Incomplete)
        {
            print("Finding paths for " + GameManager.Main.CurrentActiveUnit.name);
            result = GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, GameManager.Main.CurrentActiveUnit.MovementPoints, true);
        }
        if (result == TreeNodes.Status.RUNNING) return TreeNodes.Status.RUNNING;
        if (result == TreeNodes.Status.FAILURE) return TreeNodes.Status.FAILURE;

        if (GameManager.Main.AStar.Done && !GameManager.Main.AStar.Pathway)
        {
            GameManager.Main.CurrentActiveUnit.MovementPointsUsed = GameManager.Main.AStar.GetPathway();
            GameManager.Main.AStar.Pathway = true;
            GameManager.Main.CurrentActiveUnit.CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
        }

        print("Starting to move");
        return GameManager.Main.CurrentActiveUnit.Move();

    }
}
