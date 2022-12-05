using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] int uses;
    int currentUses;
    [SerializeField] GameObject item;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Renderer blockRenderer;
    [SerializeField] Material blockOnMaterial;
    [SerializeField] Material blockOffMaterial;
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
        }
    }
}
