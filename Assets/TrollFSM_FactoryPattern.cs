using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrollFSM;

public class TrollFSM_FactoryPattern : MonoBehaviour
{
    public StateMachine stateMachine;


    ///
    public GameObject enemy;
    public float maxSpeed = 2;//2m/s 

    public float FOVinDEG = 70;
    private float cosOfFOVover2InRAD;  //cut off value for visibility checks

    public Transform[] waypoints;
    public int nextWaypointIndex = 0;

    public float maxAngularSpeedInDegPerSec = 60; //deg/sec
    public float maxAngularSpeedInRadPerSec; //rad/sec
    private float maxAngularSpeedInRadPerFrame;

    ///


    // Start is called before the first frame update
    void Start()
    {
        cosOfFOVover2InRAD = Mathf.Cos(FOVinDEG / 2f * Mathf.Deg2Rad);
        maxAngularSpeedInRadPerSec = maxAngularSpeedInDegPerSec * Mathf.Deg2Rad;

        stateMachine = new StateMachine();
        StateMachine.State realignWayPoint = stateMachine.CreateState("RealignWayPoint");
        realignWayPoint.onEnter = delegate{ Debug.Log("realignWayPoint.onEnter"); };
        realignWayPoint.onExit = delegate { Debug.Log("realignWayPoint.onExit"); };
        realignWayPoint.onStay = delegate
        {
            Debug.Log("realignWayPoint.onStay");
            DoRealignWaypoint();
        };

        StateMachine.State seekWayPoint = stateMachine.CreateState("SeekWayPoint");
        seekWayPoint.onEnter = delegate { Debug.Log("seekWayPoint.onEnter"); };
        seekWayPoint.onExit = delegate { Debug.Log("seekWayPoint.onExit"); };
        seekWayPoint.onStay = delegate
        {
            Debug.Log("seekWayPoint.onStay");
            //DEFAULT ACTION
            print("HandleSeekWaypoint");
            DoSeekWaypoint();

            //CHECK FOR  TRANSITIONS
            //T4 - Waypoint Reached?
            if (Utilities.WaypointReached(this.transform.position, waypoints[nextWaypointIndex].position))
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % waypoints.Length;
                stateMachine.ChangeState("RealignWaypoint");
            }
            //T2 - SeeEnemy?
            if (Utilities.SeeEnemy(this.transform.position,enemy.transform.position,this.transform.forward,cosOfFOVover2InRAD))
            {
                stateMachine.ChangeState("ChaseEnemy");
            }

        };

        StateMachine.State chaseEnemy = stateMachine.CreateState("ChaseEnemy");
        chaseEnemy.onEnter = delegate { Debug.Log("chaseEnemy.onEnter"); };
        chaseEnemy.onExit = delegate { Debug.Log("chaseEnemy.onExit"); };
        chaseEnemy.onStay = delegate { Debug.Log("chaseEnemy.onStay"); };

        StateMachine.State fightEnemy = stateMachine.CreateState("FightEnemy");
        fightEnemy.onEnter = delegate { Debug.Log("fightEnemy.onEnter"); };
        fightEnemy.onExit = delegate { Debug.Log("fightEnemy.onExit"); };
        fightEnemy.onStay = delegate { Debug.Log("fightEnemy.onStay"); };


    }

    private void DoRealignWaypoint()
    {
        //DEFAULT
        print("DoRealignWaypoint");
        DoRealign();

        //TRANSITIONS
        //T1 - Aligned?
        if (Utilities.IsAligned(this.transform.position,enemy.transform.position, this.transform.forward,0.01f))
        {
            print("IsAligned.............");
            stateMachine.ChangeState("SeekWaypoint");
        }
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }



    private void DoRealign()
    {


        //int i1 = (nextWaypointIndex + 1) % waypoints.Length;
        Vector3 headingToNextWP = waypoints[nextWaypointIndex].position - this.transform.position;
        headingToNextWP.Normalize();
        maxAngularSpeedInRadPerFrame = maxAngularSpeedInRadPerSec * Time.deltaTime;
        //Vector3 fwdWorld = this.transform.TransformVector(this.transform.forward);
        Vector3 newRotation = Vector3.RotateTowards(this.transform.forward, headingToNextWP, maxAngularSpeedInRadPerFrame, 0);
        this.transform.rotation = Quaternion.LookRotation(newRotation);
    }

    private bool IsAligned()
    {

        //int i1 = (nextWaypointIndex + 1) % waypoints.Length;
        Vector3 headingToNextWP = waypoints[nextWaypointIndex].position - this.transform.position;
        headingToNextWP.Normalize();
        float diff = Vector3.Distance(headingToNextWP, this.transform.forward);
        if (diff < 0.01)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DoSeekWaypoint()
    {

        this.transform.position = Vector3.MoveTowards(this.transform.position, waypoints[nextWaypointIndex].position, maxSpeed * Time.deltaTime);
    }

    private bool WaypointReached()
    {

        if (Vector3.Distance(this.transform.position, waypoints[nextWaypointIndex].position) < float.Epsilon)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool SeeEnemy()
    {

        Vector3 T2Eheading = enemy.transform.position - this.transform.position;
        T2Eheading.Normalize();
        float cosTheta = Vector3.Dot(this.transform.forward, T2Eheading);
        return (cosTheta > cosOfFOVover2InRAD);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            int i1 = (i + 1) % waypoints.Length;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i1].position);


        }

        Gizmos.color = Color.cyan;
        Vector3 from = this.transform.position;
        Vector3 to = this.transform.position + this.transform.forward * 10;
        //Vector3 fovMinus=Vector3.RotateTowards(fro)
        Gizmos.DrawLine(from, to);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(waypoints[nextWaypointIndex].position, .5f);

        /*if (stateMachine.currentState.Name == "ChaseEnemy")
        {
            Gizmos.DrawSphere(enemy.transform.position,.5f);
        }*/

    }
}
