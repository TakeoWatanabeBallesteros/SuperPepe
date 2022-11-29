using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<ITakeDamage>() != null) other.GetComponent<ITakeDamage>().TakeDamage(100);
    }
}
