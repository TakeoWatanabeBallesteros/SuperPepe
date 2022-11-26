using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour, IReset
{
    [SerializeField] int checkpointNumber;

    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        CheckpointManager.instance.SetCheckpoint(transform, checkpointNumber);
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        gameObject.SetActive(true);
    }
}