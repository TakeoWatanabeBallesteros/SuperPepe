using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour
{
    [SerializeField] int checkpointNumber;
    [SerializeField] Animator anim;
    [SerializeField] Renderer flagRenderer;
    [SerializeField] Material marioFlagMaterial;
    [SerializeField] Material bowserFlagMaterial;
    [SerializeField] Transform spawnPoint;
    private bool captured = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || captured) return;
        captured = true;
        CheckpointManager.instance.SetCheckpoint(spawnPoint, checkpointNumber);
        anim.SetBool("Captured", captured);
        flagRenderer.material = marioFlagMaterial;
        gameObject.SetActive(false);
    }
}