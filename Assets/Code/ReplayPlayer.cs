using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : MonoBehaviour
{
    public delegate void MatchLoop();
    public MatchLoop matchLoop;

    public Replay replay;
    public GameObject camera;
    public bool play;

    private CommandCommander commander;
    private System.UInt64 frameCount;
    private int replayNext;
    private long commandId;

    private void Start()
    {
        replayNext = 0;
        frameCount = 0;
        commandId = 0;
    }

    void FixedUpdate()
    {
        if (play)
        {
            // note: should be able to have multiple commands executing on a single frame.
            while (replayNext < replay.actions.Count && replay.actions[replayNext].frame == frameCount)
            {
                commander.AddCommand(replay.actions[replayNext].command);
                replayNext += 1;
            }

            frameCount += 1;

            matchLoop?.Invoke();
        }        
    }

    public void Play(string replayId)
    {
        // Load in replay by string.
        Load(replayId);

        // set up subscribers.
        commander = new CommandCommander();
        matchLoop += commander.Execution;

        // todo allow camera to look at player one's base.

        //camera.transform.LookAt(Vector3.zero);

        //VectorFixed camPos = new VectorFixed(IntPow(2, 22), IntPow(2, 22), 0);
        //Vector3 unityPos = camPos.AsUnityTransform();

        //transform.localScale = new Vector3(WorldValues.UNIT_SIZE, WorldValues.UNIT_SIZE, WorldValues.UNIT_SIZE);
        //camera.transform.position = new Vector3(unityPos.x, unityPos.y, camera.transform.position.z * WorldValues.UNIT_SIZE);

        play = true;
    }

    // Cleans up the replay.
    public bool Stop()
    {
        play = false;

        // fielded units cleared.
        Match.fieldedUnits.Clear();

        // Remove units.
        Unit[] units = gameObject.GetComponentsInChildren<Unit>();
        foreach(Unit unit in units)
        {
            Destroy(unit.gameObject);
        }

        return true;
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

    private void Load(string replayId)
    {
        Replay loadedReplay = new Replay();
        using (StreamReader sr = File.OpenText(replayId + ".txt"))
        {
            while (sr.Peek() >= 0)
            {
                // Replay Frame (frame, command)
                // read line.
                string line = sr.ReadLine();
                // break by white space.
                string[] items = line.Split(' ');
                // first string is frame
                System.UInt64 frame = 0;
                System.UInt64.TryParse(items[0], out frame);

                ICommand newCommand = null;
                // second dictates command
                switch (items[1])
                {
                    case "MOVE":
                        // next line is all ids to be moved.
                        string[] unitLine = sr.ReadLine().Split(' ');
                        System.UInt32[] units = new System.UInt32[unitLine.Length];
                        for(var i = 0; i < unitLine.Length; i += 1)
                        {
                            System.UInt32.TryParse(unitLine[i], out units[i]);
                        }

                        // next line is position to move to.
                        System.UInt32 x, y, z;
                        string[] position = sr.ReadLine().Split(' ');
                        System.UInt32.TryParse(position[0], out x);
                        System.UInt32.TryParse(position[1], out y);
                        System.UInt32.TryParse(position[2], out z);
                        VectorFixed commandPosition = new VectorFixed(
                            x, y, z
                        );
                        newCommand = new MovementCommand(
                            commandPosition,
                            units[0], units, commandId);
                        break;
                    case "SPAWN":
                        // next line is prefab name, followed by player Id.
                        string[] prefabPlayer = sr.ReadLine().Split(' ');
                        System.UInt32 playerId = 0;

                        System.UInt32.TryParse(prefabPlayer[1], out playerId);

                        // next line is position to spawn at.
                        position = sr.ReadLine().Split(' ');
                        System.UInt32.TryParse(position[0], out x);
                        System.UInt32.TryParse(position[1], out y);
                        System.UInt32.TryParse(position[2], out z);
                        commandPosition = new VectorFixed(
                            x, y, z
                        );
                        newCommand = new SpawnCommand(
                            commandPosition, 
                            Match.allUnits[prefabPlayer[0]], 
                            playerId, commandId);
                        break;
                    case "END":
                        newCommand = new EndCommand(Stop);
                        break;
                }

                ReplayFrame replayFrame = new ReplayFrame(frame, newCommand);

                // increment next command id.
                commandId += 1;

                // Consume hanging new line.
                sr.ReadLine();

                // add to replay
                loadedReplay.actions.Add(replayFrame);
            }
        }

        replay = loadedReplay;
    }
}
