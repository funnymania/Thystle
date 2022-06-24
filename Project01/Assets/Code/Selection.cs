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

        // Size is a throwaway. I just need this to be referencing something.
        // Without this line, _selection does not have a Length, which we need.
        _selection = new RaycastHit[0];

        //SpawnCommand spawn = new SpawnCommand(VectorFixed.zero, test, 0);
        //match.commander.AddCommand(spawn);

        _time = 5;

        _testTime = 0;
    }

    // todo: do not allow a player to be able to move their opponents pieces (or spawn from their bases, etc)
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

            _dragBeginPosition = CursorPositionInWorldSpace(Camera.main, Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            _dragEndPosition = CursorPositionInWorldSpace(Camera.main, Input.mousePosition);

            float xExtents =
                Mathf.Abs(_dragBeginPosition.x - _dragEndPosition.x);
            float yExtents =
                Mathf.Abs(_dragBeginPosition.y - _dragEndPosition.y);

            Vector3 boxCenter = Vector3.zero;
            if (_dragBeginPosition.x >= _dragEndPosition.x)
            {
                boxCenter.x = _dragEndPosition.x;
                boxCenter.x += xExtents / 2;
            }
            else
            {
                boxCenter.x = _dragBeginPosition.x;
                boxCenter.x += xExtents / 2;
            }

            if (_dragBeginPosition.y > _dragEndPosition.y)
            {
                boxCenter.y = _dragEndPosition.y;
                boxCenter.y += yExtents / 2;
            }
            else
            {
                boxCenter.y = _dragBeginPosition.y;
                boxCenter.y += yExtents / 2;
            }

            Debug.Log(boxCenter);

            Debug.DrawLine(_dragBeginPosition, _dragEndPosition, Color.black, 5, false);

            _selection = Physics.BoxCastAll(
                boxCenter,
                new Vector3(xExtents / 2, yExtents / 2, 0),
                Vector3.forward
            );
            
            ProcessSelection(_selection);

            Debug.Log("Selected " + _selection.Length + " units.");
        }
        else if (Input.GetMouseButtonUp(1) && _selection.Length > 0) // Right click.
        {
            // todo: we need to handle what happens when the user selects something invalid here.
            //       moving to a space that cannot be moved to should have some special logic.

            // get all positions and members.
            List<VectorFixed> vectors = new List<VectorFixed>();
            List<uint> unitIds = new List<uint>();
            for (var i = 0; i < _selection.Length; i += 1)
            {
                if (_selection[i].transform.gameObject.GetComponent<Unit>().isMovable)
                {
                    vectors.Add(VectorFixed.FromVector3(_selection[i].transform.position));
                    unitIds.Add(_selection[i].transform.GetComponent<Unit>().id);
                }
            }

            VectorFixed cursorWorld = VectorFixed.TruncateToVectorFixed(
                    CursorPositionInWorldSpace(Camera.main, Input.mousePosition)
            );

            // get closest member, mark as leader.
            if (vectors.Count != 0)
            {
                var leaderId = FFI.ClosestMemberTo(
                    cursorWorld,
                    vectors.ToArray(),
                    (ulong)vectors.Count
                );

                // note: currently only supporting single player. whichever id a player has would be resolved
                //       in netcode in multiplayer.
                // note: compilation symbols are broken in Unity 2021.3 so just comment out to go back to
                //       old style.
                //MovementCommand mc = new MovementCommand(
                //    cursorWorld,
                //    unitIds[(int)leaderId], 
                //    unitIds.ToArray(),
                //    match.commander.nextId
                //);
                PathMoveCommand mc = new PathMoveCommand(
                    cursorWorld,
                    unitIds[(int)leaderId], 
                    unitIds.ToArray(),
                    match.commander.nextId
                );

                match.commander.AddCommand(mc);
            }
        }

        if (_selection.Length > 0)
        {
            // spawn Brunei from base.
            if (Input.GetKeyUp(KeyCode.A) && _baseIndex != -1)
            {
                Base aBase = _selection[_baseIndex].transform.GetComponent<Base>();

                // todo: spawning a unit must ALSO involve a movement command.
                // bug: scale compensation not working.
                SpawnCommand sc = new SpawnCommand(
                    aBase.truePosition.AddV3(aBase.spawnOffset),
                    Match.allUnits["Peasant"],
                    0,
                    Match.nextUnitId
                );

                match.commander.nextId += 1;
                match.commander.AddCommand(sc);
            }
        }
    }

    /// <summary>
    /// Assumes a Vertical FOV, and a perspective camera.
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="cursorPosition"></param>
    /// <returns></returns>
    public Vector3 CursorPositionInWorldSpace(Camera cam, Vector3 cursorPosition)
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

    //public Vector3 BoxFromDragPoints(Vector3 dragBegin, Vector3 dragEnd)
    //{
    //    Vector3 box = Vector3.zero;
    //    if (dragBegin.x > dragEnd.x)
    //    {
    //        box.x = (dragBegin.x - dragEnd.x) / 2;
    //        box.x
    //    }
    //    else
    //    {

    //    }

    //    if (dragBegin.y > dragEnd.y)
    //    {

    //    }
    //    else
    //    {

    //    }

    //    return box;
    //}

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
