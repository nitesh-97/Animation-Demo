using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationMoveController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    Vector2 CurrentMovementInput;
    Vector3 currentMovement;
    bool ismovementpressed;
    float rotationFactorPerFrame = 2f;

    private void Awake()
    {
        playerInput = new PlayerInput();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
    }
    void OnMovementInput(InputAction.CallbackContext context)
    {
        CurrentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = CurrentMovementInput.x;
        currentMovement.z = CurrentMovementInput.y;
        ismovementpressed = (CurrentMovementInput.x != 0 || CurrentMovementInput.y != 0);
    }
    void HandleAnimation()
    {
        bool IsWalking = animator.GetBool("IsWalking");
        if (ismovementpressed && !IsWalking)
        {
           // animator.SetBool("Isidle", false);
            animator.SetBool("IsWalking", true);
        }
        else if(!ismovementpressed && IsWalking)
        {
            //animator.SetBool("Isidle", true);
            animator.SetBool("IsWalking", false);
        }
    }
    void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -.85f;
            currentMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity;
        }
    }
    void HandleRotation()
    {
        Vector3 positiontolookat;
        positiontolookat.x = currentMovement.x;
        positiontolookat.y = 0.0f;
        positiontolookat.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation ;
        if (ismovementpressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positiontolookat);
           transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame*Time.deltaTime);
        }
        
    }

   
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        HandleGravity();
        HandleRotation();
        HandleAnimation();
        
        characterController.Move(currentMovement*Time.deltaTime); 
        
    }
}
