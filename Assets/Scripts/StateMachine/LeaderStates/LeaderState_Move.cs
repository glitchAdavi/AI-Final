using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderState_Move<T> : State<T>
{
    Leader _leader;

    public LeaderState_Move(Leader leader)
    {
        _leader = leader;
    }

    public override void Enter()
    {
        _leader.currentState = "Move (Leader)";
    }

    public override void Tick()
    {
        if (_leader.targetLeader.died)
        {
            _leader.MoveCoroutineControl(false);
            _leader.PathCoroutineControl(false);
            GameManager.Instance.GetTarget(_leader);
            _leader.PathCoroutineControl(true);
            _leader.MoveCoroutineControl(true);
        }

        if (!_leader.IsMoving()) _leader.MoveCoroutineControl(true);

        Vector3 _from = _leader.IgnoreY(_leader.transform.position);
        if (_leader.targetPathV.Count > 1 &&
            Vector3.Distance(_from, _leader.IgnoreY(_leader.targetPathV[_leader.currentNode])) < 0.6f &&
            _leader.currentNode < _leader.targetPathV.Count - 1)
        {
            _leader.currentNode++;
            _leader.SetSteeringBehaviour(new Seek(_leader.targetPathV[_leader.currentNode], _leader.transform, 0.1f));
        }
        else
        {
            if (_leader.targetLeader != null)
            {
                if (Vector3.Distance(_from, _leader.IgnoreY(_leader.targetLeader.transform.position)) <= _leader.contactRange)
                {
                    if (_leader.IsMoving()) _leader.MoveCoroutineControl(false);
                    _leader.GoToState("Combat", 0f);
                }
            }
        }
    }
}
