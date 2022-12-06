using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Block : MonoBehaviour
{
    [SerializeField] int uses;
    int currentUses;
    [SerializeField] private GameObject item;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Renderer blockRenderer;
    [SerializeField] private Material blockOnMaterial;
    [SerializeField] private Material blockOffMaterial;
    [SerializeField] private EventReference blockSoundEvent;
    Animator anim;

    
    private void Start() {
        anim = GetComponent<Animator>();
        blockRenderer.material = blockOnMaterial;
        currentUses = uses;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && currentUses > 0)
        {
            Instantiate(item,spawnPoint.position,transform.rotation);
            anim.SetTrigger("Pop");
            currentUses--;
            if(currentUses<=0) blockRenderer.material = blockOffMaterial;
            RuntimeManager.PlayOneShot(blockSoundEvent, transform.position);
        }
    }
}
