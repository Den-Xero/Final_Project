using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Struct to make the tree printed out easier to read.
struct NodeTier
{
    public int Tier;
    public TreeNodes sNode;
}


public class TreeRoot : TreeNodes
{
    //Constructor with base name.
    public TreeRoot()
    {
        Name = "Root";
    }

    //Constructor with custom name.
    public TreeRoot(string n)
    {
        Name = n;
    }

    //Keep track of the tree nodes progress to see if node has successed, failed or still running.
    public override Status Process()
    {
        return Children[CurrentChild].Process();
    }


    //Print tree for debugging.
    public void PrintTree()
    {
        string TreeLayout = "";
        Stack<NodeTier> NodeStack = new Stack<NodeTier>();
        TreeNodes CurrentNode = this;
        NodeStack.Push(new NodeTier { Tier = 0, sNode = CurrentNode } );

        while (NodeStack.Count != 0)
        {
            NodeTier NextNode = NodeStack.Pop();
            TreeLayout += new string('\t', NextNode.Tier) + NextNode.sNode.Name + "\n";
            for(int i = NextNode.sNode.Children.Count - 1; i >= 0; i--)
            {
                NodeStack.Push(new NodeTier { Tier = NextNode.Tier + 1, sNode = NextNode.sNode.Children[i] } );
            }
        }

        Debug.Log(TreeLayout);
    }
}
