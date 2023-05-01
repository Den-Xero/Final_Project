using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarrior : UnitBaseClass
{
    private void Awake()
    {
        WarriorSetUp();
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
