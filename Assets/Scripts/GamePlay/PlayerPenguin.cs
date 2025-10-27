using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenguin : Player
{

    protected new void Awake()
    {
        playerUI.Init("Penguin", maxHP);
        if (!animeParameters.Contains("Slide"))
            animeParameters.Add("Slide");
        base.Awake();
    }

    protected new void Update()
    {
        if (transform.position.y < -10f)
        {
            uiManager.OnDie();
            return;
        }
        if (isSkilling) return;

        float moveInput = ReadMoveInput();

        float vx = moveInput * moveSpeed;
        float vy = rb.velocityY;

        bool groundTest = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) && vy <= 1e-4f;
        if (!isGrounded && groundTest)
        {
            isGrounded = true;
            isFalling = false;
        }
        if (!groundTest)
            isGrounded = false;

        if (Input.GetKeyDown(GameSettings.SkillKey) && isSkillReady && moveInput != 0)
        {
            vx *= 2f;
            rb.velocity = new Vector2(vx, rb.velocity.y);
            TryStartSkill();
            return;
        }    

        if (Input.GetKeyDown(GameSettings.JumpKey) && isGrounded)
        {
            isGrounded = false;
            vy = jumpForce;
            OnJump(ref vy);
            SetParameterOnlyTrue("Jump");
        }

        if (!isFalling && !isGrounded && vy < 0f)
        {
            isFalling = true;
            SetParameterOnlyTrue("Falling");
        }

        OnSkillTick(ref vx, ref vy);

        rb.velocity = new Vector2(vx, vy);

        if (!isGrounded)
            return;
        if (moveInput == 0)
            SetParameterOnlyTrue("IDLE");
        else
            SetParameterOnlyTrue("Run");
    }

    protected override void TryStartSkill()
    {
        animator.SetTrigger("StartSlide");
        SetParameterOnlyTrue("Slide");
        skillActiveRoutine = StartCoroutine(SkillActiveRoutine());
    }

    protected override IEnumerator SkillActiveRoutine()
    {
        var wait = new WaitForSeconds(0.1f);
        isSkilling = true;
        isSkillReady = false;
        float t = skillDuration;
        while (t > skillDuration * 0.3f)
        {
            yield return wait;
            t -= 0.1f;
            playerUI.SetCoolDownUI(t, skillDuration, true);
        }
        rb.velocity = new Vector2(rb.velocity.x * 0.3f, rb.velocity.y);
        while (t > 0)
        {
            yield return wait;
            t -= 0.1f;
            playerUI.SetCoolDownUI(t, skillDuration, true);
        }
        isSkilling = false;
        OnSkillActiveEnd();
        playerUI.SkillCoolDown(skillCoolTime, () => isSkillReady = true);
    }

    protected override void OnSkillActiveEnd()
    {
        animator.SetBool("Slide", false);
    }

    protected override void HitInterrupt()
    {
        if (!isSkilling) return;

        if (skillActiveRoutine != null) StopCoroutine(skillActiveRoutine);
        isSkilling = false;
        playerUI.SkillCoolDown(skillCoolTime, () => isSkillReady = true);
    }
}
