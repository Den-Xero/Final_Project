using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWarrior : UnitBaseClass
{
    private void Awake()
    {
        WarriorSetUp();
    }

    public override void UpdateLoop()
    {
        print(Health);
        Moved = true;
        GameManager.Main.EndTurn();
    }
}
