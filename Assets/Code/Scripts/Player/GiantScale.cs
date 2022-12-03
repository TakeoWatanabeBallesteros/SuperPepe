using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantScale : MonoBehaviour
{
    bool giant = false;
    public void MakeGiantScale(float time)
    {
        StartCoroutine(GiantMode(time));
    }
    IEnumerator GiantMode(float time)
    {
        giant = true;
        //makeGiant
        yield return new WaitForSeconds(time);
        //makeSmallAgain
        giant = false;
    }
    public bool CanBeGiant()
    {
        return !giant;
    }
}
