using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITank : UnitBaseClass
{
    TankAIBehaviour Tree;

    private void Awake()
    {
        TankSetUp();
        Tree = GetComponent<TankAIBehaviour>();
    }

    public override void UpdateLoop()
    {
        Tree.TreeUpdate();
    }
}
