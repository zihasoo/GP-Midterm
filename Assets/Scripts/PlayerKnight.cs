using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnight : Player
{
    public GameObject blockEffectPrefab;
    public GameObject blockEndEffectPrefab;
    public float blockDuration;
    public float blockCoolTime;

    bool isBlocking;
    bool readyBlock;
    GameObject blockEffectInstance;
    IEnumerator blockingRoutine;

    private new void Start()
    {
        base.Start();
        isBlocking = false;
        readyBlock = true;
        animeParameters.Add("Block");
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

        if (Input.GetKeyDown(skillKey) && readyBlock)
        {
            readyBlock = false;
            isBlocking = true;
            animator.SetTrigger("StartBlock");
            SetParameterOnlyTrue("Block");
            blockEffectInstance = Instantiate(blockEffectPrefab, transform);
            blockingRoutine = BlockingRoutine();
            StartCoroutine(blockingRoutine);
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

    protected new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if (isBlocking)
            {
                var pos = transform.position;
                pos.y += 0.6f;
                Instantiate(blockEndEffectPrefab, pos, Quaternion.identity);
                HitInterrupt();
            }
            else
            {
                base.OnTriggerEnter2D(collision);
            }
        }
    }


    IEnumerator BlockingRoutine()
    {
        yield return new WaitForSeconds(blockDuration);
        EndBlock();
    }

    IEnumerator BlockCoolDown()
    {
        yield return new WaitForSeconds(blockCoolTime);
        readyBlock = true;
    }

    protected override void HitInterrupt()
    {
        if (blockingRoutine != null)
            StopCoroutine(blockingRoutine);
        EndBlock();
    }

    private void EndBlock()
    {
        Destroy(blockEffectInstance);
        isBlocking = false;
        StartCoroutine(BlockCoolDown());
    }

    protected new void SetParameterOnlyTrue(string param)
    {
        if (isBlocking && param != "Block") return;
        foreach (var p in animeParameters)
        {
            if (p == param) animator.SetBool(p, true);
            else animator.SetBool(p, false);
        }
    }
}
