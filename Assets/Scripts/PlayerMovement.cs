using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D playerCollider;


    //Movement
    public float playerSpeed;
    public float LerpingFloat;

    //Input
    private float xInput;
    private float yInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        UpdateInput();
        
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        

    }

    private void UpdateMovement()
    {
        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, xInput*playerSpeed, LerpingFloat), Mathf.Lerp(rb.velocity.y, yInput*playerSpeed, LerpingFloat));
    }

    
}
