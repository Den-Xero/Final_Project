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
    protected TreeNodes.Status FDistanceToTarget()
    {
        //Finds the distnce to the target for attack and retreat check.
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
        //Some AI target the closest player unit to them so this finds the close unit for them to target.
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

    protected TreeNodes.Status FFindTargetWithHighestHealth()
    {
        //Mage attacks the player unit with the most current health, this will search though the unit list to find the player unit with the most health.
        UnitBaseClass currentHighest = null;
        float currentHighestHealth = 0;

        foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
        {
            if (!unit.PlayerUnit) continue;

            if (currentHighest == null)
            {
                currentHighest = unit;
                currentHighestHealth = unit.Health;
                continue;
            }

            if (unit.Health > currentHighestHealth)
            {
                currentHighest = unit;
                currentHighestHealth = unit.Health;
            }
        }

        if (currentHighest != null)
        {
            GameManager.Main.CurrentActiveUnit.AttackTarget = currentHighest;
            Debug.Log("Successful in finding target " + GameManager.Main.CurrentActiveUnit.name);
            return TreeNodes.Status.SUCCESS;
        }
        Debug.Log("Failed at finding target " + GameManager.Main.CurrentActiveUnit.name);
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FFindTargetWithLowestHealth()
    {
        //Rogue attacks the player unit with the lowest health, this will search through the unit list to find the player unit with the lowest health.
        UnitBaseClass currentLowest = null;
        float currentLowestHealth = 0;

        foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
        {
            if (!unit.PlayerUnit) continue;

            if (currentLowest == null)
            {
                currentLowest = unit;
                currentLowestHealth = unit.Health;
                continue;
            }

            if (unit.Health < currentLowestHealth)
            {
                currentLowest = unit;
                currentLowestHealth = unit.Health;
            }
        }

        if (currentLowest != null)
        {
            GameManager.Main.CurrentActiveUnit.AttackTarget = currentLowest;
            Debug.Log("Successful in finding target " + GameManager.Main.CurrentActiveUnit.name);
            return TreeNodes.Status.SUCCESS;
        }
        Debug.Log("Failed at finding target " + GameManager.Main.CurrentActiveUnit.name);
        return TreeNodes.Status.FAILURE;
    }

    protected TreeNodes.Status FFindClosestTargetToLowHealthAIUnit()
    {
        //The tank tries to protect it friendly units so will search for the lowest health friendly unit the find the closes player unit to the friendly unit and set that as the target in a effort to help and protect them.
        UnitBaseClass currentLowestHealthTeamMate = null;
        float currentLowestHealth = 0;
        UnitBaseClass currentClosest = null;
        float currentClosestDistance = 100;
        float tempDistance;

        foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
        {
            if (unit.PlayerUnit || unit == GameManager.Main.CurrentActiveUnit) continue;

            if(currentLowestHealthTeamMate == null)
            {
                currentLowestHealthTeamMate = unit;
                currentLowestHealth = unit.Health;
                continue;
            }

            if(unit.Health < currentLowestHealth)
            {
                currentLowestHealthTeamMate = unit;
                currentLowestHealth = unit.Health;
            }
        }


        if(currentLowestHealthTeamMate != null)
        {
            foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
            {
                if (!unit.PlayerUnit) continue;
                if (currentClosest == null)
                {
                    currentClosest = unit;
                    currentClosestDistance = Vector3.Distance(unit.transform.position, currentLowestHealthTeamMate.transform.position);
                    continue;
                }

                tempDistance = Vector3.Distance(unit.transform.position, currentLowestHealthTeamMate.transform.position);
                if (tempDistance < currentClosestDistance)
                {
                    currentClosest = unit;
                    currentClosestDistance = tempDistance;
                }
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
        //Checks if the AI can attack if so will if not will end the AI turn.
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
        //Checks if the AI can attack if so will if not will move on to next node.
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
        //AI will attempted to move so it can attack
        Debug.Log("Move So Can Attack " + GameManager.Main.CurrentActiveUnit.name);
        GameManager.Main.CurrentActiveUnit.Moved = true;
        GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
        GameManager.Main.CurrentActiveUnit.Moved = false;
        return TreeNodes.Status.SUCCESS;
    }

    protected TreeNodes.Status FFindGoodHexToMoveTo()
    {
        //AI will look for good hexes that it can move to.
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
                    bool IsTaken = false;
                    foreach(UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                    {
                        if( unit.Pos == neighbour)
                        {
                            IsTaken = true;
                        }
                    }
                    if (!IsTaken) TargetNeighbour.Push(neighbour);

                }

            }
            else
            {
                foreach (Vector2Int dir in GameManager.Main.GameBoard.PointTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.Coords;
                    bool IsTaken = false;
                    foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                    {
                        if (unit.Pos == neighbour)
                        {
                            IsTaken = true;
                        }
                    }
                    if (!IsTaken) TargetNeighbour.Push(neighbour);

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
                    bool IsTaken = false;
                    foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                    {
                        if (unit.Pos == neighbour)
                        {
                            IsTaken = true;
                        }
                    }
                    if (!IsTaken) TargetNeighbour.Push(neighbour);

                }

            }
            else
            {
                foreach (Vector2Int dir in GameManager.Main.GameBoard.FlatTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.Coords;
                    bool IsTaken = false;
                    foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                    {
                        if (unit.Pos == neighbour)
                        {
                            IsTaken = true;
                        }
                    }
                    if (!IsTaken) TargetNeighbour.Push(neighbour);

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

    protected TreeNodes.Status FMoveAwayFromTarget()
    {
        //If target is too close to AI it will try to move away.
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
        //AI will attack and then end its turn.
        Debug.Log("Attack and end turn " + GameManager.Main.CurrentActiveUnit.name);
        GameManager.Main.AStar.BeginSearch(GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
        TreeNodes.Status status = GameManager.Main.CurrentActiveUnit.Attack();
        if (status == TreeNodes.Status.RUNNING) return TreeNodes.Status.RUNNING;
        if (status == TreeNodes.Status.FAILURE) return TreeNodes.Status.FAILURE;
        return GameManager.Main.EndTurn();
    }

    protected TreeNodes.Status FSetAsMoved()
    {
        //AI acts as if it has moved without moving.
        Debug.Log("Set as moved " + GameManager.Main.CurrentActiveUnit.name);
        return GameManager.Main.SetAsMoved();
    }

    protected TreeNodes.Status FMove()
    {
        //AI tries to move to a hex next to the target so that it can attack.
        Debug.Log("Moving " + GameManager.Main.CurrentActiveUnit.name);
        return FindHexesNextToTargetThatAreInMovementRange();
    }

    protected TreeNodes.Status FWorkBackFromTargetToFindHex()
    {
        //Using target as the target will find the best path to follow and then move its max range on the path.
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
        //AI ends turn.
        Debug.Log("Ready end turn " + GameManager.Main.CurrentActiveUnit.name);
        return GameManager.Main.EndTurn();
    }

    
    TreeNodes.Status FindWorkBackHex()
    {
        //Using target as the target will find the best path to follow and then move its max range on the path.
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
            if (TempWaypointList.Count == 0) return GameManager.Main.SetAsMoved();
            if(!DoOnce)
            {
                DoOnce = true;
                GameObject X = TempWaypointList[TempWaypointList.Count - 1];
                Hex hex = X.GetComponentInParent<Hex>();
                foreach(UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                {
                    if(unit.Pos == hex.Coords)
                    {
                        TempWaypointList.RemoveAt(TempWaypointList.Count - 1);
                        DoOnce = false;
                        break;
                    }
                }
            }
            else
            {
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
            Finished = true;
            return TreeNodes.Status.RUNNING;

        }
        CurrentMovementCost = TempCurrentMovementCost;
        TempWaypointList.Add(point);
        return TreeNodes.Status.RUNNING;
    }

    public TreeNodes.Status FMoveToAttack()
    {
        //AI will attempted to move so it can attack
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
            if (TempWaypointList.Count == 0) return TreeNodes.Status.FAILURE;
            Finished = true;
            Debug.LogError("In waypoint stack Final " + GameManager.Main.AStar.waypoint.Count);
            Debug.LogError("In temp waypoint list Final " + TempWaypointList.Count);

            GameManager.Main.AStar.waypoint.Clear();
            GameManager.Main.AStar.RemoveAllMarkers();

            if (!DoOnce)
            {
                DoOnce = true;
                GameObject X = TempWaypointList[TempWaypointList.Count - 1];
                Hex hex = X.GetComponentInParent<Hex>();
                foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                {
                    if (unit.Pos == hex.Coords)
                    {
                        TempWaypointList.RemoveAt(TempWaypointList.Count - 1);
                        DoOnce = false;
                        break;
                    }
                }

                if(DoOnce) GameManager.Main.AStar.BeginSearchWithStartpoint(TempWaypointList[TempWaypointList.Count - 1].GetComponentInParent<Hex>(), GameManager.Main.GameBoard.FindHex(GameManager.Main.CurrentActiveUnit.AttackTarget.Pos));
                
            }
            else
            {
                TreeNodes.Status status = GameManager.Main.AStar.PathFinding(GameManager.Main.AStar.LastPos, GameManager.Main.CurrentActiveUnit.AttackRange, false);
                if (status == TreeNodes.Status.RUNNING) return TreeNodes.Status.RUNNING;
                if (status == TreeNodes.Status.FAILURE)
                {
                    Debug.Log("Not close enough to attack" + GameManager.Main.CurrentActiveUnit.name);
                    GameManager.Main.AStar.Incomplete = false;
                    GameManager.Main.AStar.SearchStarted = false;
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
            return TreeNodes.Status.RUNNING;

        }
        CurrentMovementCost = TempCurrentMovementCost;
        TempWaypointList.Add(point);
        return TreeNodes.Status.RUNNING;
    }

    TreeNodes.Status FindHexesNextToTargetThatAreInMovementRange()
    {
        //AI will look for good hexes next to its target to try and move to.
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
        else
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
        }

        GameManager.Main.AStar.BeginSearch(BestHexOrdered.Pop());
        return Move();
    }

    TreeNodes.Status Move()
    {
        //AI moves.
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
