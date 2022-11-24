using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Transform player;
    public float minBalancingDistance;
    public float balanceSpeed;
    public bool enter;
    public bool exit;
    bool keepEnt;
    bool keepEx;
    public Transform bridge;
    float maxDistance;
    Vector3 initialBridge;
    float repositionSpeed;
    float threshold;
    //exit calculations
    float exitSpeed;
    
    void Start()
    {
        initialBridge = bridge.right;
    }

    // Update is called once per frame
    void Update()
    {
        if(enter)
        {
            keepEnt = true;
            keepEx = false;
            enter = false;
        }
        if(exit)
        {
            keepEx = true;
            keepEnt = false;
            exit = false;
        }

        if(keepEnt) Enter();
        if(keepEx) Exit();
    }

    void Enter()
    {
        Vector3 playerToCenter = player.position - transform.position;
        if(playerToCenter.magnitude > minBalancingDistance)//si se borra este if, el centro no sera una zona estable.
        {
            float speed = Vector3.Dot(playerToCenter.normalized, transform.right) > 0 ? balanceSpeed : -balanceSpeed;
            if(Vector3.Dot(bridge.right,initialBridge) > 0)
            {
                bridge.localEulerAngles += new Vector3(0,speed*playerToCenter.magnitude/4.5f*Time.deltaTime,0);
                float currentYRotation = bridge.localEulerAngles.y < 180 ? bridge.localEulerAngles.y :  bridge.localEulerAngles.y - 360;
                bridge.localEulerAngles = new Vector3(0,Mathf.Clamp(currentYRotation,-45,45));
            }
        }
    }
    void Exit()
    {
        if(Mathf.Abs(bridge.localEulerAngles.y - 0) >= threshold)
        {
            
        }
    }
    void ExitCalculations()
    {
        float currentYRotation = bridge.localEulerAngles.y < 180 ? bridge.localEulerAngles.y :  bridge.localEulerAngles.y - 360;
        exitSpeed = currentYRotation > 0 ? repositionSpeed : -repositionSpeed;
    }
}
