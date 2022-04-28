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

    private float buffering;
    private bool replayGo;

    private MatchPlayerInfo[] playerInfo;

    private void Start()
    {
        currentFrame = 0;
        buffering = 0;
        replay = new Replay();
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
                // todo: save replay to file. Perhaps as a serialized dictionary... or some DB.
                // todo: write to file anything.
                // todo: format should follow: 
                // FRAME_NUMBER COMMAND_TYPE 
                // COMMAND_ARGS
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
                            case MovementCommand mc:
                                command = "MOVE\r\n";
                                foreach(Unit member in mc.members)
                                {
                                    command += member.id + "\r\n";
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
            // todo: read file, play it. 

            //using (StreamReader sr = File.OpenText("replay.txt"))
            //{
            //    string 
            //}
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
