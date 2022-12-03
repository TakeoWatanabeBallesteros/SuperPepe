using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGreenMushroom : Item
{
    public override void ItemExecution(Transform _player)
    {
        _player.GetComponent<HealthSystem>().LifeUp();
    }
}
