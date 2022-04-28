using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Brunei : Unit
{
    private VectorFixed currentPosition;
    private VectorFixed pastComputation;
    private VectorFixed moveTo;

    private bool _isActiveCommand;
    private float _time;

    private void Start()
    {
        pastComputation = VectorFixed.zero;
        currentPosition = VectorFixed.zero;
        // moveTo = new VectorFixed(4, 2, 0);
        moveTo = new VectorFixed(
            (UInt32)UnityEngine.Random.Range(1, 13),
            (UInt32)UnityEngine.Random.Range(1, 13),
            0
        );

        _time = 0;

        _isActiveCommand = true;
        name = "Brunei";
    }

    private void FixedUpdate()
    {
//        if (_isActiveCommand)
//        {
//#if (UNITY_EDITOR)
//            // verify that alterations from backend are aligned with the transform
//            if (pastComputation.EqualToUnity(transform.position) == false)
//            {
//                // raise exception
//                throw new System.Exception("Position of transform drifting from correct value " 
//                    + pastComputation + " as " + transform.position);
//            }
//#endif

//            pastComputation = FFI.CrunchMovement(moveTo, pastComputation, 2);
//            transform.position = pastComputation.AsUnityTransform();

//            if (moveTo == pastComputation)
//            {
//                _isActiveCommand = false;
//            }
//        }
//        else
//        {
//            _time += Time.deltaTime;
//            if (_time > 4)
//            {
//                moveTo = new VectorFixed(
//                    moveTo.x + (UInt32)UnityEngine.Random.Range(1, 13),
//                    moveTo.y + (UInt32)UnityEngine.Random.Range(1, 13),
//                    0
//                );
//                _isActiveCommand = true;
//                _time = 0;
//            }
//        }
    }
}

/**
 * 
 * 
 * 
 * 
        if (_isActiveCommand)
        {
            pastComputation = FFI.CrunchMovement(moveTo, pastComputation, 2);
            transform.position = pastComputation.AsUnityTransform();

            if (moveTo == pastComputation)
            {
                _isActiveCommand = false;
            }
        }
        else
        {
            _time += Time.deltaTime;
            if (_time > 4)
            {
                moveTo = new VectorFixed(
                    moveTo.x + 6,
                    moveTo.y + 6,
                    0
                );
                _isActiveCommand = true;
                _time = 0;
            }
        }
 * 
 * 
 * 
 * 
 * 
 */
