using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] float force;
    Rigidbody rb;
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AddForceToBridge(-other.transform.up, other.transform.position);
        }
    }

    private void AddForceToBridge(Vector3 direction,Vector3 pos)
    {
        rb.AddForceAtPosition(direction*force,pos);
    }

}
