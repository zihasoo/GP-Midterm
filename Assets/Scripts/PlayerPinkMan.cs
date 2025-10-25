using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPinkMan : Player
{
    public GameObject bigJumpEffectPrefab;
    public GameObject bigJumpActiveEffectPrefab;
    public float bigJumpDuration;
    public float bigJumpCoolTime;

    bool readyBigJump;
    bool activeBigJump;
    GameObject bigJumpActiveEffectInstance;
    IEnumerator bigJumpActiveRoutine;

    private new void Start()
    {
        base.Start();
        readyBigJump = true;
        activeBigJump = false;
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

        if (Input.GetKeyDown(skillKey) && readyBigJump)
        {
            bigJumpActiveEffectInstance = Instantiate(bigJumpActiveEffectPrefab, transform);
            activeBigJump = true;
            readyBigJump = false;
            bigJumpActiveRoutine = BigJumpActiveRoutine();
            StartCoroutine(bigJumpActiveRoutine);
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            isGrounded = false;
            vy = jumpForce;
            if (activeBigJump)
            {
                var obj = Instantiate(bigJumpEffectPrefab, transform.position, Quaternion.identity);
                Destroy(obj, 0.5f);
                vy *= 1.5f;
                StopBigJumpActiveRoutine();
            }
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
            SetParameterOnlyTrue("Run");
        else
            SetParameterOnlyTrue("IDLE");
    }

    IEnumerator BigJumpActiveRoutine()
    {
        yield return new WaitForSeconds(bigJumpDuration);
        activeBigJump = false;
        Destroy(bigJumpActiveEffectInstance);
        StartCoroutine(BigJumpCoolDown());
    }

    IEnumerator BigJumpCoolDown()
    {
        yield return new WaitForSeconds(bigJumpCoolTime);
        readyBigJump = true;
    }

    void StopBigJumpActiveRoutine()
    {
        activeBigJump = false;
        Destroy(bigJumpActiveEffectInstance);
        StopCoroutine(bigJumpActiveRoutine);
        StartCoroutine(BigJumpCoolDown());
    }
}
