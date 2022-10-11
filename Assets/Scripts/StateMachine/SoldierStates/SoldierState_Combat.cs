using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierState_Combat<T> : State<T>
{
    Soldier _soldier;

    public SoldierState_Combat(Soldier soldier)
    {
        _soldier = soldier;
    }

    public override void Enter()
    {
        _soldier.currentState = "Combat (Soldier)";
    }

    public override void Tick()
    {
        if (_soldier.attackTarget == null || _soldier.attackTarget.died)
        {
            _soldier.AttackCoroutineControl(false);
            _soldier.GetAttackTarget();
            if (_soldier.attackTarget != null) _soldier.AttackCoroutineControl(true);
        }

        if (_soldier.attackTarget != null && Vector3.Distance(_soldier.attackTarget.transform.position, _soldier.transform.position) > 0.6f)
        {
            _soldier.SetSteeringBehaviour(new Seek(_soldier.attackTarget.transform, _soldier.transform, 0.6f));
        } else
        {
            _soldier.SetSteeringBehaviour(null);
            _soldier.MoveCoroutineControl(false);
        }

        if (Vector3.Distance(_soldier.transform.position, _soldier.leader.transform.position) > 8f)
        {
            _soldier.GoToState("Move", 0f);
        }
    }
}
