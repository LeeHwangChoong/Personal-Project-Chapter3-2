using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;
    public float baseJumpPower;
    private int jumpCount;
    public int maxJumpCount = 2;
    public float staminaCostPerJump = 15f;
    private Vector2 curMonvementInput;
    public LayerMask groundLayerMask;

    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;
        
    public Vector3 cameraOffset = new Vector3(0, 2, -4); 

    private Rigidbody rb;

    public Action inventory;
    Animator anim;
    PlayerCondition condition;

    public GameObject wing;

    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        condition = GetComponent<PlayerCondition>();

        cameraContainer.localPosition = cameraOffset;
        cameraContainer.localRotation = Quaternion.identity;

        baseJumpPower = jumpPower; // 기본 점프 값 초기화
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        Move();
        CheckGrounded();
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMonvementInput.y + transform.right * curMonvementInput.x;
        dir *= moveSpeed;
        dir.y = rb.velocity.y;

        rb.velocity = dir;                
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMonvementInput = context.ReadValue<Vector2>();
            anim.SetBool("IsRun", true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMonvementInput = Vector2.zero;
            anim.SetBool("IsRun", false);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && CanDoubleJump())
        {
            if (condition != null && condition.UseStamina(staminaCostPerJump))
            {
                rb.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                jumpCount++;
                anim.SetBool("IsJump", true);
                isGrounded = false;
                condition.isGrounded = false;
            }
        }
    }

    void CheckGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        bool wasGrounded = isGrounded; // 상태가 변할때만 호출되게
        isGrounded = false;

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.3f, groundLayerMask))
            {
                isGrounded = true;
                break;
            }
        }

        if (isGrounded != wasGrounded)
        {
            if (isGrounded)
            {
                Debug.Log("땅이다");
                condition.isGrounded = true; // 착지 상태를 true로 설정
                anim.SetBool("IsJump", false);
            }
            else
            {
                condition.isGrounded = false; // 착지 상태를 false로 설정
            }
        }
    }

    bool CanDoubleJump()
    {
        if (isGrounded)
        {
            jumpCount = 0; 
            return true;
        }
        return jumpCount < maxJumpCount;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;        
        canLook = !toggle;        
    }
}
