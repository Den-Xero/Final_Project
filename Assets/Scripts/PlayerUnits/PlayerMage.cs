using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMage : UnitBaseClass
{
    private void Awake()
    {
        Initiative = 2;
        PlayerUnit = true;
        MovementPoints = 3;
        AttackRange = 4;
    }
}
