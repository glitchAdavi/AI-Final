using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public List<Unit> batallionBoids;
    public Collider[] nearBoidsCollection;
    public List<Unit> nearBoids;
    public float radius;
    public LayerMask mask;

    public Transform leaderTransform;

    public float globalCohesionWeight;
    public float globalAlignmentWeight;
    public float globalSeparationWeight;

    public float cohesionWeight;
    public float alignmentWeight;
    public float separationWeight;

    public float leaderWeight;

    public Vector3 GetDirection()
    {
        batallionBoids = leaderTransform.gameObject.GetComponent<Leader>().batallion;
        nearBoidsCollection = Physics.OverlapSphere(transform.position, radius, mask);
        nearBoids.Clear();
        foreach (Collider s in nearBoidsCollection)
        {
            nearBoids.Add(s.GetComponent<Unit>());
        }

        Vector3 result = Vector3.zero;

        Vector3 globalCohesion = GetCohesion(batallionBoids, globalCohesionWeight);
        Vector3 globalAlignment = GetAlignment(batallionBoids, globalAlignmentWeight);
        Vector3 globalSeparation = GetSeparation(batallionBoids, globalSeparationWeight);

        Vector3 cohesion = GetCohesion(nearBoids, cohesionWeight);
        Vector3 alignment = GetAlignment(nearBoids, alignmentWeight);
        Vector3 separation = GetSeparation(nearBoids, separationWeight);

        Vector3 leader = GetLeader(leaderTransform.transform.position);

        result = result + leader + globalCohesion + globalAlignment + globalSeparation + cohesion + alignment + separation;
        result = new Vector3(result.x, 0f, result.z);
        return result.normalized;
    }

    public Vector3 GetCohesion(List<Unit> boids, float weight)
    {
        if (boids.Count < 1) return Vector3.zero;

        Vector3 result = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            result += boids[i].transform.position;
        }

        result /= boids.Count;

        return result.normalized * weight;
    }

    public Vector3 GetAlignment(List<Unit> boids, float weight)
    {
        if (boids.Count < 1) return Vector3.zero;

        Vector3 result = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            result += boids[i].transform.forward;
        }

        result /= boids.Count;

        return result.normalized * weight;
    }

    public Vector3 GetSeparation(List<Unit> boids, float weight)
    {
        if (boids.Count < 1) return Vector3.zero;

        Vector3 result = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            Vector3 direction = (transform.position - boids[i].transform.position);
            float distance = direction.magnitude;
            direction = direction.normalized * (radius - distance);
            result += direction;
        }

        return result.normalized * weight;
    }

    public Vector3 GetLeader(Vector3 leaderPosition)
    {
        if (leaderPosition == transform.position) return transform.forward;
        return (leaderPosition - transform.position).normalized * leaderWeight;
    }
}
