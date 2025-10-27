using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPinkMan : Player
{
    [Header("PinkMan Data")]
    public GameObject bigJumpEffectPrefab;
    public GameObject bigJumpActiveEffectPrefab;

    GameObject bigJumpActiveEffectInstance;

    protected new void Awake()
    {
        playerUI.Init("PinkMan", maxHP);
        base.Awake();
    }

    protected override void TryStartSkill()
    {
        bigJumpActiveEffectInstance = Instantiate(bigJumpActiveEffectPrefab, transform);
        skillActiveRoutine = StartCoroutine(SkillActiveRoutine());
    }

    protected override void OnJump(ref float vy)
    {
        if (!isSkilling) return;

        var obj = Instantiate(bigJumpEffectPrefab, transform.position, Quaternion.identity);
        Destroy(obj, 0.5f);

        vy *= 1.5f;
        isSkilling = false;
        Destroy(bigJumpActiveEffectInstance);
        if (skillActiveRoutine != null)
            StopCoroutine(skillActiveRoutine);
        playerUI.SkillCoolDown(skillCoolTime, () => isSkillReady = true);
    }

    protected override void OnSkillActiveEnd()
    {
        Destroy(bigJumpActiveEffectInstance);
    }
}
