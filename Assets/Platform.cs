using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] LayerMask[] attachableLayers;
    List<Transform> attachedObjects = new List<Transform>();
    private void OnTriggerEnter(Collider other) {
        if(!FacingUpwards()) return;
        if(!IsAttachable(other.gameObject.layer)) return;
        if(!ObjectAttached(other.transform)) AttachObject(other.transform);
    }
    private void OnTriggerExit(Collider other) {
        if(ObjectAttached(other.transform)) DettachObject(other.transform);
    }
    private void OnTriggerStay(Collider other) {
        if(FacingUpwards()) return;
        if(ObjectAttached(other.transform)) DettachObject(other.transform);
    }
    void AttachObject(Transform target)
    {
        attachedObjects.Add(target);
        target.SetParent(transform);
    }
    void DettachObject(Transform target)
    {
        attachedObjects.Remove(target);
        target.SetParent(null);
    }
    bool FacingUpwards()
    {
        return Vector3.Dot(transform.forward,Vector3.up) < 0.95f;
    }
    bool IsAttachable(LayerMask objectLayer)
    {
        foreach (LayerMask layer in attachableLayers)
        {
            if(objectLayer == layer) return true;
        }
        return false;
    }
    bool ObjectAttached(Transform target)
    {
        return attachedObjects.Contains(target);
    }
}