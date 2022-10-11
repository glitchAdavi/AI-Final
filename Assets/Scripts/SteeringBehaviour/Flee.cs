using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : ISteering
{
    Transform _target;
    Vector3 _targetVector;
    Transform _transform;

    public Flee(Transform to, Transform from)
    {
        _target = to;
        _transform = from;
    }

    public Flee(Vector3 to, Transform from)
    {
        _targetVector = to;
        _transform = from;
    }

    public Vector3 GetDirection()
    {
        if (_target != null) return GetDirectionTransform();
        else return GetDirectionVector();
    }

    private Vector3 GetDirectionTransform()
    {
        Vector3 direction = (_target.position - _transform.position).normalized;
        return -direction;
    }

    private Vector3 GetDirectionVector()
    {
        Vector3 direction = (_targetVector - _transform.position).normalized;
        return -direction;
    }
}
