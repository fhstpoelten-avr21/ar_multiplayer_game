using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody ball;
    private Vector3 initialPosition;
    public float throwForce = 5;
    void Start()
    {
        ball = GetComponent<Rigidbody>();
        ThrowBallUp();
    }
    void Update()
    {

    }

    void ThrowBallUp()
    {
        ball.isKinematic = false;
        ball.AddForce(Vector3.up * throwForce, ForceMode.Impulse);
    }

    void Freeze()
    {
        ball.isKinematic = true;
    }

    void Unfreeze()
    {
        ball.isKinematic = false;
    }

    
    void ToggleVisibility()
    {
        Renderer render = GetComponent<Renderer>();
        if (render != null)
        {
            render.enabled = !render.enabled;
        }
    }

    
}
