using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Item : MonoBehaviour,IReset
{
    Animator anim;
    [SerializeField] bool spawned;
    Vector3 initPos;
    bool collected = false;
    [SerializeField] EventReference thisItemSoundEvent;
    [SerializeField] private EventReference itemSpawnSoundEvent;
    private void Awake() {
        anim = GetComponent<Animator>();
        anim.SetBool("Spawned",spawned);
        if(spawned) RuntimeManager.PlayOneShot(itemSpawnSoundEvent, transform.position);
    }
    private void Start() {
        initPos = transform.position;
        GameManager.GetGameManager().AddResetObject(this);
    }
    public void Collect(Transform _player)
    {
        anim.SetTrigger("Collect");
        collected = true;
        StartCoroutine(FlyToPlayer(_player));
        RuntimeManager.PlayOneShot(thisItemSoundEvent, transform.position);
    }
    public virtual bool ItemCondition(Transform _player){return true;}
    public virtual void ItemExecution(Transform _player){}
    IEnumerator FlyToPlayer(Transform _player)
    {
        float _offset = 0.5f;
        float cooldown = 0.5f;
        float timer = 0f;
        yield return new WaitForSeconds(0.5f);
        while(timer < cooldown)
        {
            transform.position = Vector3.Lerp(initPos,_player.position+new Vector3(0,_offset,0),timer/cooldown);
            transform.localScale = Vector3.Lerp(Vector3.one,Vector3.zero,timer/cooldown);
            timer = Mathf.Clamp(timer+Time.deltaTime,0,cooldown);
            yield return null;
        }
        ItemExecution(_player); 
        gameObject.SetActive(false);
    }
    public void Reset()
    {
        if (!spawned && !collected) return;
        GameManager.GetGameManager().RemoveResetObject(this);
        Destroy(gameObject);
    }
    public void SetSpawned(bool _spawned)
    {
        spawned = _spawned;
    }
    public void SetCollected(bool _collected)
    {
        collected = _collected;
    }
    private void OnTriggerEnter(Collider other) {
        if(collected) return;
        if(!ItemCondition(other.transform)) return;
        Collect(other.transform);
    }
}
