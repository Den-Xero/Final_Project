using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSequence : TreeNodes
{
    //Blank constructor.
    public TreeSequence() { }

    //Constructor with name.
    public TreeSequence(string n)
    {
        Name = n;
    }

    //Runs through all the children of the sequence and check the status of each node.
    public override Status Process()
    {
        //checks if node is complete, if not returns running so tree know that it is still running.
        Status ChildStatus = Children[CurrentChild].Process();
        if(ChildStatus == Status.RUNNING) return Status.RUNNING;
        //if complete check if the node failed, if it did returns that so the tree knows that the sequence has failed.
        if (ChildStatus == Status.FAILURE) return ChildStatus;

        //if node was seccessful then it will move on to the next child.
        CurrentChild++;
        //check if it has hit the end of the sequence's children.
        if (CurrentChild >= Children.Count)
        {
            //if at end of sequence's children it will reset the start point back to 0 and return the sequence as being successful.
            CurrentChild = 0;
            return Status.SUCCESS;
        }

        //if not at end, it returns that the sequence is still running.
        return Status.RUNNING;
    }



}
