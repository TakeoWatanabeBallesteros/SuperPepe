using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStar : Item
{
    [SerializeField] int healAmount;
    public override void ItemExecution(Transform _player)
    {
        _player.GetComponent<HealthSystem>().Heal(healAmount);
    }
    public override bool ItemCondition(Transform _player)
    {
        return _player.GetComponent<HealthSystem>().CanHeal();
    }
    public void Spawned()
    {
        base.SetSpawned();
    }
}
