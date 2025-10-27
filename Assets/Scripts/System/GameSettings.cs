using UnityEngine;

public static class GameSettings
{
    // Resolution presets: 0: HD, 1: FHD, 2: QHD, 3: 4K
    public static readonly int[] ResWidths = { 1280, 1920, 2560, 3840 };
    public static readonly int[] ResHeights = { 720, 1080, 1440, 2160 };

    // Defaults
    private const int DefaultResIndex = 1; // FHD
    private const bool DefaultFullscreen = false;
    private const KeyCode DefaultJump = KeyCode.UpArrow;
    private const KeyCode DefaultLeft = KeyCode.LeftArrow;
    private const KeyCode DefaultRight = KeyCode.RightArrow;
    private const KeyCode DefaultSkill = KeyCode.Space;
    private const KeyCode DefaultChange1 = KeyCode.A;
    private const KeyCode DefaultChange2 = KeyCode.S;
    private const KeyCode DefaultChange3 = KeyCode.D;
    private const KeyCode DefaultChange4 = KeyCode.F;

    private static bool _loaded;

    public static int ResolutionIndex { get; private set; }
    public static bool Fullscreen { get; private set; }

    public static KeyCode JumpKey { get; private set; }
    public static KeyCode LeftKey { get; private set; }
    public static KeyCode RightKey { get; private set; }
    public static KeyCode SkillKey { get; private set; }
    public static KeyCode Change1Key { get; private set; }
    public static KeyCode Change2Key { get; private set; }
    public static KeyCode Change3Key { get; private set; }
    public static KeyCode Change4Key { get; private set; }

    static GameSettings()
    {
        Load();
    }

    public static void Load()
    {
        if (_loaded) return;

        ResolutionIndex = PlayerPrefs.GetInt("GS.ResIndex", DefaultResIndex);
        ResolutionIndex = Mathf.Clamp(ResolutionIndex, 0, 3);
        Fullscreen = PlayerPrefs.GetInt("GS.Fullscreen", DefaultFullscreen ? 1 : 0) == 1;

        JumpKey = (KeyCode)PlayerPrefs.GetInt("GS.Key.Jump", (int)DefaultJump);
        LeftKey = (KeyCode)PlayerPrefs.GetInt("GS.Key.Left", (int)DefaultLeft);
        RightKey = (KeyCode)PlayerPrefs.GetInt("GS.Key.Right", (int)DefaultRight);
        SkillKey = (KeyCode)PlayerPrefs.GetInt("GS.Key.Skill", (int)DefaultSkill);
        Change1Key = (KeyCode)PlayerPrefs.GetInt("GS.Key.Change1", (int)DefaultChange1);
        Change2Key = (KeyCode)PlayerPrefs.GetInt("GS.Key.Change2", (int)DefaultChange2);
        Change3Key = (KeyCode)PlayerPrefs.GetInt("GS.Key.Change3", (int)DefaultChange3);
        Change4Key = (KeyCode)PlayerPrefs.GetInt("GS.Key.Change4", (int)DefaultChange4);

        _loaded = true;
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("GS.ResIndex", ResolutionIndex);
        PlayerPrefs.SetInt("GS.Fullscreen", Fullscreen ? 1 : 0);

        PlayerPrefs.SetInt("GS.Key.Jump", (int)JumpKey);
        PlayerPrefs.SetInt("GS.Key.Left", (int)LeftKey);
        PlayerPrefs.SetInt("GS.Key.Right", (int)RightKey);
        PlayerPrefs.SetInt("GS.Key.Skill", (int)SkillKey);
        PlayerPrefs.SetInt("GS.Key.Change1", (int)Change1Key);
        PlayerPrefs.SetInt("GS.Key.Change2", (int)Change2Key);
        PlayerPrefs.SetInt("GS.Key.Change3", (int)Change3Key);
        PlayerPrefs.SetInt("GS.Key.Change4", (int)Change4Key);

        PlayerPrefs.Save();
    }

    public static void ApplyDisplay()
    {
        var w = ResWidths[Mathf.Clamp(ResolutionIndex, 0, 3)];
        var h = ResHeights[Mathf.Clamp(ResolutionIndex, 0, 3)];
        Screen.SetResolution(w, h, Fullscreen);
    }

    public static void SetResolutionPreset(int index)
    {
        Load();
        ResolutionIndex = Mathf.Clamp(index, 0, 3);
        Save();
        ApplyDisplay();
    }

    public static void SetFullscreen(bool fullscreen)
    {
        Load();
        Fullscreen = fullscreen;
        Save();
        ApplyDisplay();
    }

    public static void RestoreDefaultKeys()
    {
        JumpKey = DefaultJump;
        LeftKey = DefaultLeft;
        RightKey = DefaultRight;
        SkillKey = DefaultSkill;
        Change1Key = DefaultChange1;
        Change2Key = DefaultChange2;
        Change3Key = DefaultChange3;
        Change4Key = DefaultChange4;
        Save();
    }

    public static void SetKeyByAction(string actionName, KeyCode key)
    {
        Load();
        switch (actionName.ToLowerInvariant())
        {
            case "jump": JumpKey = key; break;
            case "left": LeftKey = key; break;
            case "right": RightKey = key; break;
            case "skill": SkillKey = key; break;
            case "change1": Change1Key = key; break;
            case "change2": Change2Key = key; break;
            case "change3": Change3Key = key; break;
            case "change4": Change4Key = key; break;
            default:
                Debug.LogWarning($"Unknown action name: {actionName}");
                return;
        }
        Save();
    }

    public static KeyCode GetKeyByAction(string actionName)
    {
        switch (actionName.ToLowerInvariant())
        {
            case "jump": return JumpKey;
            case "left": return LeftKey;
            case "right": return RightKey;
            case "skill": return SkillKey;
            case "change1": return Change1Key;
            case "change2": return Change2Key;
            case "change3": return Change3Key;
            case "change4": return Change4Key;
            default: return KeyCode.None;
        }
    }
}