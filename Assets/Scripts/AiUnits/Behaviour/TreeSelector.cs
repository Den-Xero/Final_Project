using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSelector : TreeNodes
{
    //Blank constructor.
    public TreeSelector() { }

    //Constructor with name.
    public TreeSelector(string n)
    {
        Name = n;
    }

    //Runs through all the children of the selector and check the status of each node.
    public override Status Process()
    {
        //checks if node is complete, if not returns running so tree know that it is still running.
        Status ChildStatus = Children[CurrentChild].Process();
        if (ChildStatus == Status.RUNNING) return Status.RUNNING;
        //if complete check if the node successful, if is it will return success so the tree knows that the selector was successful.
        if (ChildStatus == Status.SUCCESS)
        {
            CurrentChild = 0;
            return Status.SUCCESS;
        }

        //if node was not seccessful then it will move on to the next child.
        CurrentChild++;
        //check if it has hit the end of the selector's children.
        if (CurrentChild >= Children.Count)
        {
            //if at end of selector's children it will reset the start point back to 0 and return the selector as failing.
            CurrentChild = 0;
            return Status.FAILURE;
        }

        //if not at end, it returns that the selector is still running.
        return Status.RUNNING;
    }

}
