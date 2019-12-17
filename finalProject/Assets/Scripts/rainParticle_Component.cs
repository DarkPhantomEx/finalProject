using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

//Random ID number for rain drops
public struct rainParticle_Component : IComponentData
{
    public int particle_ID;
}
