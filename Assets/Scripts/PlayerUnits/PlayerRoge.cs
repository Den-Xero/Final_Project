using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoge : UnitBaseClass
{
    private void Awake()
    {
        Initiative = 7;
        PlayerUnit = true;
        MovementPoints = 8;
        AttackRange = 1;
    }

}
