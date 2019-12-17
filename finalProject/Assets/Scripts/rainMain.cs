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
    //Keeps count of total number of Raindrop entities spawned
    public int RainDropSpawner = 200;

    public Text RainDropCount;
    int numberOfDrops = 0;

    //Materials and Meshes used in-game.
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
    [SerializeField]
    private Material wetland_mat;
    [SerializeField]
    private Material darkerland_mat;
    [SerializeField]
    private Material floodland_mat;

    private static EntityManager entityManager;

    //Start - Generates Terrain
    private void Start()
    {
        entityManager = World.Active.EntityManager;
        SpawnLand(); //Spawns base surface
        CreateWaypoints(); //Spawns invisible waypoints for the "HighPriest" to follow

        for (int i =0; i<50; i++)
        TreeGen(); //Generates 50 trees at random locations
        SpawnRainGod(); //Spawns the Rain God (a.k.a Snowman) 
        SpawnFollowers(); //Spawns the Followers that follow the High Priest
        SpawnHighPriest(); //Spawns the HighPriest
    }

    private void Update()
    {
        //Spawns rain if Spacebar is pressed
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRain();
        }
        
        //The below conditions respawn the base land, if the Raindropcount reaches the number mentioned
        if(numberOfDrops > 10000)
        {
            SpawnLand();
        }
        if(numberOfDrops > 15000)
        {
            SpawnLand();
        }
        if(numberOfDrops >20000)
        {
            SpawnLand();
        }
    }
    
   
    private void SpawnLand()
    {
        //Creates Land Entity
        Entity land = entityManager.CreateEntity(
            typeof(Translation),
            typeof(NonUniformScale),
            typeof(RenderMesh),
            typeof(LocalToWorld));
        //Spawns base "barren" land
        if (numberOfDrops < 10000)
        {
            //Sets Translation
            entityManager.SetComponentData(land, new Translation
            {
                Value = float3.zero
            });

            //Sets Scale
            entityManager.SetComponentData(land, new NonUniformScale
            {
                Value = new float3(90, 1, 90)
            });
            //Sets Material and Mesh to be rendered onto the Entity
            entityManager.SetSharedComponentData(land, new RenderMesh
            {
                material = land_mat,
                mesh = land_mesh
            });
        }
        else
        //Spawns base "green" land
        if(numberOfDrops >=20000)
        {
            //Sets Scale
            entityManager.SetComponentData(land, new NonUniformScale
            {
                Value = new float3(90, 4f, 90)
            });

            //Sets Material and Mesh to be rendered onto the Entity
            entityManager.SetSharedComponentData(land, new RenderMesh
            {
                material = floodland_mat,
                mesh = land_mesh
            });
        }else    
        //Spawns base "wet land"
        if(numberOfDrops >= 15000)
        {
            //Sets Scale
            entityManager.SetComponentData(land, new NonUniformScale
            {
                Value = new float3(90, 1.2f, 90)
            });

            //Sets Material and Mesh to be rendered onto the Entity
            entityManager.SetSharedComponentData(land, new RenderMesh
            {
                material = darkerland_mat,
                mesh = land_mesh
            });
        }else
        //Spawns "flood"
        if(numberOfDrops >= 10000)
        {
            //Sets Scale
            entityManager.SetComponentData(land, new NonUniformScale
            {
                Value = new float3(90, 1.1f, 90)
            });

            //Sets Material and Mesh to be rendered onto the Entity
            entityManager.SetSharedComponentData(land, new RenderMesh
            {
                material = wetland_mat,
                mesh = land_mesh
            });
        }
    }
    private void SpawnRainGod()
    {
        //Creating Entity
        Entity RainGod = entityManager.CreateEntity(
            typeof(Translation),
            typeof(raingod_tag),
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
        //Creating Entity
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
            typeof(followerMoveSpeed_Component),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );
        
        //Creating Native Array to contain the various followers
        NativeArray<Entity> followerContainer = new NativeArray<Entity>(10, Allocator.Temp);
        entityManager.CreateEntity(Follower, followerContainer);

        //Spawn point for the various followers
        float3[] fSpawn = new float3[4] { new float3(10, 1.5f, 0), new float3(0, 1.5f, 4), new float3(-4, 1.5f, 0), new float3(0, 1.5f, -4) };

        //Setting the various Components of the Followers
        for (int i = 0; i < followerContainer.Length; i++)
        {
            Entity E = followerContainer[i];
            entityManager.SetComponentData(E, new Translation { Value = fSpawn[i % 4] }); //Translation
            entityManager.SetComponentData(E, new followerMoveSpeed_Component //Setting Random movespeed for the various followers
            {
                MoveSpeed = UnityEngine.Random.Range(3f, 10f)
            });

            entityManager.SetSharedComponentData(E, new RenderMesh //Setting Mesh and Material of the followers
            {
                material = follower_mat,
                mesh = follower_mesh
            });

        }
        followerContainer.Dispose(); //Disposing the Nativearray/container
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

        //Creating NativeArray / Container
        NativeArray<Entity> rainContainer = new NativeArray<Entity>(RainDropSpawner, Allocator.Temp);
        entityManager.CreateEntity(Rain, rainContainer);

        //Setting Components for each RainDrop
        for (int i = 0; i < rainContainer.Length; i++)
        {
            Entity E = rainContainer[i];
            entityManager.SetComponentData(E, new rainParticle_Component //Just a particle/drop ID number
            {
                particle_ID = i + 1
            });
            entityManager.SetComponentData(E, new Translation { Value = new float3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(22f, 30f), UnityEngine.Random.Range(-20f, 20f)) });
            entityManager.SetComponentData(E, new rainSpeed_Component { rainMoveSpeed = UnityEngine.Random.Range(1.0f, 7.5f) }); //Raindrop rainfall speed
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

        //Increments the Raindrop counter
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
        NativeArray<Entity> waypointContainer = new NativeArray<Entity>(8, Allocator.Temp);
        entityManager.CreateEntity(WayPoint, waypointContainer);

        //Waypoint placement locations
        float3[] wayPoints = new float3[8] { new float3(4, 1.5f, 4), new float3(-4, 1.5f, 4), new float3(-4, 1.5f, -4), new float3(4, 1.5f, -4),
            new float3 (40, 1.5f, -4), new float3(0, 1.5f, -20), new float3(-50, 1.5f, -10), new float3(60,1.5f,0) };

        //Generates Invisible waypoints for the HighPriest to follow
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
        //Using Random to generate trees in different sections of the map
        if (UnityEngine.Random.value % 2 == 0)
        {
            entityManager.SetComponentData(E, new Translation
            {
                Value = new float3(UnityEngine.Random.Range(-20f, 20f), 1f, UnityEngine.Random.Range(-30f, -4f))
            });
        }
        else
        {
            entityManager.SetComponentData(E, new Translation
            {
                Value = new float3(UnityEngine.Random.Range(-20f, 20f), 1f, UnityEngine.Random.Range(4f, 20f))
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
public struct raingod_tag : IComponentData { }
//"Locator" tags
public struct foundHighPriest : IComponentData
{
    public Entity highPriest;
}
public struct foundWaypoint : IComponentData
{
    public Entity currWaypoint;
}