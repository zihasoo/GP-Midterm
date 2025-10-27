using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject hitPrefab;
    public int maxHP;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float skillDuration;
    public float skillCoolTime;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public PlayerUI playerUI;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spRenderer;
    protected int HP;
    protected bool isGrounded;
    protected bool isFalling;
    protected bool isSkilling;
    protected bool isSkillReady;
    protected Coroutine skillActiveRoutine;

    protected const KeyCode jumpKey = KeyCode.UpArrow;
    protected const KeyCode leftMoveKey = KeyCode.LeftArrow;
    protected const KeyCode rightMoveKey = KeyCode.RightArrow;
    protected const KeyCode skillKey = KeyCode.Space;

    protected List<string> animeParameters = new()
    {
        "IDLE", "Run", "Jump", "Falling"
    };

    public int CurrentHP => HP;
    public Rigidbody2D Rb => rb;

    public bool IsSkilling => isSkilling;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spRenderer = GetComponent<SpriteRenderer>();
        HP = maxHP;
        isGrounded = false;
        isFalling = false;
        isSkilling = false;
        isSkillReady = true;
    }

    protected void Update()
    {
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

        if (Input.GetKeyDown(skillKey) && isSkillReady)
            TryStartSkill();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
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

    protected float ReadMoveInput()
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
        return moveInput;
    }

    protected virtual void TryStartSkill() { }

    protected virtual void OnJump(ref float vy) { }

    protected virtual void OnSkillTick(ref float vx, ref float vy) { }

    protected virtual void HitInterrupt() { }

    protected virtual void OnSkillActiveEnd() { }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Obstacle":
                HP--;
                playerUI.SetHP(HP);
                PlayHitEffect();
                animator.SetTrigger("Hit");
                HitInterrupt();
                return;
            case "Tram":
                rb.velocityY = jumpForce * 1.5f;
                var tramAnimator = collision.gameObject.GetComponent<Animator>();
                tramAnimator.SetTrigger("IsJump");
                isFalling = false;
                SetParameterOnlyTrue("Jump");
                return;
            case "Item":
                HP = Mathf.Min(HP + 1, maxHP);
                playerUI.SetHP(HP);
                var anim = collision.GetComponent<Animator>();
                anim.SetTrigger("Collected");
                Destroy(collision.gameObject, 0.5f);
                return;
            case "CheckPoint":
                collision.GetComponent<SpriteRenderer>().color = Color.red;
                print(collision.transform.position);
                return;
            default:
                return;
        }
    }

    protected void PlayHitEffect()
    {
        var obj = Instantiate(hitPrefab, transform.position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }

    protected virtual void SetParameterOnlyTrue(string param)
    {
        foreach (var p in animeParameters)
        {
            if (p == param) animator.SetBool(p, true);
            else animator.SetBool(p, false);
        }
    }

    protected virtual IEnumerator SkillActiveRoutine()
    {
        var wait = new WaitForSeconds(0.1f);
        isSkilling = true;
        isSkillReady = false;
        float t = skillDuration;
        while (t > 0.05f)
        {
            yield return wait;
            t -= 0.1f;
            playerUI.SetSkillCool(t, skillDuration);
        }
        isSkilling = false;
        OnSkillActiveEnd();
        playerUI.SkillCoolDown(skillCoolTime, () => this.isSkillReady = true);
    }
}
