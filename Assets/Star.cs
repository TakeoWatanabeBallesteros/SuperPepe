using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour,IReset
{
    [SerializeField] bool spawned;
    Animator anim;
    private void Awake() {
        anim = GetComponent<Animator>();
        anim.SetBool("Spawned",spawned);
    }
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
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
        StarsManager.instance.AddStar();
        gameObject.SetActive(false);
    }
    public void Reset()
    {
        anim.SetBool("Collected",false);
        anim.Play("Idle");
        gameObject.SetActive(true);
    }
}
