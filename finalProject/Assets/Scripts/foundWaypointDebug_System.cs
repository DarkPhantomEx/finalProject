using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class foundWaypointDebug_System : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity H, ref Translation translation, ref foundWaypoint waypointFound) =>
        {
            Translation currentWaypoint = World.Active.EntityManager.GetComponentData<Translation>(waypointFound.currWaypoint);
            Debug.DrawLine(translation.Value, currentWaypoint.Value);
        });
    }
}
