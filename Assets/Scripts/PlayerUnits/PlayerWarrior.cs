using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarrior : UnitBaseClass
{
    private void Awake()
    {
        Initiative = 3;
        PlayerUnit = true;
        MovementPoints = 4;
        AttackRange = 2;
    }

}
