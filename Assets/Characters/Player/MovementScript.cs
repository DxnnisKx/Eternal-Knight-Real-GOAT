using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;   // Bewegungsgeschwindigkeit

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Horizontale Bewegung
        float moveDirection = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }
}
