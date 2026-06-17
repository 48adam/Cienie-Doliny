using UnityEngine;
using UnityEngine.InputSystem;

public class Skill_Tree_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject skillTreePanel;
    [SerializeField] private CanvasGroup skillTreeCanvasGroup;

    [Header("Input")]
    [SerializeField] private bool allowKeyboardToggle = true;
    [SerializeField] private Key toggleKey = Key.K;
    [SerializeField] private Key closeKey = Key.Escape;
    [SerializeField] private bool showOnStart = false;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenPanelOpen = true;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (skillTreePanel == null)
            skillTreePanel = gameObject;

        if (skillTreeCanvasGroup == null && skillTreePanel != null)
            skillTreeCanvasGroup = skillTreePanel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        SetSkillTreeVisible(showOnStart);
    }

    private void Update()
    {
        if (!allowKeyboardToggle || Keyboard.current == null)
            return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            ToggleSkillTree();
        }

        if (IsOpen && Keyboard.current[closeKey].wasPressedThisFrame)
        {
            HideSkillTree();
        }
    }

    private void OnDisable()
    {
        if (pauseGameWhenPanelOpen && IsOpen)
            Game_Pause_Manager.ForceUnpause();
    }

    public void ToggleSkillTree()
    {
        SetSkillTreeVisible(!IsOpen);
    }

    public void ShowSkillTree()
    {
        SetSkillTreeVisible(true);
    }

    public void HideSkillTree()
    {
        SetSkillTreeVisible(false);
    }

    public void SetSkillTreeVisible(bool visible)
    {
        IsOpen = visible;

        if (skillTreePanel == null)
            return;

        if (skillTreeCanvasGroup != null)
        {
            // Panel zostaje aktywny, ale staje się niewidoczny i nieklikalny.
            // To jest wygodne przy UI robionym Canvas Group, tak jak w tutorialu.
            skillTreePanel.SetActive(true);
            skillTreeCanvasGroup.alpha = visible ? 1f : 0f;
            skillTreeCanvasGroup.interactable = visible;
            skillTreeCanvasGroup.blocksRaycasts = visible;
        }
        else
        {
            // Fallback: jeśli nie masz Canvas Group, zwyczajnie włączamy/wyłączamy panel.
            skillTreePanel.SetActive(visible);
        }

        if (pauseGameWhenPanelOpen)
            Game_Pause_Manager.SetPaused(visible);
    }
}
