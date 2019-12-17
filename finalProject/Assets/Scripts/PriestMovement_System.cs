using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PriestMovement_System : ComponentSystem
{
    //System that checks on the location of the invisible waypoint closest to the priest and moves the priest towards it
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity H, ref foundWaypoint waypointFound, ref Translation highPriestPOS, ref waypointCount_Component count) =>
        {
            Translation currentWaypoint = World.Active.EntityManager.GetComponentData<Translation>(waypointFound.currWaypoint);

            float3 targetLoc = math.normalize(currentWaypoint.Value - highPriestPOS.Value);
            float movespeed = 10f;
            highPriestPOS.Value += targetLoc * movespeed * Time.deltaTime;

            if (math.distance(highPriestPOS.Value, currentWaypoint.Value) < .15f)
            {
                PostUpdateCommands.DestroyEntity(waypointFound.currWaypoint);
                PostUpdateCommands.RemoveComponent(H, typeof(foundWaypoint));               
            }
        });
    }
}
