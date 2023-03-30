using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectClick : MonoBehaviour
{
    bool EnemyAtLocation = false;
    void OnMouseDown()
    {
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
                if (!GameManager.Main.PlayerArcher.Moved)
                {
                    GameManager.Main.AStar.BeginSearch(par);
                    GameManager.Main.AStar.Pathway = false;
                    GameManager.Main.AStar.Incomplete = false;
                    GameManager.Main.AStar.Done = false;
                    GameManager.Main.PlayerArcher.StartFindingPath = false;
                    GameManager.Main.CurrentActiveUnit.TileEffect = par.Effect;
                }
                else
                {
                    if(GameManager.Main.PlayerArcher.Action) { print("Unit action used"); return; }
                    EnemyAtLocation = false;
                    if (!GameManager.Main.PlayerArcher.CanAttack()) { print("Unit has moved too far to attack this turn."); return; }
                    foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
                    {
                        if (unit.Pos == par.Coords && !unit.PlayerUnit && unit.Alive) 
                        {
                            print("A unit is at that location"); 
                            EnemyAtLocation = true; 
                            GameManager.Main.PlayerArcher.AttackTarget = unit; 
                            break; 
                        }
                    }
                    if(!EnemyAtLocation) { print("No unit at location"); return; }
                    GameManager.Main.PlayerArcher.Attacking = true;
                    GameManager.Main.AStar.BeginSearch(par);
                    GameManager.Main.PlayerArcher.Attack();
                }

            }
        }
    }

}
