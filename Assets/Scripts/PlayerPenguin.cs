using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenguin : Player
{
    public float slideDuration;
    public float slideCoolTime;

    bool isSliding;
    bool readySlide;
    IEnumerator slidingRoutine;

    private new void Start()
    {
        base.Start();
        isSliding = false;
        readySlide = true;
        animeParameters.Add("Slide");
    }

    private void Update()
    {
        if (isSliding) return;
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

        if (Input.GetKeyDown(skillKey) && readySlide && moveInput != 0)
        {
            vx *= 2f;
            rb.velocity = new Vector2(vx, vy);
            isSliding = true;
            readySlide = false;
            animator.SetTrigger("StartSlide");
            SetParameterOnlyTrue("Slide");
            slidingRoutine = SlidingRoutine();
            StartCoroutine(slidingRoutine);
            return;
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

        rb.velocity = new Vector2(vx, vy);

        if (!isGrounded) return;

        if (moveInput != 0)
        {
            SetParameterOnlyTrue("Run");
        }
        else
        {
            SetParameterOnlyTrue("IDLE");
        }
    }

    IEnumerator SlidingRoutine()
    {
        yield return new WaitForSeconds(slideDuration * 0.7f);
        rb.velocityX = rb.velocity.x * 0.3f;
        yield return new WaitForSeconds(slideDuration * 0.3f);
        isSliding = false;
        animator.SetBool("Slide", false);
        StartCoroutine(SlideCoolDown());
    }

    IEnumerator SlideCoolDown()
    {
        yield return new WaitForSeconds(slideCoolTime);
        readySlide = true;
    }

    protected override void HitInterrupt()
    {
        if (slidingRoutine != null)
            StopCoroutine(slidingRoutine);
        isSliding = false;
        StartCoroutine(SlideCoolDown());
    }
}
