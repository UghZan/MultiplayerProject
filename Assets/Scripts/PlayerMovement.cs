using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Player controls depending on camera angle
//Uses Input System and Character Controller
public class PlayerMovement : NetworkBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;
    public Transform playerModel; //the girl model

    [Header("Movement Settings")]
    public float maxSpeed;
    public float maxAcceleration;
    public float rotationSpeed;

    [Header("Misc")]
    [SerializeField] Camera cam;
    [SerializeField] CharacterController cc;

    Vector3 movementDirection, //movement direction from input
            movementVector; //final direction
    bool currentlyMoving;
    float speed;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        //endabling player input module here so that input is coming from local player only
        GetComponent<PlayerInput>().enabled = true;
        //enabling camera and it's controller for local player
        cam.gameObject.SetActive(true);
        GetComponent<CameraController>().enabled = true;
    }
    void Update()
    {
        //only local player should control character
        if (!isLocalPlayer) return;
            speed = Mathf.Lerp(speed, maxSpeed * (currentlyMoving ? 1 : 0), maxAcceleration * Time.deltaTime); //instead of instantly going to max speed, let character have a bit of inertia
            animator.SetFloat("speed", speed); //updating animator parameters
            if (currentlyMoving)
            {
                UpdateMovementVector();
                RotatePlayer();
            }
            cc.Move(movementVector * speed * Time.deltaTime);
    }

    public void UpdateMovementVector()
    {
        //we only want to update character rotation when movement buttons are pressed
        //otherwise it would reset as soon as we release movement button
        //there could be a better way to do that, but this'll do
        Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z); //project cam forward direction to floor
        movementVector = movementDirection.y * camForward + movementDirection.x * cam.transform.right; // get a movement direction related to camera forward direction
    }

    public void RotatePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(movementVector.normalized); //get a forward rotation
        playerModel.rotation = Quaternion.Lerp(playerModel.rotation, targetRotation, Time.deltaTime * rotationSpeed); //smoothly rotate towards destination
    }

    public void GetMovementVector(InputAction.CallbackContext ctx)
    {
        //on button press/joystick movement remember the movement direction
        if (ctx.phase == InputActionPhase.Performed)
        {
            currentlyMoving = true;
            movementDirection = ctx.ReadValue<Vector2>();
        }
        else if (ctx.phase == InputActionPhase.Canceled) //when the action ends (button is released), reset the value
        {
            currentlyMoving = false;
            movementDirection = Vector2.zero;
        }

    }
}
