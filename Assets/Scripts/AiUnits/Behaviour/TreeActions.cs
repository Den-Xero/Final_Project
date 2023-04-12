using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TreeActions : MonoBehaviour
{
    bool ConditionFulfilled = true;
    int CurrentMovementCost = 0;
    List<GameObject> TempWaypointList = new List<GameObject>();
    Stack<Vector2Int> TargetNeighbour = new Stack<Vector2Int>();
    Vector2Int NeighbourPoint;
    List<Hex> InRange = new List<Hex>();
    Stack<Hex> BestHexOrdered = new Stack<Hex>();
    //Condition leaf.
    protected TreeNodes.Status FConditionLeaf()
    {
        if (ConditionFulfilled) return TreeNodes.Status.SUCCESS;
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FFindClosestTarget()
    {
        UnitBaseClass currentClosest = null;
        float currentClosestDistance = 100;
        float tempDistance;
        foreach(UnitBaseClass unit in GameManager.Main.UnitIntOrder)
        {
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
            return TreeNodes.Status.SUCCESS;
        }
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FCanAttackWithEndTurn()
    {
        if (GameManager.Main.CurrentActiveUnit.CanAttack()) return TreeNodes.Status.SUCCESS;
        GameManager.Main.EndTurn();
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FCanAttackWithoutEndTurn()
    {
        if (GameManager.Main.CurrentActiveUnit.CanAttack()) return TreeNodes.Status.SUCCESS;
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FFindGoodHexToMoveTo()
    {
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
            return TreeNodes.Status.SUCCESS;
        }
        return TreeNodes.Status.FAILURE;
    }

    //the Actions leafs.
    protected TreeNodes.Status FAction1()
    {
        //return Movement(GameObject1.transform.position);
        return TreeNodes.Status.SUCCESS;
    }

    protected TreeNodes.Status FAttackAndSetEndTurn()
    {
        GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
        return GameManager.Main.CurrentActiveUnit.Attack();
    }

    protected TreeNodes.Status FSetAsMoved()
    {
        return GameManager.Main.SetAsMoved();
    }

    protected TreeNodes.Status FMove()
    {
        return FindHexesNextToTargetThatAreInRange();
    }

    protected TreeNodes.Status FWorkBackFromTargetToFindHex()
    {
        if(!GameManager.Main.AStar.SearchStarted)GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
        return FindWorkBackHex();
    }

    protected TreeNodes.Status FReadyEndTurn()
    {
        return GameManager.Main.EndTurn();
    }

    protected TreeNodes.Status FAction2()
    {
        //return Movement(GameObject2.transform.position);
        return TreeNodes.Status.SUCCESS;
    }


    
    TreeNodes.Status FindWorkBackHex()
    {
        if (GameManager.Main.AStar.Pathway) return Move();
        GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, 1000, true);

        if (!GameManager.Main.AStar.Done) return TreeNodes.Status.RUNNING;

        GameObject point = GameManager.Main.AStar.waypoint.Pop();
        Hex tempHex = point.GetComponentInParent<Hex>();
        CurrentMovementCost += tempHex.MovementCost;
        if(CurrentMovementCost >= GameManager.Main.CurrentActiveUnit.MovementPoints)
        {
            GameManager.Main.AStar.waypoint.Clear();
            for(int i = TempWaypointList.Count - 1; i >= 0; i--)
                GameManager.Main.AStar.waypoint.Push(TempWaypointList[i]);
            GameManager.Main.CurrentActiveUnit.MovementPointsUsed = GameManager.Main.AStar.GetPathway();
            GameManager.Main.AStar.Pathway = true;
            GameManager.Main.CurrentActiveUnit.CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
            return Move();
        }
        TempWaypointList.Add(point);
        return TreeNodes.Status.RUNNING;
    }

    TreeNodes.Status FindHexesNextToTargetThatAreInRange()
    {
        if(GameManager.Main.AStar.Pathway) return Move();
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
        TreeNodes.Status result = TreeNodes.Status.RUNNING;
        if (!GameManager.Main.AStar.Done && !GameManager.Main.AStar.Incomplete)
        { 
            result = GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, GameManager.Main.CurrentActiveUnit.MovementPoints, true);
        }
        if(result == TreeNodes.Status.RUNNING) return TreeNodes.Status.RUNNING;
        if (result == TreeNodes.Status.FAILURE) return TreeNodes.Status.FAILURE;

        if (GameManager.Main.AStar.Done && !GameManager.Main.AStar.Pathway)
        {
            GameManager.Main.CurrentActiveUnit.MovementPointsUsed = GameManager.Main.AStar.GetPathway();
            GameManager.Main.AStar.Pathway = true;
            GameManager.Main.CurrentActiveUnit.CurrentWaypoint = GameManager.Main.AStar.waypoint.Pop();
        }

        return GameManager.Main.CurrentActiveUnit.Move();

    }
}
