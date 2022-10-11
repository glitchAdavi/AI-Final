using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
    public Leader leader;
    public Leader enemyLeader;
    public Flocking flocking;
    public ISteering oa;
    public float oaRadius;
    public float oaWeight;

    protected override void Awake()
    {
        base.Awake();
        totalHp = FlyWeightPointer.Soldier.hp;
        damage = FlyWeightPointer.Soldier.damage;
        attackSpeed = FlyWeightPointer.Soldier.atkSpeed;
    }

    protected override void Start()
    {
        base.Start();

        SoldierState_Move<string> moveState = new SoldierState_Move<string>(this);
        SoldierState_Combat<string> combatState = new SoldierState_Combat<string>(this);
        SoldierState_Flee<string> fleeState = new SoldierState_Flee<string>(this);

        _idleState.AddTransition("Move", moveState);
        _idleState.AddTransition("Combat", combatState);
        _idleState.AddTransition("Flee", fleeState);
        _idleState.AddTransition("Die", _deadState);

        moveState.AddTransition("Idle", _idleState);
        moveState.AddTransition("Combat", combatState);
        moveState.AddTransition("Flee", fleeState);
        moveState.AddTransition("Die", _deadState);

        combatState.AddTransition("Idle", _idleState);
        combatState.AddTransition("Move", moveState);
        combatState.AddTransition("Flee", fleeState);
        combatState.AddTransition("Die", _deadState);

        fleeState.AddTransition("Idle", _idleState);
        fleeState.AddTransition("Die", _deadState);

        flocking.leaderTransform = leader.transform;
        enemyLeader = leader.targetLeader;
        oa = new ObstacleAvoidance(transform, oaRadius, 1 << 10);
    }

    protected override void Update()
    {
        base.Update();
        if (!leader.gameObject.activeSelf) GoToState("Flee", 0f);
    }

    public override void Move()
    {
        Vector3 dir = Vector3.zero;
        if (_sb != null) dir = _sb.GetDirection() + oa.GetDirection() * oaWeight;
        else dir = flocking.GetDirection() + oa.GetDirection() * oaWeight;
        _rb.velocity = dir.normalized * FlyWeightPointer.Soldier.speed;
        transform.forward = Vector3.Lerp(transform.forward, dir, 0.1f);
    }

    public override void GetAttacked(float damage)
    {
        base.GetAttacked(damage);
        if (totalHp < FlyWeightPointer.Soldier.hp / 3)
        {
            float r = Random.Range(0f, 100f);
            if (r < GameManager.Instance.soldierFleeChance)
            {
                died = true;
                GoToState("Flee", 0f);
            }
        }
    }
}
