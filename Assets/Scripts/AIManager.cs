using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private static AIManager _AIManager;
    public static AIManager Instance { get { return _AIManager; } }

    public delegate void AIFunction();

    List<Tuple<Unit, AIFunction>> pendingAIFunctions = new List<Tuple<Unit, AIFunction>>();
    List<Tuple<Unit, AIFunction>> pendingAIFunctionsFixed = new List<Tuple<Unit, AIFunction>>();
    List<Tuple<Unit, AIFunction>> pendingAIAttackFunctions = new List<Tuple<Unit, AIFunction>>();

    public int execPerFrame = 1;
    public int damageExecPerFrame = 3;
    public int remainingPending = 0;
    public int remainingPendingFixed = 0;
    public int remainingAttackPending = 0;

    private void Awake()
    {
        if (_AIManager == null) _AIManager = this;
        else Destroy(this);
    }

    private void Update()
    {
        remainingPending = pendingAIFunctions.Count;
        if (pendingAIFunctions.Count > 0)
        {
            for (int i = 0; i < pendingAIFunctions.Count && i < execPerFrame; i++)
            {
                if (pendingAIFunctions[0].Item1.gameObject.activeSelf)
                {
                    if (!GameManager.Instance.paused) pendingAIFunctions[0].Item2();
                    pendingAIFunctions.RemoveAt(0);
                }
            }
        }

        remainingAttackPending = pendingAIAttackFunctions.Count;
        if (pendingAIAttackFunctions.Count > 0)
        {
            for (int i = 0; i < pendingAIAttackFunctions.Count && i < damageExecPerFrame; i++)
            {
                if (pendingAIAttackFunctions[0].Item1.gameObject.activeSelf)
                {
                    if (!GameManager.Instance.paused) pendingAIAttackFunctions[0].Item2();
                    pendingAIAttackFunctions.RemoveAt(0);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        remainingPendingFixed = pendingAIFunctionsFixed.Count;
        if (pendingAIFunctionsFixed.Count > 0)
        {
            for (int i = 0; i < pendingAIFunctionsFixed.Count && i < execPerFrame; i++)
            {
                if (pendingAIFunctionsFixed[0].Item1.gameObject.activeSelf)
                {
                    if (!GameManager.Instance.paused) pendingAIFunctionsFixed[0].Item2();
                    pendingAIFunctionsFixed.RemoveAt(0);
                }
            }
        }
    }

    public void AddCalculation(Unit unit, AIFunction func)
    {
        pendingAIFunctions.Add(new Tuple<Unit, AIFunction>(unit, func));
    }

    public void AddCalculationFixed(Unit unit, AIFunction func)
    {
        pendingAIFunctionsFixed.Add(new Tuple<Unit, AIFunction>(unit, func));
    }

    public void AddAttack(Unit unit, AIFunction func)
    {
        pendingAIAttackFunctions.Add(new Tuple<Unit, AIFunction>(unit, func));
    }
}
