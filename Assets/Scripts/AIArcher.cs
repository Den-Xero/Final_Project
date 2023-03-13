using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AIArcher : UnitBaseClass
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        Initiative = 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateLoop()
    {
        print("AI Archer Takes turn");
        Moved = true;
        GameManager.Main.EndTurn();
    }

}
