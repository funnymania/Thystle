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

    private Vector3 _dragBeginPosition;
    private Vector3 _dragEndPosition;
    private RaycastHit[] _selection;

    private int _baseIndex = -1;

    private Vector3 beginInput;

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

        test = Match.allUnits["Brunei"];

        //SpawnCommand spawn = new SpawnCommand(VectorFixed.zero, test, 0);
        //match.commander.AddCommand(spawn);

        _time = 5;

        _testTime = 0;
    }

    // todo: "some calculation" which will be grabbing all units within some selection and generate a Selection.
    //       The Selection will already HAVE entity refs.

    // RepumpkinCan
    private void Update()
    {
        // note: input inside the player counts, even if it lies in the 'blackbars'.
        //       we are just ignoring that for now.
        // Left click.
        if (Input.GetMouseButtonDown(0) 
            && Input.mousePosition.x >= 0
            && Input.mousePosition.y >= 0
        )
        {
            beginInput = Input.mousePosition;

            // bug: is only accurate to every ~2565 world units, somehow.
            //      Must implement our own math here.
            // note: we are STILL relying on Unity to capture mouse input at the correct spaces.
            //_dragBeginPosition = Camera.main.ScreenToWorldPoint(
            //    new Vector3(
            //        Input.mousePosition.x, 
            //        Input.mousePosition.y,
            //        Mathf.Abs(Camera.main.farClipPlane)
            //    )
            //);

            // bug: incorrect value, should be 1000,1000, instead reads... -900000?
            _dragBeginPosition = WorldPointForCameraSpace(Camera.main, Input.mousePosition);

            Debug.DrawLine(Vector3.zero, _dragBeginPosition, Color.black, 5, false);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            _dragEndPosition = WorldPointForCameraSpace(Camera.main, Input.mousePosition);
            //_dragEndPosition = Camera.main.ScreenToWorldPoint(
            //    new Vector3(
            //        Input.mousePosition.x,
            //        Input.mousePosition.y,
            //        Mathf.Abs(Camera.main.farClipPlane)
            //    )
            //);

            float xExtents =
                Mathf.Abs(_dragBeginPosition.x - _dragEndPosition.x);
            float yExtents =
                Mathf.Abs(_dragBeginPosition.y - _dragEndPosition.y);
            Vector3 boxCenter = new Vector3(
                xExtents,
                yExtents,
                0
            );

            Debug.Log(boxCenter);

            Debug.DrawLine(_dragBeginPosition, _dragEndPosition, Color.black, 5, false);

            _selection = Physics.BoxCastAll(
                boxCenter,
                new Vector3(xExtents / 2, yExtents / 2, 0),
                Vector3.forward
            );

            ProcessSelection(_selection);

            if (_selection.Length > 0)
            {
                // spawn Brunei from base.
                if (Input.GetKeyUp(KeyCode.A) && _baseIndex != -1)
                {
                    Base aBase = _selection[_baseIndex].transform.GetComponent<Base>();

                    // todo bogman this is the point where our questions really matter.
                    //       If 24 bit precision is enough, then we can utilize the Transform
                    //       associated with a unit. However if not, then we need to either 
                    SpawnCommand sc = new SpawnCommand(
                        VectorFixed.FromVector3(
                            aBase.transform.position + aBase.spawnOffset.AsUnityTransform()
                        ),
                        Match.allUnits["Brunei"],
                        0,
                        Match.nextUnitId
                    );

                    match.commander.nextId += 1;
                }
            }

            Debug.Log("Selected " + _selection.Length + " units.");
        }
        else if (Input.GetMouseButtonUp(1) && _selection.Length > 0) // Right click.
        {
            // get all positions and members.
            VectorFixed[] vectors = new VectorFixed[_selection.Length];
            uint[] unitIds = new uint[_selection.Length];
            for (var i = 0; i < _selection.Length; i += 1)
            {
                vectors[i] = VectorFixed.FromVector3(_selection[i].transform.position);
                unitIds[i] = _selection[i].transform.GetComponent<Unit>().id;
            }

            // get closest member, mark as leader.
            var leaderId = FFI.ClosestMemberTo(
                VectorFixed.FromVector3(Camera.main.ScreenPointToRay(Input.mousePosition).origin),
                vectors,
                (ulong)vectors.Length
            );

            MovementCommand mc = new MovementCommand(
                VectorFixed.FromVector3(Camera.main.ScreenPointToRay(Input.mousePosition).origin),
                (uint)leaderId, 
                unitIds,
                match.commander.nextId
            );

            match.commander.AddCommand(mc);
        }

        //if (_testTime > 10)
        //{
        //    Match.matchEnd = true;
        //}
        //else
        //{
        //    _time += Time.deltaTime;
        //    if (_time > 4)
        //    {
        //        move = new MovementCommand(
        //            new VectorFixed(move.destination.x + 8, 0, 0), 
        //            Match.fieldedUnits[0].GetComponent<Unit>().id, 
        //            new System.UInt32[] { Match.fieldedUnits[0].GetComponent<Unit>().id }
        //        );

        //        match.commander.AddCommand(move);
        //        _time = 0;
        //    }
        //}

        //_testTime += Time.deltaTime;
    }

    /// <summary>
    /// Assumes a Vertical FOV, and a perspective camera.
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="cursorPosition"></param>
    /// <returns></returns>
    public Vector3 WorldPointForCameraSpace(Camera cam, Vector3 cursorPosition)
    {
        float fovRadians = (cam.fieldOfView / 2) * Mathf.PI / 180;

        // Y-distance in world space from center of camera to top left corner. 
        float topLeftY = Mathf.Abs(cam.transform.position.z) * Mathf.Tan(fovRadians);

        // The X-distance.
        float topLeftX = topLeftY * Screen.width / Screen.height; // X Offset from center.

        float viewPortX = cam.transform.position.x - topLeftX; // Left end of viewport.
        float viewPortY = cam.transform.position.y - topLeftY; // Bottom end of viewport.

        float cursorXWorld = viewPortX + (topLeftX * 2 * cursorPosition.x / Screen.width );
        float cursorYWorld = viewPortY + (topLeftY * 2 * cursorPosition.y / Screen.height );

        return new Vector3(cursorXWorld, cursorYWorld);
    }

    /// <summary>
    /// Changes bool/index flags to describe what is in a user selection.
    /// </summary>
    /// <param name="selection"></param>
    public void ProcessSelection(RaycastHit[] selection)
    {
        for (int i = 0; i < selection.Length; i++)
        {
            Unit unit = selection[i].transform.GetComponent<Unit>();
            if (unit.name == "Base")
            {
                _baseIndex = i;
            }
        }
    } 
}
