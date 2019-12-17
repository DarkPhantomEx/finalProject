using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

public class findHighPriest_System : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<foundHighPriest>().WithAll<follower_tag>().ForEach((Entity F, ref Translation followerTranslation) =>
        {
            Entity PriestLoc = Entity.Null;
            float3 followerPOS = followerTranslation.Value;
            float3 closestPos = float3.zero;
            Entities.WithAll<highpriest_tag>().ForEach((Entity H, ref Translation PriestTranslation) =>
            {

                if (PriestLoc == Entity.Null)
                {
                    //In case there's no Priest taken into consideration
                    PriestLoc = H;
                    closestPos = PriestTranslation.Value;
                }
                

            }); // Done cycling through a set of wayPoints for a Follower
            if (PriestLoc != Entity.Null)
            {
                //Debug.DrawLine(followerPos, closestPos);
                PostUpdateCommands.AddComponent(F, new foundHighPriest { highPriest = PriestLoc });
            }
        });
    }
}
