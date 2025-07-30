using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private Rigidbody2D rigiBody;
    private bool lookingRight = true;
    private Animator animator;

    private void Start()
    {
        rigiBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Movement();
    }


    void Movement()
    {   
        float inputMovementX = Input.GetAxis("Horizontal");
        float inputMovementY = Input.GetAxis("Vertical");

        if (inputMovementX != 0f || inputMovementY != 0f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        rigiBody.velocity = new Vector2(inputMovementX * speed, inputMovementY * speed);

        flipPlayer(inputMovementX);
    }

    void flipPlayer(float inputMovementX)
    {
        if ((lookingRight == true && inputMovementX < 0) || (lookingRight == false && inputMovementX > 0))
        {
            lookingRight = !lookingRight;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

    }

}