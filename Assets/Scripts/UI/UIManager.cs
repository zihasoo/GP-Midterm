using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UIManager : MonoBehaviour
{
    [Header("Players")]
    public Player[] players;

    [Header("UI Refs")]
    public Text hpText;
    public RectTransform hpFill;
    public Text skillTimeText;
    public RectTransform skillCooldownFill;

    [Header("Camera")]
    public Transform cameraTransform;

    int currentIndex;
    readonly float[] playerPosCorrection = { 0f, -0.018f, 0.147f, -0.8294f};

    void Start()
    {
        currentIndex = 0;
        foreach (var p in players)
        {
            p.gameObject.SetActive(false);
        }
        players[currentIndex].gameObject.SetActive(true);
        players[currentIndex].playerUI.SetSelected();
    }

    void Update()
    {
        if (!GetActivePlayer().IsSkilling)
        {
            if (Input.GetKeyDown(KeyCode.A)) ChangePlayer(0);
            else if (Input.GetKeyDown(KeyCode.S)) ChangePlayer(1);
            else if (Input.GetKeyDown(KeyCode.D)) ChangePlayer(2);
            else if (Input.GetKeyDown(KeyCode.F)) ChangePlayer(3);
        }

        var p = GetActivePlayer();

        //hpText.text = $"HP {p.CurrentHP}/{p.maxHP}";
        //float width = FillMaxWidth * ((float)p.CurrentHP / p.maxHP);
        //hpFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        //width = FillMaxWidth * p.GetSkillCooldownNormalized();
        //skillCooldownFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        //float sec = p.GetSkillCooldownNormalized();
        //skillTimeText.text = $"{sec:0.0}s";

        cameraTransform.position = new Vector3(p.transform.position.x, p.transform.position.y + 3f - playerPosCorrection[currentIndex], -10);
    }

    private void ChangePlayer(int targetIndex)
    {
        if (targetIndex == currentIndex) return;

        var from = GetActivePlayer();
        var target = players[targetIndex];

        target.gameObject.SetActive(true);

        target.transform.position = GetCorrectionPos(from.transform.position, currentIndex, targetIndex);
        target.Rb.velocity = from.Rb.velocity;
        target.Rb.totalForce = from.Rb.totalForce;

        from.playerUI.SetUnSelected();
        target.playerUI.SetSelected();
        
        from.gameObject.SetActive(false);
        currentIndex = targetIndex;
    }

    private Player GetActivePlayer() => players[currentIndex];
    private Vector3 GetCorrectionPos(Vector3 standPos, int fromIndex, int toIndex)
    {
        standPos.y += playerPosCorrection[toIndex] - playerPosCorrection[fromIndex];
        return standPos;
    }
}
