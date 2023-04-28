using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectClick : MonoBehaviour
{
    bool EnemyAtLocation = false;
    public Vector2Int Pos;
    void OnMouseDown()
    {
        if(!GameManager.Main.CurrentActiveUnit.PlayerUnit) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Determine which object was clicked on
            if (hit.collider.gameObject == gameObject)
            {
                // Do something with the clicked object
                //Debug.Log("Clicked on " + gameObject.name);
                Hex par = GetComponentInParent<Hex>();
                if (!GameManager.Main.CurrentActiveUnit.Moved)
                {
                    GameManager.Main.AStar.BeginSearch(par);
                    GameManager.Main.AStar.Pathway = false;
                    GameManager.Main.AStar.Incomplete = false;
                    GameManager.Main.AStar.Done = false;
                    GameManager.Main.CurrentActiveUnit.StartFindingPath = false;
                    GameManager.Main.CurrentActiveUnit.TileEffect = par.Effect;
                }
                else
                {
                    if(GameManager.Main.CurrentActiveUnit.Action) { print("Unit action used"); return; }
                    EnemyAtLocation = false;
                    if (GameManager.Main.CurrentActiveUnit.CanAttack() == TreeNodes.Status.FAILURE) { print("Unit can not attack this turn."); return; }
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
                    GameManager.Main.CurrentActiveUnit.Attacking = true;
                    GameManager.Main.AStar.BeginSearch(par);
                    GameManager.Main.CurrentActiveUnit.Attack();
                }

            }
        }
    }

}
