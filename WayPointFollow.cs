using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Created by Paul O'Callaghn
/// This script is for having objects follow a set path laid out by you in the inspector
/// it requires objects being spawned to have Object_Waypoint_Data class attached to it
/// 
/// 
/// </summary>




public class WayPointFollow : MonoBehaviour
{

    [Tooltip("WARNING: This will destroy all placed objects in your scene!!!")]
    public bool CLEAR_LIST = false;


    [Space(20)]
    [Header("Objects to Follow on Tracks")]
    // This is what is to be rendered as a track that the game object will follow
    public GameObject objectToSpawn;
    [Range(0, 99)]
    public int numberOfObjects;


    [Space(10)]
    [Header("Waypoint Object")]
    public GameObject wayPointToSpawn;
    [Range(2, 99)]
    public int numberOfWayPoints = 2;

    //[SerializeField]
    private List<Transform> _wayPoint = new List<Transform>();

    //[SerializeField]
    private List<Transform> _movingObjects = new List<Transform>();

    private bool _reverse = false;

    public enum PlacementType
    {
        Manual,
        Spred_Even
    }

    public enum MovementType
    {
        Go_Back_To_Start,
        Back_And_Forth,
        Loop
    }

    public enum MovementSpace
    {
        Vector_2D,
        Vector_3D
    }

    public PlacementType placementType;

    public MovementSpace type = MovementSpace.Vector_3D;

    public MovementType movementType;




    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.IsPaused() == false)
        {
            SetEachObjetPathOnWayPoint();
        }
    }

    void SetEachObjetPathOnWayPoint()
    {
        foreach (Transform movingObject in _movingObjects)
        {
            Object_Waypoint_Data wp = movingObject.GetComponent<Object_Waypoint_Data>();


            switch (type)
            {
                case MovementSpace.Vector_3D:
                    movingObject.position = Vector3.MoveTowards(movingObject.position,
                        _wayPoint[wp.myTarget].position, wp.speed * Time.fixedDeltaTime);

                    if (Vector3.Distance(movingObject.position, _wayPoint[wp.myTarget].position) < .2f)
                    {
                        movingObject.position = _wayPoint[wp.myTarget].position;

                        UpdateWayPoint(wp, movingObject);
                    }
                    break;

                case MovementSpace.Vector_2D:
                    movingObject.position = Vector2.MoveTowards(movingObject.position,
                      _wayPoint[wp.myTarget].position, wp.speed * Time.fixedDeltaTime);

                    if (Vector2.Distance(movingObject.position, _wayPoint[wp.myTarget].position) < .2f)
                    {
                        movingObject.position = _wayPoint[wp.myTarget].position;

                        UpdateWayPoint(wp, movingObject);
                    }
                    break;

            }


        }
    }

    void UpdateWayPoint(Object_Waypoint_Data wp, Transform movingObject)
    {
        switch (movementType)
        {
            case MovementType.Back_And_Forth:
                if (wp.reverse)
                {
                    wp.myTarget--;

                    if (wp.myTarget < 0)
                    {
                        wp.reverse = false;
                        wp.myTarget = 1;
                    }
                }
                else if (wp.reverse == false)
                {
                    wp.myTarget++;

                    if (wp.myTarget > _wayPoint.Count - 1)
                    {
                        wp.reverse = true;
                        wp.myTarget = _wayPoint.Count - 1;
                    }
                }

                break;

            case MovementType.Go_Back_To_Start:
                wp.myTarget++;

                if (wp.myTarget > _wayPoint.Count - 1)
                {
                    movingObject.position = _wayPoint[0].position;
                    wp.myTarget = 1;
                }

                break;

            case MovementType.Loop:
                wp.myTarget++;
                if (wp.myTarget > _wayPoint.Count - 1)
                {
                    wp.myTarget = 0;
                }
                break;
        }
    }


    void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            AddWayPoints();
            AddMovableObjects();
        }
        DebugTrackLines();

        if (CLEAR_LIST)
        {
            ClearList();
        }
    }


    void AddWayPoints()
    {

        if (_wayPoint.Count != numberOfWayPoints)
        {
            for (int i = 0; i < _wayPoint.Count; i++)
            {
                if (_wayPoint[i] == null)
                {

                    _wayPoint.RemoveAt(i);
                    i = -1;
                }

            }


            while (_wayPoint.Count > numberOfWayPoints)
            {
                GameObject waypointToDestroy = _wayPoint[_wayPoint.Count - 1].gameObject;
                _wayPoint.RemoveAt(_wayPoint.Count - 1);
                DestroyImmediate(waypointToDestroy);

            }

            while (_wayPoint.Count < numberOfWayPoints)
            {
                GameObject newWaypoint = Instantiate(wayPointToSpawn, transform);
                newWaypoint.name = "Waypoint " + _wayPoint.Count;
                newWaypoint.transform.position = transform.position;
                _wayPoint.Add(newWaypoint.transform);
            }

            // if part of list is empty
            for (int i = 0; i < _movingObjects.Count; i++)
            {
                if (_movingObjects[i] == null)
                {
                    GameObject newWaypoint = Instantiate(wayPointToSpawn, transform);
                    newWaypoint.name = "Waypoint " + _wayPoint.Count;
                    newWaypoint.transform.position = transform.position;
                    _wayPoint[i] = newWaypoint.transform;
                }
            }



        } // end if object count changes
    }

    void AddMovableObjects()
    {

        if (_movingObjects.Count != numberOfObjects)
        {
            for (int i = 0; i < _movingObjects.Count; i++)
            {
                if (_movingObjects[i] == null)
                {

                    _movingObjects.RemoveAt(i);
                    i = -1;
                }

            }


            while (_movingObjects.Count > numberOfObjects)
            {
                GameObject movingObjectToDestroy = _movingObjects[_movingObjects.Count - 1].gameObject;
                _movingObjects.RemoveAt(_movingObjects.Count - 1);
                DestroyImmediate(movingObjectToDestroy);

            }

            while (_movingObjects.Count < numberOfObjects)
            {
                GameObject newMovingObject = Instantiate(objectToSpawn, transform);
                newMovingObject.name = objectToSpawn.name + " " + _movingObjects.Count;
                newMovingObject.transform.position = _wayPoint[0].position;
                _movingObjects.Add(newMovingObject.transform);
            }

            for (int i = 0; i < _movingObjects.Count; i++)
            {
                if (_movingObjects[i] == null)
                {
                    GameObject newMovingObject = Instantiate(objectToSpawn, transform);
                    newMovingObject.name = objectToSpawn.name + " " + _movingObjects.Count;
                    newMovingObject.transform.position = _wayPoint[0].position;
                    _movingObjects[i] = newMovingObject.transform;
                }
            }

            switch (placementType)
            {
                case PlacementType.Spred_Even:
                    SpreadEvenly();
                    break;
                case PlacementType.Manual:
                    break;
            }


        } // end if object count changes
    }

    void DebugTrackLines()
    {
        if (_wayPoint != null)
        {
            for (int i = 0; i < _wayPoint.Count; i++)
            {

                Gizmos.DrawIcon(_wayPoint[i].position, _wayPoint[i].name, true);

                // Draws a blue line from this transform to the target
                if (i != _wayPoint.Count - 1)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(_wayPoint[i].position, _wayPoint[i + 1].position);
                }
                else
                {
                    if (movementType == MovementType.Loop)
                    {

                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(_wayPoint[i].position, _wayPoint[0].position);
                    }
                }
            }
        }
    }


    void ClearList()
    {
        for (int i = 0; i < _movingObjects.Count; i++)
        {
            if (_movingObjects[i] != null)
            {

                DestroyImmediate(_movingObjects[i].gameObject);
            }
            _movingObjects.RemoveAt(i);
        }
        for (int i = 2; i < _wayPoint.Count; i++)
        {
            if (_wayPoint[i] != null)
            {

                DestroyImmediate(_wayPoint[i].gameObject);
            }
            _wayPoint.RemoveAt(i);
        }
        numberOfObjects = 0;
        numberOfWayPoints = 2;
        CLEAR_LIST = false;
    }


    void SpreadEvenly()
    {
        float dist = 0;
        float spreadAmount = 0;

        if (_wayPoint.Count >= 2)
        {
            for (int i = 0; i < _wayPoint.Count; i++)
            {
                if (i == _wayPoint.Count - 1 && movementType == MovementType.Loop)
                {
                    dist += Vector3.Distance(_wayPoint[i].position, _wayPoint[0].position);
                    break;
                }
                else if (i != _wayPoint.Count - 1)
                {
                    dist += Vector3.Distance(_wayPoint[i].position, _wayPoint[i + 1].position);
                }
            }

            spreadAmount = dist / _movingObjects.Count;

            Debug.Log("Spread: " + spreadAmount);

            if (_wayPoint.Count >= 2)
            {
                switch (movementType)
                {
                    case MovementType.Go_Back_To_Start:
                        ReturnToStart(spreadAmount);
                        break;
                    case MovementType.Back_And_Forth:
                        BackAndForth(spreadAmount);
                        break;
                    case MovementType.Loop:
                        Loop(spreadAmount);
                        break;
                }
            } // greater or equal to 2
            else
            {
                Vector3 dir = _wayPoint[1].position - _wayPoint[0].position;

                for (int i = 0; i < _movingObjects.Count; i++)
                {
                    if (_movingObjects[i] != null)
                    {
                        _movingObjects[i].position = _wayPoint[0].position + (Vector3.Normalize(dir) * (spreadAmount * i));
                    }
                }
            }
        } // end check waypoint is = or greater to 2
    } // End Spread Evenly


    void BackAndForth(float t_spreadAmount)
    {

        ReturnToStart(t_spreadAmount);

        int x = 0;

        for (int i = 0; i < _movingObjects.Count; i++)
        {
            if (x == 1)
            {
                _movingObjects[i].GetComponent<Object_Waypoint_Data>().myTarget -= 1;
                _movingObjects[i].GetComponent<Object_Waypoint_Data>().reverse = true;
                x = 0;
            }
            else
            {
                x++;
            }
        }




    }

    void Loop(float t_spreadAmount)
    {
        // initial direction for alligining objects
        Vector3 dir = _wayPoint[1].position - _wayPoint[0].position;

        for (int i = 0; i < _movingObjects.Count; i++)
        {
            _movingObjects[i].position = _wayPoint[0].position + (Vector3.Normalize(dir) * (t_spreadAmount * i));
        }


        foreach (Transform _object in _movingObjects)
        {
            Object_Waypoint_Data owd = _object.GetComponent<Object_Waypoint_Data>();
            owd.myTarget = 1; // increade their target as they have moved from pi

            int safetyPoint = 0;

            // initialise values
            float myDistance = Vector3.Distance(_object.position, _wayPoint[owd.myTarget - 1].position);
            float distanceBetweenPoints = Vector3.Distance(_wayPoint[owd.myTarget - 1].position, _wayPoint[owd.myTarget].position);

            // while distance of me from my previous target is greater than the target from start point
            while ((myDistance > distanceBetweenPoints) && safetyPoint < 10)
            {
                // store amount of distance off object is from current target
                float offsetDistance = Vector3.Distance(_object.position, _wayPoint[owd.myTarget].position);

                if (owd.myTarget < _wayPoint.Count - 1)
                {


                    Debug.Log(owd.name + " working");
                    // dir = target.pos - start.pos
                    Vector3 direction = _wayPoint[owd.myTarget + 1].position - _wayPoint[owd.myTarget].position;


                    // set new position
                    _object.position = _wayPoint[owd.myTarget].position + (Vector3.Normalize(direction) * offsetDistance);
                    owd.myTarget++;

                    // update new values
                    myDistance = Vector3.Distance(_object.position, _wayPoint[owd.myTarget - 1].position);
                    distanceBetweenPoints = Vector3.Distance(_wayPoint[owd.myTarget - 1].position, _wayPoint[owd.myTarget].position);

                }
                else
                {
                    Debug.Log(owd.name + " dfdf");
                    // dir = target.pos - start.pos we go back to start
                    Vector3 direction = _wayPoint[0].position - _wayPoint[owd.myTarget].position;

                    // set new position
                    _object.position = _wayPoint[owd.myTarget].position + (Vector3.Normalize(direction) * offsetDistance);
                    owd.myTarget = 0;


                    myDistance = Vector3.Distance(_object.position, _wayPoint[_wayPoint.Count - 1].position);
                    distanceBetweenPoints = Vector3.Distance(_wayPoint[_wayPoint.Count - 1].position, _wayPoint[owd.myTarget].position);
                }



                // on the off chance we have an endless while loop
                safetyPoint++;
            } // end while 


        } // end foreach
    } // end loop 


    void ReturnToStart(float t_spreadAmount)
    {
        // initial direction for alligining objects
        Vector3 dir = _wayPoint[1].position - _wayPoint[0].position;

        for (int i = 0; i < _movingObjects.Count; i++)
        {
            _movingObjects[i].position = _wayPoint[0].position + (Vector3.Normalize(dir) * (t_spreadAmount * i));
        }

        foreach (Transform _object in _movingObjects)
        {
            Object_Waypoint_Data owd = _object.GetComponent<Object_Waypoint_Data>();
            owd.myTarget = 1; // increade their target as they have moved from pi

            int safetyPoint = 0;

            // while distance of me from my previous target is greater than the target from start point
            while ((Vector3.Distance(_object.position, _wayPoint[owd.myTarget - 1].position) >
                    Vector3.Distance(_wayPoint[owd.myTarget - 1].position, _wayPoint[owd.myTarget].position)) && safetyPoint < 10)
            {

                // store amount of distance off object is from current target
                float offsetDistance = Vector3.Distance(_object.position, _wayPoint[owd.myTarget].position);

                // dir = target.pos - start.pos
                Vector3 direction = _wayPoint[owd.myTarget + 1].position - _wayPoint[owd.myTarget].position;


                // set new position
                _object.position = _wayPoint[owd.myTarget].position + (Vector3.Normalize(direction) * offsetDistance);
                owd.myTarget++;

                // on the off chance we have an endless while loop
                safetyPoint++;
            }

        } // end foreach loop
    } // end return to start function
}
