using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("GamePlay")]
    public Player[] players;
    public GameObject playerChangeEffect;
    public GameObject GoalEffect;
    public Transform[] checkPoints;
    public Transform items;
    public Animator collectionAnimator;

    [Header("UI ref")]
    public GameObject dieScreen;
    public GameObject pauseScreen;
    public GameObject endScreen;

    [Header("Camera")]
    public Transform cameraTransform;

    int currentPlayerIndex;
    int currentCheckPointIndex;
    bool isEnd;
    readonly float[] playerPosCorrection = { 0f, -0.018f, 0.147f, -0.8294f};

    void Start()
    {
        isEnd = false;
        currentPlayerIndex = 0;
        currentCheckPointIndex = 0;
        foreach (var p in players)
        {
            p.gameObject.SetActive(false);
        }
        players[currentPlayerIndex].gameObject.SetActive(true);
        players[currentPlayerIndex].playerUI.SetSelected();
    }

    void Update()
    {
        if (isEnd) return;

        if (!GetActivePlayer().IsSkilling)
        {
            if (Input.GetKeyDown(GameSettings.Change1Key)) ChangePlayer(0);
            else if (Input.GetKeyDown(GameSettings.Change2Key)) ChangePlayer(1);
            else if (Input.GetKeyDown(GameSettings.Change3Key)) ChangePlayer(2);
            else if (Input.GetKeyDown(GameSettings.Change4Key)) ChangePlayer(3);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseScreen.SetActive(!pauseScreen.activeSelf);
            Time.timeScale = pauseScreen.activeSelf ? 0f : 1f;
        }

        var p = GetActivePlayer();

        cameraTransform.position = new Vector3(p.transform.position.x, p.transform.position.y + 2.5f - playerPosCorrection[currentPlayerIndex], -10);
    }

    public void OnLevelComplete()
    {
        isEnd = true;
        GetActivePlayer().gameObject.SetActive(false);
        StartCoroutine(FireWork());
        endScreen.SetActive(true);
    }

    public void OnItemCollected(Transform collision)
    {
        collision.gameObject.SetActive(false);

        collectionAnimator.gameObject.SetActive(true);
        collectionAnimator.transform.position = collision.transform.position;
        collectionAnimator.SetTrigger("Collected");

        StartCoroutine(WaitCollectionAnimation());
    }

    public void HealEveryPlayer(int healAmount)
    {
        foreach (var p in players)
        {
            p.Heal(healAmount);
        }
    }

    public void OnRestartButtonClicked(bool restart)
    {
        if (restart)
        {
            SceneManager.LoadScene("Stage_1");
        }
        else
        {
            ResetToCheckPoint();
            dieScreen.SetActive(false);
        }
    }
    
    public void OnExitPauseScreen()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CheckPointReached(string checkPointName)
    {
        currentCheckPointIndex = int.Parse(checkPointName.Split('_')[1]);
    }

    public void OnDie()
    {
        GetActivePlayer().playerUI.SetUnSelected();
        GetActivePlayer().gameObject.SetActive(false);
        dieScreen.SetActive(true);
    }

    private void ResetToCheckPoint()
    {
        ResetPlayers();
        foreach (Transform item in items)
        {
            item.gameObject.SetActive(true);
        }
        collectionAnimator.gameObject.SetActive(false);
        var p = GetActivePlayer();
        p.Rb.velocity = Vector2.zero;
        p.Rb.totalForce = Vector2.zero;
        p.transform.position = checkPoints[currentCheckPointIndex].position;
    }

    private void ResetPlayers()
    {
        GetActivePlayer().playerUI.SetUnSelected();
        foreach (var p in players)
        {
            p.gameObject.SetActive(false);
            p.SetDefault();
        }
        currentPlayerIndex = 0;
        players[currentPlayerIndex].gameObject.SetActive(true);
        players[currentPlayerIndex].playerUI.SetSelected();
    }

    private void ChangePlayer(int targetIndex)
    {
        if (targetIndex == currentPlayerIndex) return;

        var from = GetActivePlayer();
        var target = players[targetIndex];

        target.gameObject.SetActive(true);

        target.transform.position = GetCorrectionPos(from.transform.position, currentPlayerIndex, targetIndex);
        target.Rb.velocity = from.Rb.velocity;
        target.Rb.totalForce = from.Rb.totalForce;

        from.playerUI.SetUnSelected();
        target.playerUI.SetSelected();

        from.gameObject.SetActive(false);
        currentPlayerIndex = targetIndex;
        var obj = Instantiate(playerChangeEffect, target.transform.position, Quaternion.identity);
        Destroy(obj, 0.7f);
    }


    private Player GetActivePlayer() => players[currentPlayerIndex];

    private Vector3 GetCorrectionPos(Vector3 standPos, int fromIndex, int toIndex)
    {
        standPos.y += playerPosCorrection[toIndex] - playerPosCorrection[fromIndex];
        return standPos;
    }

    private IEnumerator FireWork()
    {
        var standPos = GetActivePlayer().transform.position;
        while (true)
        {
            var pos = new Vector3(standPos.x + UnityEngine.Random.Range(-6f, 6f), standPos.y + UnityEngine.Random.Range(-2f, 7f), 0f);
            var obj = Instantiate(GoalEffect, pos, Quaternion.identity);
            Destroy(obj, 2f);
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator WaitCollectionAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        collectionAnimator.gameObject.SetActive(false);
    }
}
