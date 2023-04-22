using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMage : UnitBaseClass
{
    MageAIBehaviour Tree;

    private void Awake()
    {
        MageSetUp();
        Tree = GetComponent<MageAIBehaviour>();
    }

    public override void UpdateLoop()
    {
        Tree.TreeUpdate();
    }
}
