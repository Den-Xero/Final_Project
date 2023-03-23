using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarrior : UnitBaseClass
{
    private void Awake()
    {
        WarriorSetUp();
        PlayerUnit = true;
    }

}
