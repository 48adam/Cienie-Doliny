using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private player_helf playerHealth;

    [Header("Texts")]
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private TMP_Text wavesSurvivedText;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Settings")]
    [SerializeField] private bool showOnStart = false;
    [SerializeField] private bool resetStatsOnStart = true;
    [SerializeField] private bool pauseGameOnGameOver = true;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<player_helf>();

        if (canvasGroup == null && gameOverPanel != null)
            canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();

        if (resetStatsOnStart)
            GameRunStats.ResetStats();

        SetGameOverVisible(showOnStart);
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnPlayerDied += ShowGameOver;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnPlayerDied -= ShowGameOver;
    }

    private void Start()
    {
        HideGameOver();
    }

    public void ShowGameOver()
    {
        UpdateTexts();
        SetGameOverVisible(true);

        if (pauseGameOnGameOver)
            Time.timeScale = 0f;
    }


    private void SetGameOverVisible(bool visible)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(visible);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }




    private void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void UpdateTexts()
    {
        if (enemiesKilledText != null)
            enemiesKilledText.text = "Pokonani przeciwnicy: " + GameRunStats.EnemiesKilled;

        if (wavesSurvivedText != null)
            wavesSurvivedText.text = "Przetrwane fale: " + GameRunStats.WavesSurvived;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameRunStats.ResetStats();

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameRunStats.ResetStats();

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}