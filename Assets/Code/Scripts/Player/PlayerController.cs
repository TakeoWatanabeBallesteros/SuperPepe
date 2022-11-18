using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Camera m_Camera;
    [Space] [Header("Settings")]
    [SerializeField] private float m_LerpRotationPct;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;

    private int SpeedID;

    private void Awake()
    {
        SpeedID = Animator.StringToHash("Speed");
    }
    void Start()
    {

    }

    void Update()
    {
        float l_Speed = 0.0f;

        Vector3 l_ForwardCamera = m_Camera.transform.forward;
        Vector3 l_RightCamera = m_Camera.transform.right;
        l_ForwardCamera.y = 0.0f;
        l_RightCamera.y = 0.0f;
        l_ForwardCamera.Normalize();
        l_RightCamera.Normalize();
        bool l_HasMovement = false;

        Vector3 l_Movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            l_Movement = l_ForwardCamera;
            l_HasMovement = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            l_Movement = -l_ForwardCamera;
            l_HasMovement = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            l_Movement -= l_RightCamera;
            l_HasMovement = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            l_Movement += l_RightCamera;
            l_HasMovement = true;
        }
        l_Movement.Normalize();

        float l_MovementSpeed = 0.0f;
        if (l_HasMovement)
        {
            Quaternion l_LookRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_LookRotation, m_LerpRotationPct);

            l_Speed = 0.05f;
            l_MovementSpeed = m_WalkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                l_Speed = 1.0f;
                l_MovementSpeed = m_RunSpeed;
            }
        }

        animator.SetFloat("speed", l_Speed);
        l_Movement = l_Movement * l_MovementSpeed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
            animator.SetTrigger("Puntch");
        
    }
}
