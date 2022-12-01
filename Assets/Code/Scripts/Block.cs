using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour,IReset
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
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && currentUses > 0)
        {
            Instantiate(item,spawnPoint.position,Quaternion.identity);
            anim.SetTrigger("Pop");
            currentUses--;
            if(currentUses<=0) blockRenderer.material = blockOffMaterial;
        }
    }
    public void Reset()
    {
        blockRenderer.material = blockOnMaterial;
        currentUses = uses;
    }
}
