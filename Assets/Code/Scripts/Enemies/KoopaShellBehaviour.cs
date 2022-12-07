using System;
using UnityEngine;


public class KoopaShellBehaviour : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform shellFront;
    private LayerMask mask = ~0;

    public bool free;
    private void Update()
    {
        if(!free) return;
        if (Physics.Raycast(shellFront.position, transform.forward, 0.1f, mask))
        {
            transform.localRotation *= Quaternion.Euler(0, 180, 0);
        }

        characterController.Move(transform.forward * 3 * Time.deltaTime + Vector3.down * 5 * Time.deltaTime);
    }
}