using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMage : UnitBaseClass
{
    private void Awake()
    {
        MageSetUp();
        PlayerUnit = true;
    }
}
