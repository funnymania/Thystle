using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Match : MonoBehaviour
{
    public static System.Int64 currentFrame;
    public static bool matchRunning;
    public static bool matchEnd;
    public static bool replayRunning;
    public static Replay replay;
    public static Dictionary<System.UInt32, GameObject> fieldedUnits;
    public static System.UInt32 nextUnitId;

    public MatchPlayerInfo[] playerInfo;
    public GameObject Brunei;

    private float buffering;
    private bool replayGo;


    private void Awake()
    {
        currentFrame = 0;
        buffering = 0;
        nextUnitId = 0;
        replay = new Replay();
        fieldedUnits = new Dictionary<uint, GameObject>();
        playerInfo = new MatchPlayerInfo[2] { new MatchPlayerInfo(), new MatchPlayerInfo() };
    }

    void FixedUpdate()
    {
        // yield on some value which will be a match starting...
        if (matchRunning)
        {
            currentFrame += 1;
            if (matchEnd)
            {
                try
                {
                    using (FileStream fs = File.Create("replay.txt"))
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
                                // todo: must add player id to commands. 
                                // todo: replay data must also contain map information
                                case MovementCommand mc:
                                    command = "MOVE\r\n";
                                    foreach(GameObject member in mc.members)
                                    {
                                        Unit unit = member.GetComponent<Unit>();
                                        command += unit.id + "\r\n";
                                    }
                                    string dest = mc.destination.ToString();
                                    command += dest + "\r\n";
                                    break;
                                case SpawnCommand sc:
                                    command = "SPAWN\r\n";
                                    command += sc.newUnit.name + "\r\n";
                                    command += sc.position + "\r\n";
                                    break;
                            }

                            completeFileWrite += frameNum + " " + command + "\r\n";
                        }

                        byte[] info = new UTF8Encoding(true).GetBytes(completeFileWrite);
                        fs.Write(info, 0, info.Length);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log("Could not save replay.\r\n" + e);
                }

                matchRunning = false;
                matchEnd = false;
            }
        }
        else if (buffering > 2)
        {
            buffering += Time.deltaTime;
            replayGo = true;
        }
        else if (replayGo)
        {
            // todo: Add commands to Replay, play it in ReplayPlayer
            replayGo = false;

            Replay loadedReplay = new Replay();
            using (StreamReader sr = File.OpenText("replay.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    // Replay Frame (frame, command)
                    // read line.
                    string line = sr.ReadLine();
                    // break by white space.
                    string[] items = line.Split(' ');
                    // first string is frame
                    System.Int64 frame = 0;
                    System.Int64.TryParse(items[0], out frame);

                    // second dictates command
                    switch (items[1])
                    {
                        case "MOVE":
                            // next line is all ids to be moved.
                            string unitLine = sr.ReadLine();
                            // todo: how do we create the GameObjects to store in Command of ReplayFrames
                            //       with only a Unit's id...?
                            GameObject
                            // next line is position to move to.
                            break;
                        case "SPAWN":
                            break;
                    }
                    ReplayFrame frame = new ReplayFrame(items[0]);

                    // add to replay

                }
            }

            ReplayPlayer player = new ReplayPlayer();
            player.replay = ;
        }
    }

    // User save...
    //       

    public void LoadReplay(int id)
    {
        // load by id. 

        replayRunning = true;
    }

}
