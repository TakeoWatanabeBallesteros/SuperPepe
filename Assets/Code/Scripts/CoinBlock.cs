using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBlock : MonoBehaviour,IReset
{
    [SerializeField] GameObject item;
    [SerializeField] float riseDistance;
    [SerializeField] float riseSpeed;
    [SerializeField] Renderer blockRenderer;
    [SerializeField] Material blockOnMaterial;
    [SerializeField] Material blockOffMaterial;
    bool picked = false;
    private void Start() {
        blockRenderer.material = blockOnMaterial;
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && !picked)
        {
            Instantiate(item,transform.position,Quaternion.identity);
            blockRenderer.material = blockOffMaterial;
            picked = true;
        }
    }
    public void Reset()
    {
        blockRenderer.material = blockOnMaterial;
        picked = false;
    }
}
