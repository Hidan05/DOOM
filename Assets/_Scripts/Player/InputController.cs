using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    //accion de movimiento
    InputAction moveAction;
    InputAction shootAction;

    [HideInInspector] public Vector2 moveVector;


    void Awake()
    {
      moveAction = InputSystem.actions.FindAction("Move");
      shootAction = InputSystem.actions.FindAction("Attack");
    }
    

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Shoot();
    }

    public void GetInput()
    {
        moveVector = moveAction.ReadValue<Vector2>();
    }

    public void Shoot()
    {
        if (shootAction.WasPressedThisFrame())
        {
            GunController.Instance.Fire();
        }
    }
}
