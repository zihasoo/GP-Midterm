using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimpleMan : Player
{
    [Header("SimpleMan Data")]
    public GameObject slowFallingEffectPrefab;
    public float slowFallingSpeed;

    protected new void Awake()
    {
        playerUI.Init("SimpleMan", maxHP);
        base.Awake();
    }

    protected override void TryStartSkill()
    {
        var obj = Instantiate(slowFallingEffectPrefab, transform);
        Destroy(obj, skillDuration);
        skillActiveRoutine = StartCoroutine(SkillActiveRoutine());
    }

    protected override void OnSkillTick(ref float vx, ref float vy)
    {
        if (isSkilling && vy < -slowFallingSpeed)
            vy = -slowFallingSpeed;
    }

    private void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }
}
