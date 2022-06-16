using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class Match : MonoBehaviour
{
    public Action matchLoop;
    public static System.UInt64 currentFrame;
    public static bool matchRunning;
    public static bool matchEnd;
    public static bool isReplay;
    public static Replay replay;
    public static Dictionary<System.UInt32, GameObject> fieldedUnits;
    public static Dictionary<System.UInt32, GameObject> staticUnits;

    // Rely on dictionary for quick lookup during runtime.
    public static Dictionary<string, GameObject> allUnits;
    public static System.UInt32 nextUnitId;

    public GameObject camera;
    public MatchPlayerInfo[] playerInfo;
    public List<NamedPrefab> allUnitsDisplayed;
    public ReplayPlayer replayPlayer;
    public CommandCommander commander;

    private NavMesh navMesh;
    private bool replayGo;
    private long commandId;

    private void Awake()
    {
        allUnits = new Dictionary<string, GameObject>();

        // Move into dictionary.
        foreach (NamedPrefab thing in allUnitsDisplayed)
        {
            allUnits[thing.name] = thing.prefab;
        }
    }

    void FixedUpdate()
    {
        if (matchRunning)
        {
            currentFrame += 1;
            matchLoop?.Invoke();
        }
    }

    /// <summary>
    /// Gets all Units which affect the navmesh.
    /// </summary>
    /// <returns></returns>
    public static List<GameObject> getMeshConstrainingUnits()
    {
        List<GameObject> meshConstrainers = new List<GameObject>();
        foreach(GameObject unit in staticUnits.Values)
        {
            if (unit.GetComponent<Unit>().isMovable == false)
            {
                meshConstrainers.Add(unit);
            }
        }

        return meshConstrainers;
    }

    // Real time match setup.
    // initial setup - 
        // give all players some amount of tangos
        // Spawn a base.
        // spawn five brunei from base.
        // there needs to be a predefined way to SPAWN something in the same space as another unit.
        // (we can say clockwise for now)
        // Actually, this needs to be the way to handle two things being on the same space in general,
        //       and it should be used globally. 
    // move five brunei.
    // move on to testing
//            if (pastComputation.EqualToUnity(transform.position) == false)
//            {
//                // raise exception
//                throw new System.Exception("Position of transform drifting from correct value " 
//                    + pastComputation + " as " + transform.position);
//            }
//  maps have spawn points which will need to be included in replay data in some way.
//       all for now, we just hard code.
// We can think of some 2^64 x 2^64 group of possible points. 
// we can spawn one base at 2^10, 2^10 and another at 2^54, 2^54
    public void Play()
    {
        // Initialize values.
        currentFrame = 0;
        commandId = 0;
        nextUnitId = 0;
        replay = new Replay();
        fieldedUnits = new Dictionary<uint, GameObject>();
        staticUnits = new Dictionary<uint, GameObject>();

        // Subscribe commander to the pulse of FixedUpdate.
        commander = new CommandCommander();
        matchLoop += commander.Execution;

        // we use these to represent circles until we can automatically do it according to game developer
        //  adjustment.
        CircleAtlas.Init();

        // load the map.
        navMesh = new NavMesh();

        // Initialize both players.
        playerInfo = new MatchPlayerInfo[2] { new MatchPlayerInfo(), new MatchPlayerInfo() };

        // test: setting player count to 1.
        for (System.UInt64 i = 0; i < (System.UInt64)playerInfo.Length - 1; i++)
        {
            playerInfo[i].tangos = 1610;
            SpawnCommand sc = new SpawnCommand(
                new VectorFixed(
                    IntPow(2, (ulong)WorldValues.MIN_SUPPORTED_BIT_RES - 12 + i), 
                    IntPow(2, (ulong)WorldValues.MIN_SUPPORTED_BIT_RES - 12 + i), 
                    0
                ),
                allUnits["Base"],
                0,
                nextUnitId
            );
            commander.AddCommand(sc);
        }

        // note: at this point, this would be the map and all its walls/ledges, but without spawnable
        //       units. On spawning a unit that constains the mesh, then we will Repath again.
        List<GameObject> gos = getMeshConstrainingUnits();
        List<CircleMcCollider> circles = new List<CircleMcCollider>();
        foreach(GameObject go in gos)
        {
            CircleMcCollider[] circleCols = go.GetComponentsInChildren<CircleMcCollider>();
            circles.AddRange(circleCols);
        }
        NavMesh.Repath(circles);

        VectorFixed camPos = new VectorFixed(
            IntPow(2, (ulong)WorldValues.MIN_SUPPORTED_BIT_RES - 12), 
            IntPow(2, (ulong)WorldValues.MIN_SUPPORTED_BIT_RES - 12), 
            0
        );
        Vector3 unityPos = camPos.AsUnityTransform();

        camera.transform.position = new Vector3(unityPos.x, unityPos.y, camera.transform.position.z * WorldValues.UNIT_SIZE);

        // Allow camera to look at first player's base.
        camera.transform.LookAt(unityPos);

        // Adjust clipping plane to double its distance from the world
        camera.GetComponent<Camera>().farClipPlane = Mathf.Abs(camera.transform.position.z * 2);

        // Scale up the size of the world to use more precision.
        transform.localScale = new Vector3(WorldValues.UNIT_SIZE, WorldValues.UNIT_SIZE, WorldValues.UNIT_SIZE);

        matchRunning = true;
    }

    public static void ConstrainNavMesh()
    {
        List<GameObject> gos = getMeshConstrainingUnits();
        List<CircleMcCollider> circles = new List<CircleMcCollider>();
        foreach(GameObject go in gos)
        {
            CircleMcCollider[] circleCols = go.GetComponentsInChildren<CircleMcCollider>();
            circles.AddRange(circleCols);
        }
        NavMesh.Repath(circles);
    }

    public System.UInt64 IntPow(System.UInt64 theBase, System.UInt64 power)
    {
        System.UInt64 result = 1;
        while (power > 0)
        {
            result *= theBase;
            power -= 1;
        }

        return result;
    }

    public bool Stop()
    {
        matchRunning = false;

        try
        {
            persistReplay();
        }
        catch (System.Exception e)
        {
            Debug.Log("Could not save replay.\r\n" + e);
        }
        
        // Fielded units cleared.
        fieldedUnits.Clear();

        // Remove units.
        Unit[] units = gameObject.GetComponentsInChildren<Unit>();
        foreach(Unit unit in units)
        {
            Destroy(unit.gameObject);
        }

        return true;
    }

    private void persistReplay()
    {
        using (FileStream fs = File.Create("0.txt"))
        {
            fs.SetLength(0);
            string completeFileWrite = "";
            string frameNum = "";
            string command = "";
            for (var i = 0; i < replay.actions.Count; i += 1)
            {
                frameNum = replay.actions[i].frame.ToString();
                switch (replay.actions[i].command)
                {
                    // todo: replay data must also contain map information
                    case MovementCommand mc:
                        command = "MOVE\r\n";
                        foreach(System.UInt32 member in mc.memberIds)
                        {
                            Unit unit = Match.fieldedUnits[member].GetComponent<Unit>();
                            command += unit.id + "\r\n";
                        }
                        string dest = mc.destination.ToString();
                        command += dest + "\r\n";
                        break;
                    case SpawnCommand sc:
                        command = "SPAWN\r\n";
                        command += sc.newUnit.name + " " + sc.playerId + "\r\n";
                        command += sc.position + "\r\n";
                        break;
                }

                completeFileWrite += frameNum + " " + command + "\r\n";
            }

            byte[] info = new UTF8Encoding(true).GetBytes(completeFileWrite);
            fs.Write(info, 0, info.Length);
        }
    }
}
