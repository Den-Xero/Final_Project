using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMage : UnitBaseClass
{
    private void Awake()
    {
        MageSetUp();
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
