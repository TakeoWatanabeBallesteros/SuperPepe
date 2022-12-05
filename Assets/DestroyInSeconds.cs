using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour,IReset
{
    [SerializeField] float timeToDestroy;
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
        StartCoroutine(DestroySec());
    }
    IEnumerator DestroySec()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(timeToDestroy);
        Reset();
    }
    public void SetTimeToDestroy(float _time)
    {
        timeToDestroy = _time;
    }
    public void Reset()
    {
        GameManager.GetGameManager().RemoveResetObject(this);
        Destroy(gameObject);
    }
}
