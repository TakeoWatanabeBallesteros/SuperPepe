using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour, IReset
{
    [SerializeField] private int checkpointNumber;
    private bool taken = false;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.GetGameManager().SetCheckpoint(transform, checkpointNumber);
        gameObject.SetActive(false);
        taken = true;
    }

    public void Reset()
    {
        gameObject.SetActive(!taken);
    }
}