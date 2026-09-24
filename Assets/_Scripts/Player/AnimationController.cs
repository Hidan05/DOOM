using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    private Animator animator;
    private InputController inputController;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        inputController = GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isWalking", inputController.moveVector.magnitude > 0.1);
    }
}
