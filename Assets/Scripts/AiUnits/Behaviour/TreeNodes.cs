using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNodes
{
   //Holds the status of the nodes. 
    public enum Status { SUCCESS, RUNNING, FAILURE};
    public Status mStatus;

    //List to store the children of the node, hold the current child it is on and the name of node.
    public List<TreeNodes> Children = new List<TreeNodes>();
    public int CurrentChild = 0;
    public string Name;

    //Blank constructor.
    public TreeNodes() { }

    //Constructor with name.
    public TreeNodes(string n)
    {
        Name = n;
    }

    //Adds children to the list
    public void AddChild(TreeNodes n)
    {
        Children.Add(n);
    }

    //Keep track of the tree nodes progress to see if node has successed, failed or still running.
    public virtual Status Process()
    {
        return Children[CurrentChild].Process();
    }

}
