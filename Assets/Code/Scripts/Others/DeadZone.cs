using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] GameObject fireParticles;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ITakeDamage>(out var damageComp))
        {
            damageComp.TakeDamage(1000);
            GameObject fire = Instantiate(fireParticles,other.transform.position,Quaternion.identity);
            fire.transform.SetParent(other.transform);
            Destroy(fire, 1.5f);
        }
    }
}
