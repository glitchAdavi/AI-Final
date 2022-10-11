using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : Unit
{
    public List<Unit> batallion = new List<Unit>();

    public Leader targetLeader;
    public Vector3 targetLastKnownPos;
    public int currentNode = 0;

    public List<Node> targetPath = new List<Node>();
    public List<Vector3> targetPathV = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();
        totalHp = FlyWeightPointer.Leader.hp;
        damage = FlyWeightPointer.Leader.damage;
        attackSpeed = FlyWeightPointer.Leader.atkSpeed;
    }

    protected override void Start()
    {
        base.Start();

        LeaderState_Move<string> moveState = new LeaderState_Move<string>(this);
        LeaderState_Combat<string> combatState = new LeaderState_Combat<string>(this);

        _idleState.AddTransition("Move", moveState);
        _idleState.AddTransition("Combat", combatState);
        _idleState.AddTransition("Die", _deadState);

        moveState.AddTransition("Idle", _idleState);
        moveState.AddTransition("Combat", combatState);
        moveState.AddTransition("Die", _deadState);

        combatState.AddTransition("Idle", _idleState);
        combatState.AddTransition("Move", moveState);
        combatState.AddTransition("Die", _deadState);
    }

    public override void Pause(bool paused)
    {
        base.Pause(paused);
        if (paused)
        {
            if (IsMoving()) MoveCoroutineControl(paused);
            if (IsAttacking()) AttackCoroutineControl(paused);
        }
        PathCoroutineControl(paused);
    }

    public override void Move()
    {
        Vector3 dir = GetDirection();
        _rb.velocity = dir.normalized * FlyWeightPointer.Leader.speed;
        transform.forward = Vector3.Lerp(transform.forward, dir, 0.5f);
    }

    public int GetNextNode(int currentPosition)
    {
        if (currentPosition < targetPathV.Count - 1) return currentPosition + 1;
        else return currentPosition;
    }

    public List<Vector3> ConvertPathToVector(List<Node> path)
    {
        List<Vector3> final = new List<Vector3>();

        final.Add(transform.position);

        for (int i = 1; i < path.Count - 1; i++)
        {
            final.Add(path[i].transform.position);
        }

        final.Add(targetLeader.GetTeamCenterPoint());

        return final;
    }

    public void AIUpdatePath()
    {
        targetPath = PathfindingManager.Instance.GetPath(transform, targetLeader.transform.position);
    }

    public void AIConvertPath()
    {
        targetPathV = ConvertPathToVector(targetPath);
        targetLastKnownPos = targetPathV[targetPathV.Count - 1];
        SetSteeringBehaviour(new Seek(targetPathV[1], transform, 0.1f));
        currentNode = 1;
    }

    public void PathCoroutineControl(bool b)
    {
        if (b)
        {
            if (_lastPathCoroutine == null) _lastPathCoroutine = StartCoroutine(PathCoroutine());
        }
        else
        {
            if (_lastPathCoroutine != null)
            {
                StopCoroutine(_lastPathCoroutine);
                _lastPathCoroutine = null;
            }
        }
    }

    public IEnumerator PathCoroutine()
    {
        if (!targetLeader.transform.position.Equals(targetLastKnownPos))
        {
            AIManager.Instance.AddCalculation(this, AIUpdatePath);
            AIManager.Instance.AddCalculation(this, AIConvertPath);
        }
        yield return new WaitForSeconds(GameManager.Instance.updateInterval);
        _lastPathCoroutine = StartCoroutine(PathCoroutine());
    }

    public Vector3 GetTeamCenterPoint()
    {
        int soldiers = 1;
        Vector3 result = transform.position;
        foreach(Soldier s in batallion)
        {
            if (s.gameObject.activeSelf && !s.died)
            {
                result += s.transform.position;
                soldiers++;
            }
        }

        result /= soldiers;

        return result;
    }

    // DEBUG

    private void OnDrawGizmos()
    {
        foreach (Vector3 node in targetPathV)
        {
            Gizmos.DrawWireSphere(node + new Vector3(0, 1, 0), 0.6f);
        }
        for (int i = 0; i < targetPathV.Count; i++)
        {
            if (i + 1 < targetPathV.Count) Gizmos.DrawLine(targetPathV[i], targetPathV[i + 1]);
        }
    }
}
