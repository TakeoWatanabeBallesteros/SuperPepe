using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Star : MonoBehaviour
{
    [SerializeField] EventReference thisItemSoundEvent;
    [SerializeField] bool spawned;
    Animator anim;
    private void Awake() {
        anim = GetComponent<Animator>();
        anim.SetBool("Spawned",spawned);
    }
    private void OnTriggerEnter(Collider other) {
        anim.SetBool("Collected",true);
    }
    public void SetSpawned(bool _spawned)
    {
        spawned = _spawned;
    }
    public void Collect()
    {
        //called through animation event
        RuntimeManager.PlayOneShot(thisItemSoundEvent, transform.position);
        StarsManager.instance.AddStar();
        gameObject.SetActive(false);
    }
}
