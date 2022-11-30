using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCoin : Item
{
    public override void ItemExecution(Transform _player)
    {
        CoinsManager.instance.AddCoin();
    }
    public void Spawned()
    {
        base.SetSpawned();
    }
}
