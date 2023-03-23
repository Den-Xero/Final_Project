using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AIArcher : UnitBaseClass
{
    
    private void Awake()
    {
        ArcherSetUp();
    }

    public override void UpdateLoop()
    {
        print("AI Archer Takes turn");
        Moved = true;
        GameManager.Main.EndTurn();
    }

}
