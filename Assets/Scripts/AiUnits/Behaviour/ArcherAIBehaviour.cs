using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArcherAIBehaviour : TreeActions
{
    
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
        TreeLeaf FindTarget = new TreeLeaf("Find Target", FFindClosestTarget);
        TreeSelector IsTargetTooClose = new TreeSelector("Is Target Too Close");
        TreeSequence TargetTooClose = new TreeSequence("Target To Close");
        TreeLeaf DistanceToTarget = new TreeLeaf("Distance To Target", FDistanceToTarget);
        TreeLeaf MoveAwayFromTarget = new TreeLeaf("Move Away From Target", FMoveAwayFromTarget);
        TreeSequence Attack = new TreeSequence("Attack");
        TreeLeaf CanAttackWithEndTurn = new TreeLeaf("Can Attack With End Turn ", FCanAttackWithEndTurn);
        TreeLeaf AttackAndSetEndTurn = new TreeLeaf("Attack And Set End Turn", FAttackAndSetEndTurn);
        TreeSelector CloseEnoughToAttack = new TreeSelector("Close Enough To Attack");
        TreeSequence AttackWithSkipMovement = new TreeSequence("Attack With Skip Movement");
        TreeLeaf SetAsMoved = new TreeLeaf("Set As Moved", FSetAsMoved);
        TreeSelector MoveTowardsTarget = new TreeSelector("Move Towards Target");
        TreeSequence MoveToTarget = new TreeSequence("Move To Target");
        TreeLeaf FindGoodHexToMoveTo = new TreeLeaf("FindGoodHexToMoveTo", FMoveSoCanAttack);
        TreeLeaf Move = new TreeLeaf("Move", FMoveToAttack);
        TreeSequence FindGoodMaxRangeMovement = new TreeSequence("Find Good Max Range Movement");
        TreeLeaf WorkBackFromTargetToFindHex = new TreeLeaf("Work Back From Target To Find Hex", FWorkBackFromTargetToFindHex);
        TreeSelector CanUnitAttackAnyPlayerUnit = new TreeSelector("Can Unit Attack Any Player Unit");
        TreeSequence AttackAnyPlayerUnit = new TreeSequence("Attack Any Player Unit");
        TreeLeaf CanAttackWithoutEndTurn = new TreeLeaf("Can Attack Without End Turn ", FCanAttackWithoutEndTurn);
        TreeLeaf ReadyEndTurn = new TreeLeaf("Ready End Turn", FReadyEndTurn);

        //Making the tree with adding all the nodes under one a child of the node, the numbers show what level the node is on so 0 being connected to root level with a node bing 3 needing to move up 3 nodes to get to the root level.
        /* 1 */GetNearestPlayerUnit.AddChild(FindTarget);
        /* 1 */GetNearestPlayerUnit.AddChild(IsTargetTooClose);
        /* 2 */IsTargetTooClose.AddChild(TargetTooClose);
        /* 3 */TargetTooClose.AddChild(DistanceToTarget);
        /* 3 */TargetTooClose.AddChild(MoveAwayFromTarget);
        /* 3 */TargetTooClose.AddChild(Attack);
        /* 4 */Attack.AddChild(CanAttackWithEndTurn);
        /* 4 */Attack.AddChild(AttackAndSetEndTurn);
        /* 2 */IsTargetTooClose.AddChild(CloseEnoughToAttack);
        /* 3 */CloseEnoughToAttack.AddChild(AttackWithSkipMovement);
        /* 4 */AttackWithSkipMovement.AddChild(CanAttackWithoutEndTurn);
        /* 4 */AttackWithSkipMovement.AddChild(SetAsMoved);
        /* 4 */AttackWithSkipMovement.AddChild(AttackAndSetEndTurn);
        /* 3 */CloseEnoughToAttack.AddChild(MoveTowardsTarget);
        /* 4 */MoveTowardsTarget.AddChild(MoveToTarget);
        /* 5 */MoveToTarget.AddChild(FindGoodHexToMoveTo);
        /* 5 */MoveToTarget.AddChild(Move);
        /* 5 */MoveToTarget.AddChild(CanAttackWithEndTurn);
        /* 5 */MoveToTarget.AddChild(AttackAndSetEndTurn);
        /* 4 */MoveTowardsTarget.AddChild(FindGoodMaxRangeMovement);
        /* 5 */FindGoodMaxRangeMovement.AddChild(WorkBackFromTargetToFindHex);
        /* 5 */FindGoodMaxRangeMovement.AddChild(CanUnitAttackAnyPlayerUnit);
        /* 6 */CanUnitAttackAnyPlayerUnit.AddChild(AttackAnyPlayerUnit);
        /* 7 */AttackAnyPlayerUnit.AddChild(CanAttackWithoutEndTurn);
        /* 7 */AttackAnyPlayerUnit.AddChild(AttackAndSetEndTurn);
        /* 6 */CanUnitAttackAnyPlayerUnit.AddChild(ReadyEndTurn);
        /* 0 */mRoot.AddChild(GetNearestPlayerUnit);

        
        
    }


    public void TreeUpdate()
    {
        mRoot.Process();
    }
}
