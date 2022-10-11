using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Unit : MonoBehaviour, IManager
{
    public int team;

    public GameObject[] helmetAndFlag;

    protected Rigidbody _rb;
    protected Coroutine _lastPathCoroutine = null;
    protected Coroutine _lastMoveCoroutine = null;
    protected Coroutine _lastAttackCoroutine = null;
    protected ISteering _sb;
    protected StateMachine<string> _fsm;
    protected IdleState<string> _idleState;
    protected DeadState<string> _deadState;
    public string currentState;

    public float contactRange = 2f;
    public Unit attackTarget;
    public float attackRange = 3f;

    public float totalHp = 0.1f;
    public float attackSpeed = 100f;
    public float damage = 0f;

    public bool started = false;

    public bool died = false;

    #region BasicFuncs

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        GameManager.Instance.AddItem(this);

        _idleState = new IdleState<string>(this);
        _deadState = new DeadState<string>(this);
        _fsm = new StateMachine<string>();
        _fsm.SetInitState(_idleState);
    }

    protected virtual void Update()
    {
        _fsm.OnUpdate();
    }

    #endregion

    #region FSM

    public void GoToState(string stateName, float time)
    {
        StartCoroutine(SetFSMState(stateName, time));
    }

    private IEnumerator SetFSMState(string state, float time)
    {
        yield return new WaitForSeconds(time);
        _fsm.Transition(state);
    }

    #endregion

    #region IManager

    public virtual void Pause(bool paused)
    {
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = paused;
    }

    #endregion

    #region Movement

    public abstract void Move();

    protected Vector3 GetDirection()
    {
        Vector3 dir;
        if (_sb != null) dir = _sb.GetDirection();
        else dir = Vector3.zero;
        dir.y = 0;
        return dir;
    }

    public void MoveCoroutineControl(bool b)
    {
        if (b)
        {
            if (_lastMoveCoroutine == null) _lastMoveCoroutine = StartCoroutine(MoveCoroutine());
        }
        else
        {
            if (_lastMoveCoroutine != null)
            {
                StopCoroutine(_lastMoveCoroutine);
                _lastMoveCoroutine = null;
            }
        }
    }

    public IEnumerator MoveCoroutine()
    {
        AIManager.Instance.AddCalculationFixed(this, Move);
        yield return new WaitForSeconds(GameManager.Instance.updateFixedInterval);
        _lastMoveCoroutine = StartCoroutine(MoveCoroutine());
    }

    public bool IsMoving()
    {
        if (_lastMoveCoroutine != null) return true;
        return false;
    }

    public void SetSteeringBehaviour(ISteering sb)
    {
        _sb = sb;
    }

    public Vector3 IgnoreY(Vector3 v)
    {
        Vector3 r = new Vector3(v.x, transform.position.y, v.z);
        return r;
    }

    public void StopMovement()
    {
        _rb.velocity = Vector3.zero;
    }

    #endregion

    #region Combat

    public void GetAttackTarget()
    {
        Collider[] closeUnits = Physics.OverlapSphere(transform.position, attackRange, ~15);
        List<Unit> closeUnitsList = new List<Unit>();
        foreach (Collider c in closeUnits)
        {
            Unit aux;
            if (c.gameObject.TryGetComponent<Unit>(out aux) && IsEnemy(aux) && IsVisible(aux)) closeUnitsList.Add(aux);
        }
        if (closeUnitsList.Count != 0)
        {
            int r = Random.Range(0, closeUnitsList.Count);
            attackTarget = closeUnitsList[r];
        }
    }

    public bool IsEnemy(Unit u)
    {
        if (u.team != team) return true;
        return false;
    }

    public bool IsVisible(Unit u)
    {
        RaycastHit hit;
        bool result = Physics.Raycast(transform.position, u.transform.position - transform.position, out hit, Vector3.Distance(transform.position, u.transform.position), ~15);
        if (result && hit.collider.gameObject.Equals(u.gameObject))
        {
            Debug.DrawRay(transform.position, u.transform.position - transform.position, Color.green);
            return true;
        }
        return false;
    }

    public void AttackTarget()
    {
        if (attackTarget != null && attackTarget.gameObject.activeSelf) attackTarget.GetAttacked(damage);
    }

    public void AttackCoroutineControl(bool b)
    {
        if (b)
        {
            if (_lastAttackCoroutine == null) _lastAttackCoroutine = StartCoroutine(AttackCoroutine());
        } else
        {
            if (_lastAttackCoroutine != null)
            {
                StopCoroutine(_lastAttackCoroutine);
                _lastAttackCoroutine = null;
            }
        }
    }

    public IEnumerator AttackCoroutine()
    {
        Debug.Log("atk");
        AIManager.Instance.AddAttack(this, AttackTarget);
        yield return new WaitForSeconds(attackSpeed);
        _lastAttackCoroutine = StartCoroutine(AttackCoroutine());
    }

    public bool IsAttacking()
    {
        if (_lastAttackCoroutine != null) return true;
        return false;
    }

    public virtual void GetAttacked(float damage)
    {
        totalHp -= damage;
        if (totalHp <= 0)
        {
            Die();
            return;
        }
    }

    public void Die()
    {
        GoToState("Die", 0f);
    }

    #endregion
}
