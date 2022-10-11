using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierState_Move<T> : State<T>
{
    Soldier _soldier;

    public SoldierState_Move(Soldier soldier)
    {
        _soldier = soldier;
    }

    public override void Enter()
    {
        _soldier.currentState = "Move (Soldier)";
    }

    public override void Tick()
    {
        if (!_soldier.IsMoving()) _soldier.MoveCoroutineControl(true);
        if (_soldier.leader.IsAttacking()) _soldier.GoToState("Combat", 0.75f);
    }
}
