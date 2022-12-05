using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBigMushroom : Item
{
    [SerializeField] int giantTime;
    public override void ItemExecution(Transform _player)
    {
        _player.GetComponent<GiantScale>().MakeGiantScale(giantTime);
    }
    public override bool ItemCondition(Transform _player)
    {
        return _player.GetComponent<GiantScale>().CanBeGiant();
    }
}
