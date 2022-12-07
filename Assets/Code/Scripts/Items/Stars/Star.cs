using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Star : MonoBehaviour
{
    [SerializeField] EventReference thisItemSoundEvent;
    [SerializeField] private EventReference itemSpawnSoundEvent;
    [SerializeField] bool spawned;
    Animator anim;
    private void Awake() {
        anim = GetComponent<Animator>();
        anim.SetBool("Spawned",spawned);
        if(spawned) RuntimeManager.PlayOneShot(itemSpawnSoundEvent, transform.position);
    }
    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;
        RuntimeManager.PlayOneShot(thisItemSoundEvent, transform.position);
        anim.SetBool("Collected",true);
    }
    public void SetSpawned(bool _spawned)
    {
        spawned = _spawned;
    }
    public void Collect()
    {
        //called through animation event
        StarsManager.instance.AddStar();
        gameObject.SetActive(false);
    }
}
