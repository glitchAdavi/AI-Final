using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierState_Flee<T> : State<T>
{
    Soldier _soldier;

    public SoldierState_Flee(Soldier soldier)
    {
        _soldier = soldier;
    }

    public override void Enter()
    {
        _soldier.currentState = "Flee";
        _soldier.StopAllCoroutines();
        _soldier.MoveCoroutineControl(true);
        _soldier.GoToState("Die", 3f);

        foreach(GameObject go in _soldier.helmetAndFlag)
        {
            go.GetComponent<MeshRenderer>().material = GameManager.Instance.teamGrey;
        }
    }

    public override void Tick()
    {
        _soldier.SetSteeringBehaviour(new Flee(_soldier.enemyLeader.GetTeamCenterPoint(), _soldier.transform));
    }

    public override void Exit()
    {
        _soldier.SetSteeringBehaviour(null);
    }
}
