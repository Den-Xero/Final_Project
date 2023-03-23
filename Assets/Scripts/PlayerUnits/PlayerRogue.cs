using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRogue : UnitBaseClass
{
    private void Awake()
    {
        RogueSetUp();
        PlayerUnit = true;
    }

}
