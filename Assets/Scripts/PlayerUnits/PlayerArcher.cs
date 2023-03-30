using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.XR;

public class PlayerArcher : UnitBaseClass
{

    private void Awake()
    {
        ArcherSetUp();
        PlayerUnit = true;
    }

    public override void UpdateLoop()
    {
        if(Action)return;
        if (Moved) 
        { 
            if(!GameManager.Main.AStar.Done && Attacking)
            {
                Attack();
            }
            return; 
        }
        
        MoveSetUp();

        if(!Moved) Move();

    }


}
