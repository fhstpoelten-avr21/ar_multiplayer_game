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
    void Start()
    {
        ball = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        render.sharedMaterial = materials[0];
        Freeze();
    }
    void Update()
    {
        //just for testing can be removed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int index = Array.IndexOf(materials, render.sharedMaterial);
            if (index == 1)
            {
                ChangeColor(0);
            }
            else
            {
                ChangeColor(1);
            }
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
    }

    public void Unfreeze()
    {
        ball.isKinematic = false;
    }

    
    void ToggleVisibility()
    {
        if (render != null)
        {
            render.enabled = !render.enabled;
        }
    }

    
}
