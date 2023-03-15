using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBaseClass : MonoBehaviour
{
    public int Initiative = 0;
    protected int AttackRange = 0;
    protected int Health = 100;
    public bool PlayerUnit = false;
    public bool Moved = false;
    public bool Action = false;
    public bool Attacking = false;
    protected int MovementPoints = 4;
    protected int MovementPointsUsed;
    public Vector2Int Pos;
    protected float Speed = 2f;
    protected float RotateSpeed = 4f;
    public UnitBaseClass AttackTarget;
    // Start is called before the first frame update

    public virtual void UpdateLoop() { }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}
