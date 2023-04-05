using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RogueAIBehaviour : MonoBehaviour
{
    
    bool ConditionFulfilled = true;
    TreeRoot mRoot;
    public enum ActionState { IDLE, WORKING};
    ActionState mState = ActionState.IDLE;

    TreeNodes.Status mTreeStatus = TreeNodes.Status.RUNNING;

    // Start is called before the first frame update
    void Start()
    {

        //make the root node of the entity behaviour tree.
        mRoot = new TreeRoot();
        //Sequence node setup with a condition leaf for refence.
        TreeSequence Sequence = new TreeSequence("Sequence");
        TreeLeaf ConditionLeaf = new TreeLeaf("ConditionLeaf", FConditionLeaf);
        TreeLeaf Action1 = new TreeLeaf("Action1", FAction1);
        TreeLeaf Action2 = new TreeLeaf("Action2", FAction2);

        //adding leafs to sequence as children including a condition one then adding the sequece to the root.
        Sequence.AddChild(ConditionLeaf);
        Sequence.AddChild(Action1);
        Sequence.AddChild(Action2);
        mRoot.AddChild(Sequence);

        //Selector node setup.
        TreeSelector Selector = new TreeSelector("Selector");
        TreeLeaf Action3 = new TreeLeaf("Action3", FAction3);
        TreeLeaf Action4 = new TreeLeaf("Action4", FAction4);

        //adding leafs to sequence as children then adding the sequece to the root.
        Selector.AddChild(Action3);
        Selector.AddChild(Action4);
        mRoot.AddChild(Selector);

        mRoot.PrintTree();
        
    }

    //Condition leaf.
    public TreeNodes.Status FConditionLeaf()
    {
        if(ConditionFulfilled) return TreeNodes.Status.SUCCESS;
        return TreeNodes.Status.FAILURE;
    }

    //the Actions leafs.
    public TreeNodes.Status FAction1()
    {
        //return Movement(GameObject1.transform.position);
        return TreeNodes.Status.SUCCESS;
    }

    public TreeNodes.Status FAction2()
    {
        //return Movement(GameObject2.transform.position);
        return TreeNodes.Status.SUCCESS;
    }

    public TreeNodes.Status FAction3()
    {
        //Movement(GameObject3.transform.position);
        //make a if for if move is seccessful but next action can no be completed.
        return TreeNodes.Status.SUCCESS;
    }

    public TreeNodes.Status FAction4()
    {
        //Movement(GameObject4.transform.position);
        //make a if for if move is seccessful but next action can no be completed.
        return TreeNodes.Status.SUCCESS;
    }

    //Movement action to be called by action leafs.
    TreeNodes.Status Movement(Vector3 destination)
    {
        //float DistanceToTarget = Vector3.Distance(destination, this.transform.position);
        //if(mState == ActionState.IDLE)
        //{
        //    mAgent.SetDestination(destination);
        //    mState = ActionState.WORKING;
        //}
        //else if(Vector3.Distance(mAgent.pathEndPosition, destination) >= 2)
        //{
        //    mState = ActionState.IDLE;
        //    return TreeNodes.Status.FAILURE;
        //}
        //else if(DistanceToTarget < 2)
        //{
        //    mState = ActionState.IDLE;
        //    return TreeNodes.Status.SUCCESS;
        //}
        return TreeNodes.Status.RUNNING;
    }


    // Update is called once per frame
    void Update()
    {
        //will run tree if the tree status is no success full will need to adapted this so it is the right one for the right entity behavior.
        if(mTreeStatus != TreeNodes.Status.SUCCESS)
        {
            mTreeStatus = mRoot.Process();
        }
    }
}
