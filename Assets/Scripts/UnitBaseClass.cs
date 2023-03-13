using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBaseClass : MonoBehaviour
{
    public int Initiative = 0;
    public bool PlayerUnit = false;
    public bool Moved = false;
    protected int MovementPoints = 4;
    // Start is called before the first frame update

    public virtual void UpdateLoop() { }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
