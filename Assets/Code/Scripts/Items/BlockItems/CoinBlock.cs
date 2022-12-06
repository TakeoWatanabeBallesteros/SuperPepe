using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CoinBlock : MonoBehaviour
{
    [SerializeField] EventReference thisItemSoundEvent;
    private void Start() {
        CoinsManager.instance.AddCoin();
        RuntimeManager.PlayOneShot(thisItemSoundEvent, transform.position);
        Destroy(gameObject,2f);
    }
}
