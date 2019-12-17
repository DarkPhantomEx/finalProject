using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct rainParticle_Component : IComponentData
{
    public int particle_ID;
    public float movespeed;
}
