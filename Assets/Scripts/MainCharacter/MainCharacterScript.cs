using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacterScript : Entity
{
    private Vector2 input;

    private PlayerInputAction playerInputAction;
    private PlayerInput playerInput;
    

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();

        playerInputAction.Player.Enable();
        playerInputAction.Player.Attack.performed += Attack;
    }



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerInput = GetComponent<PlayerInput>();

    }


    // Update is called once per frame
    void Update()
    {
        input = playerInputAction.Player.Move.ReadValue<Vector2>();

    }

    private void FixedUpdate()
    {
        UpdateMovement();
        Debug.Log(input);
    }

    private void UpdateMovement()
    {
        this.rb2D.velocity = new Vector2(input.x * movementSpeed,input.y * movementSpeed);
    }

    private void Attack(InputAction.CallbackContext callback)
    {
        if(callback.performed)
            Debug.Log("Attack!");
    }
}
