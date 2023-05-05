using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectClick : MonoBehaviour
{
    bool EnemyAtLocation = false;
    public Vector2Int Pos;
    void OnMouseDown()
    {
        //If the unit that is currently having a turn is not a player unit it will not do the code as to not mess with the AI turn.
        if(!GameManager.Main.CurrentActiveUnit.PlayerUnit) return;
        //Use raycasting to find where the player has clicked on the grid.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Determine which object was clicked on
            if (hit.collider.gameObject == gameObject)
            {
                //This gets the hex that the player want to interact with.
                Hex par = GetComponentInParent<Hex>();
                if (!GameManager.Main.CurrentActiveUnit.Moved)
                {
                    //If the player unit has not moved it will run the start of the pathfinding code so it can move to the hex clicked on.
                    GameManager.Main.AStar.BeginSearch(par);
                    GameManager.Main.AStar.Pathway = false;
                    GameManager.Main.AStar.Incomplete = false;
                    GameManager.Main.AStar.Done = false;
                    GameManager.Main.CurrentActiveUnit.StartFindingPath = false;
                    GameManager.Main.CurrentActiveUnit.TileEffect = par.Effect;
                }
                else
                {
                    //Stops the player from making more then one action.
                    if(GameManager.Main.CurrentActiveUnit.Action) { print("Unit action used"); return; }
                    //Cheack the hex to see if any AI units are on the tile.
                    EnemyAtLocation = false;
                    foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                    {
                        if (unit.Pos == par.Coords && !unit.PlayerUnit && unit.Alive) 
                        {
                            print("A unit is at that location"); 
                            EnemyAtLocation = true; 
                            GameManager.Main.CurrentActiveUnit.AttackTarget = unit; 
                            break; 
                        }
                    }
                    if(!EnemyAtLocation) { print("No unit at location"); return; }
                    //If there is one on the tile the player unit will see if it can attack.
                    GameManager.Main.CurrentActiveUnit.Attacking = true;
                    GameManager.Main.AStar.BeginSearch(par);

                    if (GameManager.Main.CurrentActiveUnit.Ranged)
                        GameManager.Main.CurrentActiveUnit.RangeCheckToTarget();
                    else
                        GameManager.Main.CurrentActiveUnit.MeleeCheckToTarget();

                    if (GameManager.Main.CurrentActiveUnit.CanAttack() == TreeNodes.Status.FAILURE) { print("Unit can not attack this turn."); return; }
                    //If it can attack it will attack the AI unit.
                    GameManager.Main.CurrentActiveUnit.Attack();
                }

            }
        }
    }

}
