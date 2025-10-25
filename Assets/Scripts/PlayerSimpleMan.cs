using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimpleMan : Player
{
    public GameObject slowFallingEffectPrefab;
    public float slowFallingDuration;
    public float slowFallingCoolTime;
    public float slowFallingSpeed;

    bool readySlowFalling;
    bool activeSlowFalling;
    IEnumerator slowFallingActiveRoutine;

    private new void Start()
    {
        base.Start();
        readySlowFalling = true;
        activeSlowFalling = false;
    }

    private void Update()
    {
        float moveInput = 0f;

        if (Input.GetKey(leftMoveKey))
        {
            moveInput = -1f;
            spRenderer.flipX = true;
        }
        else if (Input.GetKey(rightMoveKey))
        {
            moveInput = 1f;
            spRenderer.flipX = false;
        }
        float vx = moveInput * moveSpeed;
        float vy = rb.velocityY;

        bool groundTest = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) && rb.velocityY <= 1e-4;
        if (!isGrounded && groundTest) //처음으로 땅을 만났을 때
        {
            isGrounded = true;
            isFalling = false;
        }
        if (!groundTest) isGrounded = false;

        if (Input.GetKeyDown(skillKey) && readySlowFalling)
        {
            var obj = Instantiate(slowFallingEffectPrefab, transform);
            Destroy(obj, slowFallingDuration);
            activeSlowFalling = true;
            readySlowFalling = false;
            slowFallingActiveRoutine = SlowFallingActiveRoutine();
            StartCoroutine(slowFallingActiveRoutine);
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            isGrounded = false;
            vy = jumpForce;
            SetParameterOnlyTrue("Jump");
        }

        if (!isFalling && !isGrounded && vy < 0)
        { //처음으로 떨어지기 시작할 때
            isFalling = true;
            SetParameterOnlyTrue("Falling");
        }

        if (activeSlowFalling && vy < -slowFallingSpeed)
        {
            vy = -slowFallingSpeed;
        }

        rb.velocity = new Vector2(vx, vy);

        if (!isGrounded) return;

        if (moveInput != 0)
            SetParameterOnlyTrue("Run");
        else
            SetParameterOnlyTrue("IDLE");
    }

    IEnumerator SlowFallingActiveRoutine()
    {
        yield return new WaitForSeconds(slowFallingDuration);
        activeSlowFalling = false;
        StartCoroutine(SlowFallingCoolDown());
    }

    IEnumerator SlowFallingCoolDown()
    {
        yield return new WaitForSeconds(slowFallingCoolTime);
        readySlowFalling = true;
    }
}
