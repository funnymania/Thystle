using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //public VectorFixed moveAmt;
    //public VectorFixed prevComputation;
    //public UInt32 speed;

    //private bool _isActiveCommand = true;
    //private float _time;

    //void Start()
    //{
    //    _time = 0;
    //    prevComputation.SetToUnityTransform(transform.position);

    //    moveAmt = new VectorFixed(
    //        (UInt32)UnityEngine.Random.Range(1, 13),
    //        (UInt32)UnityEngine.Random.Range(1, 13),
    //        0
    //    );
    //}

    // Update is called once per frame
    // NOTE: May have to do something where the spriteRenderer is moving separately from the colliders.
    //       This is to preserve fluid visuals, since the position we will actually be using will
    //       moving at a choppier pace.
//    void FixedUpdate()
//    {
//        // note: moveAmt should not be changed except by player cursor
//        if (_isActiveCommand)
//            ActiveMovementCommand(moveAmt);
//        else
//        {
//            _time += Time.deltaTime;
//            if (_time > 4)
//            {
//                _isActiveCommand = true;

//                moveAmt.SetToUnityTransform(transform.position);
//                moveAmt.x += (UInt32)UnityEngine.Random.Range(1, 13);
//                moveAmt.y += (UInt32)UnityEngine.Random.Range(1, 13);
//                _time = 0;
//            }
//        }

//        // user selects some game objects
//        // we can assume that game objects are at a valid location, because they will be spawned at one.

//        // user selects a location for them to move to.
//        // location will need to approximately match the nearest in a discrete set of values.

//        // game objects will move, if multiple of them in a swarm like fashion, to the new location.

//        // . notes .
//        //
//        // movement is occurring over time. each vector3 returned from thystle_move will need to 
//        // always keep the values it assigns in check with this discrete set of values.
//    }

//    void ActiveMovementCommand(VectorFixed moveAmt)
//    {
//        if (prevComputation.EqualToUnity(transform.position) == false)
//        {
//            // raise exception
//            throw new System.Exception("Position of transform drifting from correct value " 
//                + prevComputation + " as " + transform.position);
//        }

//        // note: STRONG assumption here that casting is safe.
//        VectorFixed currPos = new VectorFixed(0,0,0);
//        currPos.SetToUnityTransform(transform.position);
//        VectorFixed collectMe = FFI.CrunchMovement(moveAmt, currPos, speed);
//        prevComputation.x = collectMe.x;
//        prevComputation.y = collectMe.y;
//        prevComputation.z = collectMe.z;
//        transform.position = prevComputation.AsUnityTransform();

//        if (moveAmt.EqualToUnity(transform.position))
//        {
//            // we are done.
//            _isActiveCommand = false;
//        }
//    }
}


