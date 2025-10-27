using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text playerText;
    public Text hpText;
    public RectTransform hpFill;
    public Text skillCoolText;
    public RectTransform skillCoolFill;

    const float FillMaxWidth = 220f;
    float maxHP;

    public void Init(string playerName, float maxHP)
    {
        playerText.text = playerName;
        this.maxHP = maxHP;
    }

    public void SetDefault()
    {
        StopAllCoroutines();
        SetHPUI(maxHP);
        SetCoolDownUI(0, 0);
    }

    public void SetSelected()
    {
        playerText.color = new Color32(184,255,63,255);
        playerText.fontStyle = FontStyle.Bold;
        playerText.fontSize = 35;
    }

    public void SetUnSelected()
    {
        playerText.color = Color.white;
        playerText.fontStyle = FontStyle.Normal;
        playerText.fontSize = 30;
    }

    public void SetHPUI(float HP)
    {
        hpText.text = $"HP: {HP} / {maxHP}";
        hpFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FillMaxWidth * (HP / maxHP));
    }

    public void SetCoolDownUI(float skillCool, float maxCool, bool isUsing = false)
    {
        if (skillCool <= 0)
        {
            skillCoolText.text = $"Skill Ready";
            skillCoolFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FillMaxWidth);
        }
        else
        {
            if (isUsing)
                skillCoolText.text = $"Active: {skillCool:0}";
            else
                skillCoolText.text = $"Cool Down: {skillCool:0}";
            skillCoolFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FillMaxWidth * (skillCool / maxCool));
        }
    }

    public void SkillCoolDown(float skillCoolTime, Action onEnd)
    {
        StartCoroutine(SkillCoolDownRoutine(skillCoolTime, onEnd));
    }

    private IEnumerator SkillCoolDownRoutine(float skillCoolTime, Action onEnd)
    {
        var wait = new WaitForSeconds(1);
        float t = skillCoolTime;
        SetCoolDownUI(t, skillCoolTime);
        while (t > 0)
        {
            yield return wait;
            t -= 1f;
            SetCoolDownUI(t, skillCoolTime);
        }
        onEnd();
    }
}
