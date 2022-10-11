using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlyWeightPointer
{
    public static readonly FlyWeight Leader = new FlyWeight
    {
        speed = 5f,
        damage = 3f,
        hp = 15f,
        atkSpeed = 1.5f,
    };

    public static readonly FlyWeight Soldier = new FlyWeight
    {
        speed = 5.2f,
        damage = 1f,
        hp = 7f,
        atkSpeed = 1f,
    };
}
