using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T>
{
    Dictionary<T, State<T>> _transitionsDictionary = new Dictionary<T, State<T>>();

    #region Transition Management
    public void AddTransition(T key, State<T> state)
    {
        if (!_transitionsDictionary.ContainsKey(key)) _transitionsDictionary.Add(key, state);
    }

    public void RemoveTransition(T key)
    {
        if (_transitionsDictionary.ContainsKey(key)) _transitionsDictionary.Remove(key);
    }

    public State<T> GetState(T key)
    {
        if (_transitionsDictionary.ContainsKey(key)) return _transitionsDictionary[key];
        return null;
    }
    #endregion

    public virtual void Enter()
    {

    }

    public virtual void Tick()
    {

    }

    public virtual void Exit()
    {

    }
}
