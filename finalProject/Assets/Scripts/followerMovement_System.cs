﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class followerMovement_System : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity F, ref foundHighPriest foundPriest, ref Translation followerPOS) =>
        {
            Translation currentWaypoint = World.Active.EntityManager.GetComponentData<Translation>(foundPriest.highPriest);

            float3 targetLoc = math.normalize(currentWaypoint.Value - followerPOS.Value);
            float movespeed = UnityEngine.Random.Range(3f,8f);
            followerPOS.Value += targetLoc * movespeed * Time.deltaTime;

            if (math.distance(followerPOS.Value, currentWaypoint.Value) < .15f)
            {
                //PostUpdateCommands.DestroyEntity(foundPriest.highPriest);
                PostUpdateCommands.RemoveComponent(F, typeof(foundHighPriest));
            }
        });
    }
}
