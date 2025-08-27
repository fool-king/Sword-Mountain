using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb;
    Vector2 moveDirection;
    Animator animator;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

  
    private void FixedUpdate()
    {
         //获取玩家输入
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        animator.SetFloat("Horizontal", moveX);
        animator.SetFloat("Vertical", moveY);
        animator.SetFloat("Speed", moveX * moveX + moveY * moveY);
        //计算移动方向
        Vector2 movementDirection = new Vector2(moveX, moveY).normalized;
        //应用移动
        rb.velocity = movementDirection * moveSpeed;
    }
}
