using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Player Stats")]
    public int MaxLife = 3;
    public int CurrentLife = 3;
    public int Score = 0;
    public int CurrentCombo = 0;
    public int MaxCombo = 0;
    public int HighScore { get; private set; } = 0;

    [Header("Score Settings")]
    public int PerfectScore = 100;
    public int GoodScore = 50;
    public float ComboMultiplier = 0.2f;

    private const string HIGH_SCORE_KEY = "HighScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadHighScore();
        ResetPlayer();
    }

    public void OnJudgementReceived(JudgementType judgement)
    {
        switch (judgement)
        {
            case JudgementType.Perfect:
                OnPerfect();
                break;

            case JudgementType.Good:
                OnGood();
                break;

            case JudgementType.Miss:
                OnMiss();
                break;
        }

        UpdateUI();
    }

    private void OnPerfect()
    {
        CurrentCombo++;
        int comboBonus = Mathf.FloorToInt(CurrentCombo * ComboMultiplier);
        Score += PerfectScore + comboBonus;

        if (CurrentCombo > MaxCombo)
        {
            MaxCombo = CurrentCombo;
        }

        Debug.Log($"Perfect! Score: {Score}, Combo: {CurrentCombo}");
    }

    private void OnGood()
    {
        CurrentCombo++;
        int comboBonus = Mathf.FloorToInt(CurrentCombo * ComboMultiplier);
        Score += GoodScore + comboBonus;

        if (CurrentCombo > MaxCombo)
        {
            MaxCombo = CurrentCombo;
        }

        Debug.Log($"Good! Score: {Score}, Combo: {CurrentCombo}");
    }

    private void OnMiss()
    {
        CurrentCombo = 0;
        CurrentLife--;

        Debug.Log($"Miss! Life: {CurrentLife}");

        if (CurrentLife <= 0)
        {
            GameOver();
        }
    }

    private void UpdateUI()
    {
        UIManager.Instance?.UpdateScore(Score);
        UIManager.Instance?.UpdateLife(CurrentLife, MaxLife);
        UIManager.Instance?.UpdateMaxCombo(MaxCombo);
        UIManager.Instance?.UpdateCurrentCombo(CurrentCombo);
    }

    public void ResetPlayer()
    {
        CurrentLife = MaxLife;
        Score = 0;
        CurrentCombo = 0;
        MaxCombo = 0;

        UpdateUI();

        Debug.Log("플레이어 상태 초기화");
    }

    private void GameOver()
    {
        UpdateHighScore();
        Debug.Log("Game Over!");
        GameManager.Instance?.GameOver();
    }

    // ========== PlayerPrefs를 사용한 최고점수 관리 ==========

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        Debug.Log($"최고점수 불러오기: {HighScore}");
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
        PlayerPrefs.Save();
        Debug.Log($"최고점수 저장: {HighScore}");
    }

    private void UpdateHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            SaveHighScore();
            Debug.Log($"새로운 최고점수 달성! {HighScore}");
        }
    }

    public void OnGameClear()
    {
        UpdateHighScore();
    }
}