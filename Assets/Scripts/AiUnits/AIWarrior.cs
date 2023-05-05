using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWarrior : UnitBaseClass
{
    //Stores the AI tree that this AI needs to follow.
    WarriorAIBehaviour Tree;

    private void Awake()
    {
        //Sets the starting values for the unit and gets the tree the is attached to the unit so it can store and run it.
        WarriorSetUp();
        Tree = GetComponent<WarriorAIBehaviour>();
    }

    public override void UpdateLoop()
    {
        //When it is the units turn it will run the tree.
        Tree.TreeUpdate();
    }
}
