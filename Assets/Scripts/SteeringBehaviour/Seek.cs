using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : ISteering
{
    Transform _target;
    Vector3 _targetVector;
    Transform _transform;
    float _distance;

    public Seek(Transform to, Transform from, float dist)
    {
        _target = to;
        _transform = from;
        _distance = dist;
    }

    public Seek(Vector3 to, Transform from, float dist)
    {
        _targetVector = to;
        _transform = from;
        _distance = dist;
    }

    public Vector3 GetDirection()
    {
        if (_target != null) return GetDirectionTransform();
        else return GetDirectionVector();
    }

    private Vector3 GetDirectionTransform()
    {
        if (Vector3.Distance(_transform.position, _target.position) > _distance)
        {
            Vector3 direction = (_target.position - _transform.position).normalized;
            return direction;
        }
        return Vector3.zero;
    }

    private Vector3 GetDirectionVector()
    {
        if (Vector3.Distance(_transform.position, _targetVector) > _distance)
        {
            Vector3 direction = (_targetVector - _transform.position).normalized;
            return direction;
        }
        return Vector3.zero;
    }
}
