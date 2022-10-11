using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderState_Combat<T> : State<T>
{
    Leader _leader;

    public LeaderState_Combat(Leader leader)
    {
        _leader = leader;
    }

    public override void Enter()
    {
        _leader.currentState = "Combat (Leader)";
        _leader.GetAttackTarget();
    }

    public override void Tick()
    {
        if (_leader.attackTarget != null && !_leader.attackTarget.died)
        {
            if (!_leader.IsAttacking()) _leader.AttackCoroutineControl(true);
        }

        if (_leader.targetLeader.died)
        {
            _leader.PathCoroutineControl(false);
            GameManager.Instance.GetTarget(_leader);
            _leader.PathCoroutineControl(true);
            _leader.GoToState("Move", 0f);
        }

        if (_leader.attackTarget != null && _leader.attackTarget.died)
        {
            _leader.SetSteeringBehaviour(null);
            _leader.GetAttackTarget();
        }

        if (_leader.attackTarget == null)
        {
            _leader.GoToState("Move", 0f);
        }

        if (!_leader.IsAttacking()) _leader.AttackCoroutineControl(true);

        Vector3 _from = _leader.IgnoreY(_leader.transform.position);

        if (_leader.attackTarget != null && !_leader.attackTarget.died && Vector3.Distance(_from, _leader.attackTarget.transform.position) > 0.75f)
        {
            if (!_leader.IsMoving())
            {
                _leader.SetSteeringBehaviour(new Seek(_leader.attackTarget.transform, _leader.transform, 0.75f));
                _leader.MoveCoroutineControl(true);
            }
        } else
        {
            if (_leader.IsMoving()) _leader.MoveCoroutineControl(false);
        }
    }

    public override void Exit()
    {
        _leader.attackTarget = null;
    }
}
