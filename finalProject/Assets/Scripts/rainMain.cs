using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine.UI;


public class rainMain : MonoBehaviour
{
    public int RainDropSpawner = 200;

    public Text RainDropCount;
    int numberOfDrops = 0;

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
    [SerializeField]
    private Material land_mat;
    [SerializeField]
    private Mesh land_mesh;

    private static EntityManager entityManager;

    private void Start()
    {

        entityManager = World.Active.EntityManager;
        SpawnLand();
        CreateWaypoints();

        for (int i =0; i<30; i++)
        TreeGen();
        SpawnRainGod();        
        SpawnFollowers();
        SpawnHighPriest();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRain();
        }
    }
    private void SpawnLand()
    {
        Entity land = entityManager.CreateEntity(
            typeof(Translation),
            typeof(NonUniformScale),
            typeof(RenderMesh),
            typeof(LocalToWorld));

        entityManager.SetComponentData(land, new Translation
        {
            Value = float3.zero
        });

        entityManager.SetComponentData(land, new NonUniformScale
        {
            Value = new float3(90, 1, 90)
        });
        entityManager.SetSharedComponentData(land, new RenderMesh
        {
            material = land_mat,
            mesh = land_mesh
        });
    }
    private void SpawnRainGod()
    {
        //RainGod Creation
        Entity RainGod = entityManager.CreateEntity(
            typeof(Translation),
            typeof(NonUniformScale),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
           );

        //Setting GodSpawn
        entityManager.SetComponentData(RainGod, new Translation
        {
            Value = new float3(0f, 0f, 0f)
        });
        //Scaling
        entityManager.SetComponentData(RainGod, new NonUniformScale
        {
            Value = new float3(2, 2, 2)
        });
        //Setting RainGod Mesh
        entityManager.SetSharedComponentData(RainGod, new RenderMesh
        {
            material = rainGod_mat,
            mesh = rainGod_mesh
        });
    }
    private void SpawnHighPriest()
    {
        Entity HighPriest = entityManager.CreateEntity(
            typeof(highpriest_tag),
            typeof(waypointCount_Component),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
           );
        //Setting HighPriest
        entityManager.SetComponentData(HighPriest, new Translation
        {
            Value = new float3(4, 1.5f, 0f)
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
    }
    private void SpawnFollowers()
    {
        //Follower Archetype
        EntityArchetype Follower = entityManager.CreateArchetype(
            typeof(follower_tag),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );

        NativeArray<Entity> followerContainer = new NativeArray<Entity>(10, Allocator.Temp);
        entityManager.CreateEntity(Follower, followerContainer);

        //Spawn point for the various followers
        float3[] fSpawn = new float3[4] { new float3(10, 1.5f, 0), new float3(0, 1.5f, 4), new float3(-4, 1.5f, 0), new float3(0, 1.5f, -4) };


        for (int i = 0; i < followerContainer.Length; i++)
        {
            Entity E = followerContainer[i];
            entityManager.SetComponentData(E, new Translation { Value = fSpawn[i % 4] });
            entityManager.SetSharedComponentData(E, new RenderMesh
            {
                material = follower_mat,
                mesh = follower_mesh
            });

        }
        followerContainer.Dispose();
    }
    private void SpawnRain()
    {
        //RainParticle Archetype
        EntityArchetype Rain = entityManager.CreateArchetype(
            typeof(rainParticle_Component),
            typeof(rain_tag),
            typeof(rainSpeed_Component),
            typeof(Translation),
            typeof(NonUniformScale),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );

        NativeArray<Entity> rainContainer = new NativeArray<Entity>(RainDropSpawner, Allocator.Temp);
        entityManager.CreateEntity(Rain, rainContainer);

        for (int i = 0; i < rainContainer.Length; i++)
        {
            Entity E = rainContainer[i];
            entityManager.SetComponentData(E, new rainParticle_Component
            {
                particle_ID = i + 1,
                movespeed = 4f
            });
            entityManager.SetComponentData(E, new Translation { Value = new float3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(22f, 30f), UnityEngine.Random.Range(-20f, 20f)) });
            entityManager.SetComponentData(E, new rainSpeed_Component { rainMoveSpeed = UnityEngine.Random.Range(1.0f, 7.5f) });
            entityManager.SetComponentData(E, new NonUniformScale
            {
                Value = new float3(.1f, .15f, .1f)
            });
            entityManager.SetSharedComponentData(E, new RenderMesh
            {
                material = rain_mat,
                mesh = rain_mesh
            });
        }
        rainContainer.Dispose();

        numberOfDrops += RainDropSpawner;
        RainDropCount.text = numberOfDrops.ToString();
    }    
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
        waypointContainer.Dispose();
    }
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