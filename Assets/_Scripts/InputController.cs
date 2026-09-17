using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    //accion de movimiento
    InputAction moveAction;

    [HideInInspector] public Vector2 moveVector;


    void Awake()
    {
      moveAction = InputSystem.actions.FindAction("Move");
    }
    

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    public void GetInput()
    {
        moveVector = moveAction.ReadValue<Vector2>();
    }
}
