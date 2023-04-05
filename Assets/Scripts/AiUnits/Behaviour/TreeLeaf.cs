using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLeaf : TreeNodes
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    //Blank constructor.
    public TreeLeaf() { }

    //Constructor with name.
    public TreeLeaf(string n, Tick pm)
    {
        Name = n;
        ProcessMethod = pm;
    }

    //Keep track of the tree nodes progress to see if node has successed, failed or still running.
    public override Status Process()
    {
        if (ProcessMethod != null) return ProcessMethod();
        return Status.FAILURE;
    }


}
