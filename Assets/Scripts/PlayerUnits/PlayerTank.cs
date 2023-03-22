using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : UnitBaseClass
{
    private void Awake()
    {
        Initiative = 1;
        PlayerUnit = true;
        MovementPoints = 2;
        AttackRange = 1;
    }



}
