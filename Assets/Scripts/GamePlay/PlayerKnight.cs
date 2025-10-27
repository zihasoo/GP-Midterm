using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnight : Player
{
    [Header("Knight Data")]
    public GameObject blockEffectPrefab;
    public GameObject blockHitEffectPrefab;

    GameObject blockEffectInstance;

    protected new void Awake()
    {
        playerUI.Init("Knight", maxHP);
        if (!animeParameters.Contains("Block"))
            animeParameters.Add("Block");
        base.Awake();
    }

    protected override void TryStartSkill()
    {
        animator.SetTrigger("StartBlock");
        SetParameterOnlyTrue("Block");
        blockEffectInstance = Instantiate(blockEffectPrefab, transform);
        skillActiveRoutine = StartCoroutine(SkillActiveRoutine());
    }

    protected override void HitInterrupt()
    {
        if (!isSkilling) return;


        var pos = transform.position; pos.y += 0.6f;
        Instantiate(blockHitEffectPrefab, pos, Quaternion.identity);

        isSkilling = false;
        Destroy(blockEffectInstance);
        if (skillActiveRoutine != null) 
            StopCoroutine(skillActiveRoutine);

        SetParameterOnlyTrue("Jump");
        playerUI.SkillCoolDown(skillCoolTime, () => isSkillReady = true);
    }

    protected new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") && isSkilling)
        {
            HitInterrupt();
            return;
        }
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnSkillActiveEnd() => Destroy(blockEffectInstance);

    protected override void SetParameterOnlyTrue(string param)
    {
        if (isSkilling && param != "Block") return;
        base.SetParameterOnlyTrue(param);
    }
}
