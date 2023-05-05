using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : UnitBaseClass
{
    private void Awake()
    {
        //Sets the starting values for the unit and Makes it so the unit knows that it is a player unit.
        TankSetUp();
        PlayerUnit = true;
    }

    public override void UpdateLoop()
    {
        //If the player has done the action for the unit this turn nothing more can be don't till player ends turn.
        if (Action) return;
        //If player has moved they can now attack if in range of a AI unit.
        if (Moved)
        {
            if (!GameManager.Main.AStar.Done && Attacking)
            {
                Attack();
            }
            return;
        }

        //Does all the set up that will allow the player unit to move.
        MoveSetUp();

        //If player unit has not moved it will do its move.
        if (!Moved) Move();

    }

}
