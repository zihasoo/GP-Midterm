using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Display Settings")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Key Rebind Buttons")]
    public Button rebindJumpButton;
    public Button rebindLeftButton;
    public Button rebindRightButton;
    public Button rebindSkillButton;
    public Button rebindChange1Button;
    public Button rebindChange2Button;
    public Button rebindChange3Button;
    public Button rebindChange4Button;
    public Button restoreDefaultKeysButton;

    [Header("Key Binding Texts")]
    public Text jumpKeyText;
    public Text leftKeyText;
    public Text rightKeyText;
    public Text skillKeyText;
    public Text change1KeyText;
    public Text change2KeyText;
    public Text change3KeyText;
    public Text change4KeyText;

    [Header("Navigation")]
    public GameObject settingScreen;
    public Button startGameButton;
    public Button goToSettingButton;
    public Button goToBackButton;
    public Button quitGameButton;

    // 리바인딩 진행 중 액션명(없으면 null)
    private string _waitingRebindAction;

    private void Start()
    {
        // 저장된 설정 적용 및 UI 초기 상태 반영
        GameSettings.Load();
        GameSettings.ApplyDisplay();

        // Initialize UI values (no null checks, assumes properly assigned in Inspector)
        resolutionDropdown.value = GameSettings.ResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        fullscreenToggle.isOn = GameSettings.Fullscreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);

        // Buttons wiring
        startGameButton.onClick.AddListener(StartGame);
        goToSettingButton.onClick.AddListener(OnClickGoToSetting);
        goToBackButton.onClick.AddListener(OnClickGoToBack);
        quitGameButton.onClick.AddListener(OnQuitGame);

        rebindJumpButton.onClick.AddListener(OnRebindJump);
        rebindLeftButton.onClick.AddListener(OnRebindLeft);
        rebindRightButton.onClick.AddListener(OnRebindRight);
        rebindSkillButton.onClick.AddListener(OnRebindSkill);
        rebindChange1Button.onClick.AddListener(OnRebindChange1);
        rebindChange2Button.onClick.AddListener(OnRebindChange2);
        rebindChange3Button.onClick.AddListener(OnRebindChange3);
        rebindChange4Button.onClick.AddListener(OnRebindChange4);
        restoreDefaultKeysButton.onClick.AddListener(OnRestoreDefaultKeys);

        UpdateAllBindingTexts();
    }

    private void OnDestroy()
    {
        resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggled);

        startGameButton.onClick.RemoveListener(StartGame);
        goToSettingButton.onClick.RemoveListener(OnClickGoToSetting);
        goToBackButton.onClick.RemoveListener(OnClickGoToBack);
        quitGameButton.onClick.RemoveListener(OnQuitGame);

        rebindJumpButton.onClick.RemoveListener(OnRebindJump);
        rebindLeftButton.onClick.RemoveListener(OnRebindLeft);
        rebindRightButton.onClick.RemoveListener(OnRebindRight);
        rebindSkillButton.onClick.RemoveListener(OnRebindSkill);
        rebindChange1Button.onClick.RemoveListener(OnRebindChange1);
        rebindChange2Button.onClick.RemoveListener(OnRebindChange2);
        rebindChange3Button.onClick.RemoveListener(OnRebindChange3);
        rebindChange4Button.onClick.RemoveListener(OnRebindChange4);
        restoreDefaultKeysButton.onClick.RemoveListener(OnRestoreDefaultKeys);
    }

    private void Update()
    {
        // 리바인딩 대기 중이면 키 입력을 한 번만 잡아서 바인딩
        if (!string.IsNullOrEmpty(_waitingRebindAction) && Input.anyKeyDown)
        {
            var key = DetectAnyKeyDown();
            if (key != KeyCode.None)
            {
                var action = _waitingRebindAction;
                GameSettings.SetKeyByAction(action, key);
                _waitingRebindAction = null;
                UpdateBindingTextForAction(action);
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Stage_1");
    }

    public void OnQuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // 해상도 드롭다운: 0:HD, 1:FHD, 2:QHD, 3:4K
    public void OnResolutionChanged(int index)
    {
        GameSettings.SetResolutionPreset(index);
    }

    // 전체화면 토글
    public void OnFullscreenToggled(bool isOn)
    {
        GameSettings.SetFullscreen(isOn);
    }

    // Settings 화면 이동
    public void OnClickGoToSetting()
    {
        settingScreen.SetActive(true);
    }

    public void OnClickGoToBack()
    {
        settingScreen.SetActive(false);
    }

    // 리바인딩 시작용 버튼 핸들러들
    public void OnRebindJump() { _waitingRebindAction = "jump"; }
    public void OnRebindLeft() { _waitingRebindAction = "left"; }
    public void OnRebindRight() { _waitingRebindAction = "right"; }
    public void OnRebindSkill() { _waitingRebindAction = "skill"; }
    public void OnRebindChange1() { _waitingRebindAction = "change1"; }
    public void OnRebindChange2() { _waitingRebindAction = "change2"; }
    public void OnRebindChange3() { _waitingRebindAction = "change3"; }
    public void OnRebindChange4() { _waitingRebindAction = "change4"; }

    // 기본키로 되돌리기 버튼
    public void OnRestoreDefaultKeys()
    {
        GameSettings.RestoreDefaultKeys();
        UpdateAllBindingTexts();
    }

    // 현재 눌린 KeyCode 탐지
    private KeyCode DetectAnyKeyDown()
    {
        foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(code))
                return code;
        }
        return KeyCode.None;
    }

    private void UpdateAllBindingTexts()
    {
        jumpKeyText.text = $"JumpKey: {GameSettings.JumpKey}";
        leftKeyText.text = $"LeftKey: {GameSettings.LeftKey}";
        rightKeyText.text = $"RightKey: {GameSettings.RightKey}";
        skillKeyText.text = $"SkillKey: {GameSettings.SkillKey}";
        change1KeyText.text = $"Change1: {GameSettings.Change1Key}";
        change2KeyText.text = $"Change2: {GameSettings.Change2Key}";
        change3KeyText.text = $"Change3: {GameSettings.Change3Key}";
        change4KeyText.text = $"Change4: {GameSettings.Change4Key}";
    }

    private void UpdateBindingTextForAction(string action)
    {
        switch (action)
        {
            case "jump":
                jumpKeyText.text = $"JumpKey: {GameSettings.JumpKey}";
                break;
            case "left":
                leftKeyText.text = $"LeftKey: {GameSettings.LeftKey}";
                break;
            case "right":
                rightKeyText.text = $"RightKey: {GameSettings.RightKey}";
                break;
            case "skill":
                skillKeyText.text = $"SkillKey: {GameSettings.SkillKey}";
                break;
            case "change1":
                change1KeyText.text = $"Change1: {GameSettings.Change1Key}";
                break;
            case "change2":
                change2KeyText.text = $"Change2: {GameSettings.Change2Key}";
                break;
            case "change3":
                change3KeyText.text = $"Change3: {GameSettings.Change3Key}";
                break;
            case "change4":
                change4KeyText.text = $"Change4: {GameSettings.Change4Key}";
                break;
        }
    }
}
