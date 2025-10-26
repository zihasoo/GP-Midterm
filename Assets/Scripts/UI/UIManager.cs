using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Players")]
    public Player[] players;
    public GameObject playerChangeEffect;

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
        var obj = Instantiate(playerChangeEffect, target.transform.position, Quaternion.identity);
        Destroy(obj, 0.7f);
    }

    private Player GetActivePlayer() => players[currentIndex];
    private Vector3 GetCorrectionPos(Vector3 standPos, int fromIndex, int toIndex)
    {
        standPos.y += playerPosCorrection[toIndex] - playerPosCorrection[fromIndex];
        return standPos;
    }
}
