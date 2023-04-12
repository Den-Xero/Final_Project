using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankAIBehaviour: TreeActions
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
        TreeSequence GetNearestPlayerUnit = new TreeSequence("Get Nearest Player Unit");
        TreeLeaf FindTarget = new TreeLeaf("Find Target", FConditionLeaf);
        TreeLeaf CanAttackWithEndTurn = new TreeLeaf("Can Attack With End Turn ", FConditionLeaf);
        TreeLeaf AttackAndSetEndTurn = new TreeLeaf("Attack And Set End Turn", FAction2);
        TreeSelector CloseEnoughToAttack = new TreeSelector("Close Enough To Attack");
        TreeSequence AttackWithSkipMovement = new TreeSequence("Attack With Skip Movement");
        TreeLeaf SetAsMoved = new TreeLeaf("Set As Moved", FAction1);
        TreeSelector MoveTowardsTarget = new TreeSelector("Move Towards Target");
        TreeSequence MoveToTarget = new TreeSequence("Move To Target");
        TreeLeaf FindGoodHexToMoveTo = new TreeLeaf("FindGoodHexToMoveTo", FConditionLeaf);
        TreeLeaf Move = new TreeLeaf("Move", FAction1);
        TreeSequence FindGoodMaxRangeMovement = new TreeSequence("Find Good Max Range Movement");
        TreeLeaf WorkBackFromTargetToFindHex = new TreeLeaf("Work Back From Target To Find Hex", FAction1);
        TreeSelector CanUnitAttackAnyPlayerUnit = new TreeSelector("Can Unit Attack Any Player Unit");
        TreeSequence AttackAnyPlayerUnit = new TreeSequence("Attack Any Player Unit");
        TreeLeaf CanAttackWithoutEndTurn = new TreeLeaf("Can Attack Without End Turn ", FConditionLeaf);
        TreeLeaf ReadyEndTurn = new TreeLeaf("Ready End Turn", FAction1);

        //Making the tree with adding all the nodes under one a child of the node, the numbers show what level the node is on so 0 being connected to root level with a node bing 3 needing to move up 3 nodes to get to the root level.
        /* 1 */GetNearestPlayerUnit.AddChild(FindTarget);
        /* 1 */GetNearestPlayerUnit.AddChild(CloseEnoughToAttack);
        /* 2 */CloseEnoughToAttack.AddChild(AttackWithSkipMovement);
        /* 3 */AttackWithSkipMovement.AddChild(CanAttackWithoutEndTurn);
        /* 3 */AttackWithSkipMovement.AddChild(SetAsMoved);
        /* 3 */AttackWithSkipMovement.AddChild(AttackAndSetEndTurn);
        /* 2 */CloseEnoughToAttack.AddChild(MoveTowardsTarget);
        /* 3 */MoveTowardsTarget.AddChild(MoveToTarget);
        /* 4 */MoveToTarget.AddChild(FindGoodHexToMoveTo);
        /* 4 */MoveToTarget.AddChild(Move);
        /* 4 */MoveToTarget.AddChild(CanAttackWithEndTurn);
        /* 4 */MoveToTarget.AddChild(AttackAndSetEndTurn);
        /* 3 */MoveTowardsTarget.AddChild(FindGoodMaxRangeMovement);
        /* 4 */FindGoodMaxRangeMovement.AddChild(WorkBackFromTargetToFindHex);
        /* 4 */FindGoodMaxRangeMovement.AddChild(CanUnitAttackAnyPlayerUnit);
        /* 5 */CanUnitAttackAnyPlayerUnit.AddChild(AttackAnyPlayerUnit);
        /* 6 */AttackAnyPlayerUnit.AddChild(CanAttackWithoutEndTurn);
        /* 6 */AttackAnyPlayerUnit.AddChild(AttackAndSetEndTurn);
        /* 5 */CanUnitAttackAnyPlayerUnit.AddChild(ReadyEndTurn);
        /* 0 */mRoot.AddChild(GetNearestPlayerUnit);

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
