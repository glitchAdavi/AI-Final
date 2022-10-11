using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState<T> : State<T>
{
    Unit _unit;

    public DeadState(Unit unit)
    {
        _unit = unit;
    }

    public override void Enter()
    {
        _unit.currentState = "Dead";

        _unit.died = true;
        _unit.StopAllCoroutines();
        _unit.gameObject.SetActive(false);
    }

    public override void Tick()
    {
        
    }
}
