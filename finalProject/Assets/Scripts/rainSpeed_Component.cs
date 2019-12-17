using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

//Movement speed component for the raindrops
public struct rainSpeed_Component : IComponentData
{
    public float rainMoveSpeed;
}
