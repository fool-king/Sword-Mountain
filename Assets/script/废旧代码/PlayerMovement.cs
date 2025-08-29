// PlayerMovement.cs
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb;
    public Animator animator;
    private bool canMove = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!canMove) 
        {
            rb.velocity = Vector2.zero; // 立即停止移动
            animator.SetFloat("Speed", 0); // 停止动画
            return;
        }
        
        //获取玩家输入
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        
        // 更新动画参数
        animator.SetFloat("Horizontal", moveX);
        animator.SetFloat("Vertical", moveY);
        animator.SetFloat("Speed", new Vector2(moveX, moveY).sqrMagnitude);
        
        //计算移动方向并应用
        Vector2 movementDirection = new Vector2(moveX, moveY).normalized;
        rb.velocity = movementDirection * moveSpeed;
    }

    public void SetCanMove(bool allowMovement)
    {
        canMove = allowMovement;
        if (!allowMovement)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
    }
}