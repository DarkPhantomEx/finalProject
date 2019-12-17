using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class rainDestroyer_System : ComponentSystem
{
    //System to destroy Rain drops that are close to the land/base
    protected override void OnUpdate()
    {
        Entities.WithAll(typeof(rain_tag)).ForEach((Entity E, ref Translation translation) =>
        {
            if (translation.Value.y < .3f)
                EntityManager.DestroyEntity(E);
        });
        }    
}
