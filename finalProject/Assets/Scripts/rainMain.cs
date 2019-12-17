using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
public class rainMain : MonoBehaviour
{
    [SerializeField]
    private Material follower_mat;
    [SerializeField]
    private Material rain_mat;
    [SerializeField]
    private Material rainGod_mat;
    [SerializeField]
    private Mesh follower_mesh;
    [SerializeField]
    private Mesh rain_mesh;
    [SerializeField]
    private Mesh rainGod_mesh;
    [SerializeField]
    private Material highpriest_mat;
    [SerializeField]
    private Mesh highpriest_mesh;
    [SerializeField]
    private Mesh tree_mesh;
    [SerializeField]
    private Material tree_mat;

    private static EntityManager entityManager;

    private void Start()
    {

        entityManager = World.Active.EntityManager;
        CreateWaypoints();
        //Archetypes

        //RainParticle Archetype
        EntityArchetype Rain = entityManager.CreateArchetype(
            typeof(rainParticle_Component),
            typeof(rain_tag),
            typeof(Translation),
            typeof(NonUniformScale),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );
        //WayPoint Archetype
        //EntityArchetype WayPoint = entityManager.CreateArchetype(
        //    typeof(waypoint_tag),
        //    typeof(Translation),
        //    typeof(RenderMesh),
        //    typeof(LocalToWorld)
        //    );

        //Follower Archetype
        EntityArchetype Follower = entityManager.CreateArchetype(
            typeof(follower_tag),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );
        //Individual Entity Creation
        Entity HighPriest = entityManager.CreateEntity(
            typeof(highpriest_tag),
            typeof(waypointCount_Component),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
           );

        //RainGod Creation
        Entity RainGod = entityManager.CreateEntity( 
            typeof(Translation), 
            typeof(Rotation),
            typeof(RenderMesh), 
            typeof(LocalToWorld)
           );

        //Setting HighPriest
        entityManager.SetComponentData(HighPriest, new Translation
        {
            Value = new float3(4 , 1.5f, 0f)
        });
        //Mesh&Mat
        entityManager.SetSharedComponentData(HighPriest, new RenderMesh
        {
            material = highpriest_mat,
            mesh = highpriest_mesh
        });
        //Setting WaypointCounter
        entityManager.SetComponentData(HighPriest, new waypointCount_Component
        {
            waypointcount = 0
        });

        //Setting GodSpawn
        entityManager.SetComponentData(RainGod, new Translation
        {
            Value = new float3(0f, 0f, 0f)
        });
        //Setting RainGod Mesh
        entityManager.SetSharedComponentData(RainGod, new RenderMesh
        {
            material = rainGod_mat,
            mesh = rainGod_mesh
        });

        //NativeArray Containers
        NativeArray<Entity> rainContainer = new NativeArray<Entity>(2000, Allocator.Temp);
        entityManager.CreateEntity(Rain, rainContainer);
        //NativeArray<Entity> waypointContainer = new NativeArray<Entity>(4, Allocator.Temp);
        //entityManager.CreateEntity(WayPoint, waypointContainer);
        NativeArray<Entity> followerContainer = new NativeArray<Entity>(10, Allocator.Temp);
        entityManager.CreateEntity(Follower, followerContainer);

        for (int i = 0; i < rainContainer.Length; i++)
        {
            Entity E = rainContainer[i];
            entityManager.SetComponentData(E, new rainParticle_Component
            {
                particle_ID = i + 1,
                movespeed = 4f
            });
            entityManager.SetComponentData(E, new Translation { Value = new float3(UnityEngine.Random.Range(-20f, 20f), 30f, UnityEngine.Random.Range(-20f, 20f)) });
            entityManager.SetComponentData(E, new NonUniformScale
            {
                Value = new float3(.2f, .18f, .21f)
            });
            entityManager.SetSharedComponentData(E, new RenderMesh
            {
                material = rain_mat,
                mesh = rain_mesh
            });
        }

        ////Waypoint placement locations
        //float3[] wayPoints = new float3[4] { new float3(4, 1.5f, 4), new float3(-4, 1.5f, 4), new float3(-4, 1.5f, -4), new float3(4, 1.5f, -4) };

        //for (int i = 0; i < waypointContainer.Length; i++)
        //{

        //    Entity E = waypointContainer[i];            
        //    entityManager.SetComponentData(E, new Translation { Value = wayPoints[i] });
        //    entityManager.SetSharedComponentData(E, new RenderMesh
        //    {
        //        material = null,
        //        mesh = null
        //    });



        //}

        //Spawn point for the various followers
        float3[] fSpawn = new float3[4] { new float3(10, 1.5f, 0), new float3(0, 1.5f, 4), new float3(-4, 1.5f, 0), new float3(0, 1.5f, -4) };


        for (int i = 0; i < followerContainer.Length; i++)
        {
            Entity E = followerContainer[i];
            entityManager.SetComponentData(E, new Translation { Value = fSpawn[i%4]});
            entityManager.SetSharedComponentData(E, new RenderMesh
            {
                material = follower_mat,
                mesh = follower_mesh
            });

        }
        for(int i =0; i<30; i++)
        TreeGen();
        followerContainer.Dispose();
        //rainContainer.Dispose();
    }

    //Create invisible waypoints for the Priest to follow
    public void CreateWaypoints()
    {
        entityManager = World.Active.EntityManager;
        EntityArchetype WayPoint = entityManager.CreateArchetype(
            typeof(waypoint_tag),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );
        NativeArray<Entity> waypointContainer = new NativeArray<Entity>(5, Allocator.Temp);
        entityManager.CreateEntity(WayPoint, waypointContainer);

        //Waypoint placement locations
        float3[] wayPoints = new float3[5] { new float3(4, 1.5f, 4), new float3(-4, 1.5f, 4), new float3(-4, 1.5f, -4), new float3(4, 1.5f, -4),
            new float3 (200, 1.5f, -4) };

        for (int i = 0; i < waypointContainer.Length; i++)
        {

            Entity E = waypointContainer[i];
            entityManager.SetComponentData(E, new Translation { Value = wayPoints[i] });
            entityManager.SetSharedComponentData(E, new RenderMesh
            {
                material = null,
                mesh = null
            });



        }
    }

    //Generate trees in the Overworld
    private void TreeGen()
    {
        Entity E = entityManager.CreateEntity(
             typeof(Translation),
             typeof(RenderMesh),
             typeof(LocalToWorld)
             );
        if (UnityEngine.Random.value % 2 == 0)
        {
            entityManager.SetComponentData(E, new Translation
            {
                Value = new float3(UnityEngine.Random.Range(-20f, 20f), 1.5f, UnityEngine.Random.Range(-30f, -4f))
            });
        }
        else
        {
            entityManager.SetComponentData(E, new Translation
            {
                Value = new float3(UnityEngine.Random.Range(-20f, 20f), 1.5f, UnityEngine.Random.Range(4f, 20f))
            });
        }
        entityManager.SetSharedComponentData(E, new RenderMesh
        {
            material = tree_mat,
            mesh = tree_mesh
        });

        
    }
} //Namespace

//Tags
public struct rain_tag : IComponentData { }
public struct waypoint_tag : IComponentData { }
public struct highpriest_tag : IComponentData { }
public struct follower_tag : IComponentData { }
//"Locator" tags
public struct foundHighPriest : IComponentData
{
    public Entity highPriest;
}
public struct foundWaypoint : IComponentData
{
    public Entity currWaypoint;
}