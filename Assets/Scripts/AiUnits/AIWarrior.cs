using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWarrior : UnitBaseClass
{
    WarriorAIBehaviour Tree;

    private void Awake()
    {
        WarriorSetUp();
        Tree = GetComponent<WarriorAIBehaviour>();
    }

    public override void UpdateLoop()
    {
        Tree.TreeUpdate();
    }
}
