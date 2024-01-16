using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody ball;
    private Renderer render;

    public float throwForce = 5;
    public Material[] materials;
    
    // Speed at which the ball rolls
    public float rollSpeed = 10f;

    // Whether the ball is currently on the ground
    private bool isGrounded;
    
    void Start()
    {
        ball = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        render.sharedMaterial = materials[0];
        //Freeze
    }
    void Update()
    {
        // Check if the ball is on the ground (you might need to adjust the detection method based on your scene)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        // Allow the player to control the ball's movement only when it's on the ground
        if (isGrounded)
        {
            Debug.Log("Ball is grounded");
            ball.useGravity = false;
            ball.isKinematic = true;
        }
    }

    void ChangeColor(int materialIndex)
    {
        render.sharedMaterial = materials[materialIndex];
    }

    void ThrowBallUp()
    {
        Unfreeze();
        ball.AddForce(Vector3.up * throwForce, ForceMode.Impulse);
    }

    public void Freeze()
    {
        ball.isKinematic = true;
        ball.useGravity = false;
    }

    public void Unfreeze()
    {
        ball.isKinematic = false;
        ball.useGravity = true;
    }

    
    void ToggleVisibility()
    {
        if (render != null)
        {
            render.enabled = !render.enabled;
        }
    }

    
}
