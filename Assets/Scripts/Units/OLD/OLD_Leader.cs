using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OLD_Leader : OLD_Unit
{
    protected override void Start()
    {
        base.Start();

        damage = FlyWeightPointer.Leader.damage;
        hp = FlyWeightPointer.Leader.hp;
        speed = FlyWeightPointer.Leader.speed;
    }

    public override void Pause(bool paused)
    {
        base.Pause(paused);
        if (target != null)
        {
            if (paused)
            {
                if (lastPathCoroutine != null) StopCoroutine(lastPathCoroutine);
            }
            else
            {
                lastPathCoroutine = StartCoroutine(UpdatePath());
            }
        }
    }
}
