using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRogue : UnitBaseClass
{
    RogueAIBehaviour Tree;

    private void Awake()
    {
        RogueSetUp();
        Tree = GetComponent<RogueAIBehaviour>();
    }

    public override void UpdateLoop()
    {
        Tree.TreeUpdate();
    }
}
