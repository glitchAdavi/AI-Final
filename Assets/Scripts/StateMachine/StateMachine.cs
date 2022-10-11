using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    State<T> _currentState;

    public void SetInitState(State<T> initialState)
    {
        _currentState = initialState;
        _currentState.Enter();
    }

    public void OnUpdate()
    {
        _currentState.Tick();
    }

    public void Transition(T input)
    {
        State<T> nextState = _currentState.GetState(input);
        if (nextState == null) return;
        _currentState.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }
}
