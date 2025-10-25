using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject hitPrefab;
    public int maxHP;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    protected Animator animator;
    protected Rigidbody2D rb;
    protected SpriteRenderer spRenderer;
    protected int HP;
    protected bool isGrounded;
    protected bool isFalling;

    protected const KeyCode jumpKey = KeyCode.UpArrow;
    protected const KeyCode leftMoveKey = KeyCode.LeftArrow;
    protected const KeyCode rightMoveKey = KeyCode.RightArrow;
    protected const KeyCode skillKey = KeyCode.Space;

    protected List<string> animeParameters = new()
    {
        "IDLE", "Run", "Jump", "Falling"
    };

    protected void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spRenderer = GetComponent<SpriteRenderer>();
        HP = maxHP;
        isGrounded = false;
        isFalling = false;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            HP--;
            PlayHitEffect();
            animator.SetTrigger("Hit");
            HitInterrupt();
            //uiManager.SetHP(HP);

            //if (HP <= 0)
            //{
            //    isOver = true;
            //    gameObject.SetActive(false);
            //    uiManager.GameOver();
            //}
        }
        //else if (collision.gameObject.CompareTag("Tram"))
        //{
        //    rb.velocityY = jumpForce * 1.5f;
        //    var tramAnimator = collision.gameObject.GetComponent<Animator>();
        //    tramAnimator.SetTrigger("IsJump");
        //    isFalling = false;
        //    SetParameterOnlyTrue("IsJump");
        //    jumpCount = maxJumpCount - 1;
        //}
        //else if (collision.gameObject.CompareTag("Item"))
        //{
        //    int hpPlus = 1;
        //    itemCount++;
        //    if (collision.name == "Banana")
        //    {
        //        hpPlus = 2;
        //    }
        //    if (HP < maxHP)
        //    {
        //        HP = Mathf.Min(maxHP, HP + hpPlus);
        //        uiManager.SetHP(HP);
        //    }
        //    var itemAnimator = collision.gameObject.GetComponent<Animator>();
        //    itemAnimator.SetTrigger("Collected");
        //    Destroy(collision.gameObject, 0.5f);
        //}
        //else if (collision.gameObject.CompareTag("End"))
        //{
        //    uiManager.GameClear(itemCount, hitCount);
        //    isOver = true;
        //    rb.velocity = Vector2.zero;
        //}
    }

    protected virtual void HitInterrupt()
    {
        
    }

    private void PlayHitEffect()
    {
        var obj = Instantiate(hitPrefab, transform.position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }

    protected void SetParameterOnlyTrue(string param)
    {
        foreach (var p in animeParameters)
        {
            if (p == param) animator.SetBool(p, true);
            else animator.SetBool(p, false);
        }
    }
}

