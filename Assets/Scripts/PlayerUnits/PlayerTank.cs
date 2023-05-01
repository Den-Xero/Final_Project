using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : UnitBaseClass
{
    private void Awake()
    {
        TankSetUp();
        PlayerUnit = true;
    }

    public override void UpdateLoop()
    {
        if (Action) return;
        if (Moved)
        {
            if (!GameManager.Main.AStar.Done && Attacking)
            {
                Attack();
            }
            return;
        }

        MoveSetUp();

        if (!Moved) Move();

    }

}
