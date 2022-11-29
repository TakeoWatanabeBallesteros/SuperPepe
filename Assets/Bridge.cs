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
    public void AddForceToBridge(Vector3 direction,Vector3 pos)
    {
        rb.AddForceAtPosition(direction*force,pos);
        Debug.Log("Aaaaa");
    }

}
