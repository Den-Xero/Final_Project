using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWarrior : UnitBaseClass
{
    private void Awake()
    {
        Initiative = 3;
        MovementPoints = 4;
        AttackRange = 1;
    }

    public override void UpdateLoop()
    {
        print(Health);
        Moved = true;
        GameManager.Main.EndTurn();
    }
}
