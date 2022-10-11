using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class OLD_Unit : MonoBehaviour, IManager
{
    protected Rigidbody _rb;
    protected Coroutine lastPathCoroutine = null;
    protected Coroutine lastMoveCoroutine = null;
    protected Coroutine lastAttackCoroutine = null;
    protected ISteering _sb;
    protected StateMachine<string> _fsm;

    public float contactDistance = 4f;
    public float speed;
    public float hp;
    public float damage;
    public float attackSpeed = 1f; 

    public List<Node> targetPath = new List<Node>();
    public List<Vector3> targetPathV = new List<Vector3>();

    public GameObject target;
    public Vector3 targetLastKnowPos;
    public int currentNode = 0;

    public bool moving = false;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        GameManager.Instance.AddItem(this);

        //IdleState<string> idleState = new IdleState<string>(this);
        //MoveState<string> moveState = new MoveState<string>(this);
        //AttackState<string> attackState = new AttackState<string>(this);

        //idleState.AddTransition("Move", moveState);

        //moveState.AddTransition("Attack", attackState);

       // attackState.AddTransition("Move", moveState);

        _fsm = new StateMachine<string>();
        //_fsm.SetInitState(idleState);
    }

    private void Update()
    {
        _fsm.OnUpdate();
    }

    public void GoToIdle(float time)
    {
        StartCoroutine(SetFSMState("Idle", time));
    }

    public void GoToMove(float time)
    {
        StartCoroutine(SetFSMState("Move", time));
    }

    public void GoToAttack(float time)
    {
        StartCoroutine(SetFSMState("Attack", time));
    }

    private IEnumerator SetFSMState(string state, float time)
    {
        yield return new WaitForSeconds(time);
        _fsm.Transition(state);
    }

    public virtual void Pause(bool paused)
    {
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = paused;
    }

    public virtual void Move()
    {
        Vector3 dir;
        if (_sb != null) dir = _sb.GetDirection();
        else dir = Vector3.zero;

        dir.y = 0;
        _rb.velocity = dir.normalized * speed;
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

        final.Add(target.transform.position);

        return final;
    }

    public Vector3 IgnoreY(Vector3 v)
    {
        Vector3 r = new Vector3(v.x, 0f, v.z);
        return r;
    }

    public void AIUpdatePath()
    {
        targetPath = PathfindingManager.Instance.GetPath(transform, target.transform.position);
    }

    public void AIConvertPath()
    {
        targetPathV = ConvertPathToVector(targetPath);
        targetLastKnowPos = targetPathV[targetPathV.Count - 1];
        SetSteeringBehaviour(new Seek(targetPathV[1], transform, 0.1f));
        currentNode = 1;
    }

    public IEnumerator UpdatePath()
    {
        if (!target.transform.position.Equals(targetLastKnowPos))
        {
            //AIManager.Instance.AddCalculation(this, AIUpdatePath);
            //AIManager.Instance.AddCalculation(this, AIConvertPath);
        }
        yield return new WaitForSeconds(GameManager.Instance.updateInterval);
        lastPathCoroutine = StartCoroutine(UpdatePath());
    }

    public IEnumerator UpdateMove()
    {
        Debug.Log("UpdateMove");
        //AIManager.Instance.AddCalculationFixed(this, Move);
        yield return new WaitForSeconds(GameManager.Instance.updateFixedInterval);
        lastMoveCoroutine = StartCoroutine(UpdateMove());
    }

    public void SetSteeringBehaviour(ISteering sb)
    {
        _sb = sb;
    }

    public void MoveCoroutine(bool b)
    {
        if (b)
        {
            lastMoveCoroutine = StartCoroutine("UpdateMove");
            moving = true;
        }
        else
        {
            StopCoroutine(lastMoveCoroutine);
            moving = false;
        }
    }

    public void AttackCoroutine(bool b)
    {
        if (b)
        {
            lastAttackCoroutine = StartCoroutine("DamageCoroutine");
            moving = true;
        }
        else
        {
            StopCoroutine(lastAttackCoroutine);
            moving = false;
        }
    }

    public IEnumerator DamageCoroutine()
    {
        Debug.Log("Attack " + target.name);
        //AIManager.Instance.AddDamage(this, DamageTarget);
        yield return new WaitForSeconds(attackSpeed);
        lastAttackCoroutine = StartCoroutine(DamageCoroutine());
    }

    public void DamageTarget()
    {
        //if (target.gameObject.activeSelf) target.GetComponent<Unit>().GetDamaged(damage);
    }

    public void GetDamaged(float damage)
    {
        hp -= damage;
        if (hp <= 0) Die();
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public bool IsAttacking()
    {
        if (lastAttackCoroutine != null) return true;
        return false;
    }

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
