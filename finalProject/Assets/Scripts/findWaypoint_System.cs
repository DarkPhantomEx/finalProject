using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

public class findWaypoint_System : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<foundWaypoint>().WithAll<highpriest_tag>().ForEach((Entity H, ref Translation highPriestTranslation) =>
        {
            Entity closestWaypoint = Entity.Null;
            float3 highPriestPOS = highPriestTranslation.Value;
            float3 closestPos = float3.zero;
        Entities.WithAll<waypoint_tag>().ForEach((Entity W, ref Translation wayPointTranslation) =>
        {

            if(closestWaypoint == Entity.Null)
            {
                //In case there's no waypoint taken into consideration
                closestWaypoint = W;
                closestPos = wayPointTranslation.Value;
            }
            else
            if(math.distance(highPriestPOS, wayPointTranslation.Value) < math.distance(highPriestPOS, closestPos))
            {
                //If waypoint is closer
                closestWaypoint = W;
                closestPos = wayPointTranslation.Value;
            }           

        }); // Done cycling through a set of wayPoints for a Follower
            if (closestWaypoint != Entity.Null)
            {
                //Debug.DrawLine(followerPos, closestPos);
                PostUpdateCommands.AddComponent(H, new foundWaypoint { currWaypoint = closestWaypoint });
            }
        });        
    }
}
