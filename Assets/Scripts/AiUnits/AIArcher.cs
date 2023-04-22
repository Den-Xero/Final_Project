using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AIArcher : UnitBaseClass
{
    ArcherAIBehaviour Tree;
    private void Awake()
    {
        ArcherSetUp();
        Tree = GetComponent<ArcherAIBehaviour>();
    }

    public override void UpdateLoop()
    {
        Tree.TreeUpdate();
    }

}
