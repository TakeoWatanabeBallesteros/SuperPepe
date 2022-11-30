using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBlock : MonoBehaviour,IReset
{
    [SerializeField] GameObject[] items;
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
            StartCoroutine(SpawnItem(items[Random.Range(0,items.Length)],other.transform));
            blockRenderer.material = blockOffMaterial;
            picked = true;
        }
    }
    IEnumerator SpawnItem(GameObject prefab,Transform _player)
    {
        GameObject coin = Instantiate(prefab,transform.position,Quaternion.identity);
        Item item = coin.GetComponent<Item>();
        item.Spawned();
        item.SetCollected(true);

        while(coin.transform.position.y < transform.position.y + riseDistance)
        {
            coin.transform.position += new Vector3(0,riseSpeed*Time.deltaTime,0);
            yield return null;
        }
        coin.transform.position = new Vector3(coin.transform.position.x,transform.position.y + riseDistance,coin.transform.position.z);
        item.SetCollected(false);
        item.Collect(_player);
    }
    public void Reset()
    {
        StopAllCoroutines();
        blockRenderer.material = blockOnMaterial;
        picked = false;
    }
}
