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
    string skillName;

    public void Init(string playerName, string skillName, float maxHP)
    {
        playerText.text = playerName;
        this.skillName = skillName;
        this.maxHP = maxHP;
        SetHP(maxHP);
        SetSkillCool(0, 0);
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

    public void SetHP(float HP)
    {
        hpText.text = $"HP: {HP} / {maxHP}";
        hpFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FillMaxWidth * (HP / maxHP));
    }

    public void SetSkillCool(float skillCool, float maxCool)
    {
        if (skillCool >= maxCool)
        {
            skillCoolText.text = $"{skillName}: ready";
            skillCoolFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FillMaxWidth);
        }
        else
        {
            skillCoolText.text = $"{skillName}: {skillCool:0.0}";
            skillCoolFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FillMaxWidth * (skillCool / maxCool));
        }
    }

    public void SkillCoolDown(float skillCoolTime, Action onEnd)
    {
        StartCoroutine(SkillCoolDownRoutine(skillCoolTime, onEnd));
    }

    private IEnumerator SkillCoolDownRoutine(float skillCoolTime, Action onEnd)
    {
        var wait = new WaitForSeconds(0.1f);
        float t = 0f;
        SetSkillCool(t, skillCoolTime);
        while (t < skillCoolTime)
        {
            yield return wait;
            t += 0.1f;
            SetSkillCool(t, skillCoolTime);
        }
        onEnd();
    }
}
