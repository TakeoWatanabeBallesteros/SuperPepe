using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour, IReset
{
    [SerializeField] int checkpointNumber;
    [SerializeField] Animator anim;
    [SerializeField] Renderer flagRenderer;
    [SerializeField] Material marioFlagMaterial;
    [SerializeField] Material bowserFlagMaterial;

    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        CheckpointManager.instance.SetCheckpoint(transform, checkpointNumber);
        anim.SetBool("Captured",true);
        flagRenderer.material = marioFlagMaterial; 
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        anim.SetBool("Captured",false);
        anim.Play("Idle");
        flagRenderer.material = bowserFlagMaterial;
    }
}