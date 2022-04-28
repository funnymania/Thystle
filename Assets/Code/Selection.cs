using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Selection : MonoBehaviour
{
    private bool _isActiveCommand;
    private float _time;
    private Match match;
    private VectorFixed moveTo;
    private GameObject test;

    private float _testTime;
    private MovementCommand move;

    private void Start()
    {
        match = GameObject.Find("Match").GetComponent<Match>();
        _isActiveCommand = false;
        _time = 0;
        moveTo = new VectorFixed(
            (UInt32)UnityEngine.Random.Range(1, 13),
            (UInt32)UnityEngine.Random.Range(1, 13),
            0
        );

         test = match.Brunei;

        SpawnCommand spawn = new SpawnCommand(VectorFixed.zero, test);
        spawn.Execute(0);
        spawn.Record();

        move = new MovementCommand(
            new VectorFixed(4, 0, 0), Match.fieldedUnits[0], new GameObject[] { Match.fieldedUnits[0] }
        );

        Match.matchRunning = true;
        _testTime = 0;
    }

    // todo: "some calculation" which will be grabbing all units within some selection and generate a Selection.
    //       The Selection will already HAVE entity refs.

    // RepumpkinCan
    private void FixedUpdate()
    {
        if (_testTime > 10)
        {
            Match.matchEnd = true;
        }
        else if (!_isActiveCommand)
        {
            // todo: this produces duplicate commands in replay file. We should only be Recording one time per
            //          Command.
            // From list of selected Units, if another point clicked, we will be creating a new MovementCommand.
            _isActiveCommand = move.Execute(); // to coroutine of not?
            move.Record();
        }
        else
        {
            _time += Time.deltaTime;
            if (_time > 4)
            {
                move = new MovementCommand(
                    new VectorFixed(move.destination.x + 8, 0, 0), Match.fieldedUnits[0], new GameObject[] {  Match.fieldedUnits[0] }
                );
                _isActiveCommand = false;
                _time = 0;
            }
        }

        _testTime += Time.deltaTime;
    }
}
