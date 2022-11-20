using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairClimb : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private GameObject stepRayUpper;
    [SerializeField] private GameObject stepRayLower;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 2f;

    private void Awake()
    {
        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.localPosition.x, stepHeight, stepRayUpper.transform.localPosition.z);
    }
    
    private void FixedUpdate()
    {
        if (rigidBody.velocity == Vector3.zero) return;
        stepClimb();
    }

    void stepClimb()
    {
        // directions that we need to cast rays to ensure we can climb stair from any angle
        Vector3[] directions = new Vector3[]
        {
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f)
        };

        // if the bottom raycast collides but the top doesn't then bounce us up over the step
        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(direction), 0.2f))
            {
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(direction), 0.3f))
                {
                    rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }
        }
    }
}