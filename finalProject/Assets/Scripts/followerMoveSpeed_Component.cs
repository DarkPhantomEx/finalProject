using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public struct followerMoveSpeed_Component : IComponentData
{
   public float MoveSpeed;
}
