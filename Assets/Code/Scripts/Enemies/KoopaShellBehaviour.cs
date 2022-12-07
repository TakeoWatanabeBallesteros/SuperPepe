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
            transform.LookAt(-transform.forward);
        }

        characterController.Move(transform.forward * 3 * Time.deltaTime);
    }
}