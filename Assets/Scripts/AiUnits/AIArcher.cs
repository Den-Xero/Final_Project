using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AIArcher : UnitBaseClass
{
    
    private void Awake()
    {
        Initiative = 6;
    }

    public override void UpdateLoop()
    {
        print("AI Archer Takes turn");
        Moved = true;
        GameManager.Main.EndTurn();
    }

}
