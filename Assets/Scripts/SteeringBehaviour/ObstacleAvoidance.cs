using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : ISteering
{
    Transform _unit;
    float _radius;
    LayerMask _maskLayer;

    public ObstacleAvoidance(Transform unit, float radius, LayerMask maskLayer)
    {
        _unit = unit;
        _radius = radius;
        _maskLayer = maskLayer;
    }

    public Vector3 GetDirection()
    {
        Collider[] obstacles = Physics.OverlapSphere(_unit.transform.position, _radius, _maskLayer);
        float currentDistance = _radius;
        int closerObstacleIndex = 0;
        if (obstacles.Length > 0)
        {
            Debug.Log(obstacles[0].gameObject.layer);
            for (int i = 0; i < obstacles.Length; i++)
            {
                var distanceToObstacle = Vector3.Distance(_unit.transform.position, obstacles[i].transform.position);
                if (distanceToObstacle < currentDistance)
                {
                    currentDistance = distanceToObstacle;
                    closerObstacleIndex = i;
                }
            }
            Vector3 dir = (_unit.transform.position - obstacles[closerObstacleIndex].transform.position).normalized;
            float angle = Vector3.Angle(dir, _unit.transform.forward);
            Debug.Log(angle);
            if (angle <= 45/2)
            {
                dir = Vector3.Cross(dir, _unit.transform.up);
            }
            return dir;
        }
        else return Vector3.zero;
    }
}
