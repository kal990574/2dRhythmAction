using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Text References")]
    public TextMeshProUGUI LifeText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI MaxComboText;
    public TextMeshProUGUI CurrentComboText;

    [Header("Judgement Display")]
    public TextMeshProUGUI JudgementText;
    public float JudgementDisplayDuration = 0.5f;

    private float judgementTimer = 0f;

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

    private void Update()
    {
        if (JudgementText != null && judgementTimer > 0f)
        {
            judgementTimer -= Time.deltaTime;

            if (judgementTimer <= 0f)
            {
                JudgementText.text = "";
            }
        }
    }

    public void UpdateLife(int currentLife, int maxLife)
    {
        if (LifeText != null)
        {
            LifeText.text = $"LIFE: {currentLife} / {maxLife}";
        }
    }

    public void UpdateScore(int score)
    {
        if (ScoreText != null)
        {
            ScoreText.text = $"SCORE: {score}";
        }
    }

    public void UpdateMaxCombo(int maxCombo)
    {
        if (MaxComboText != null)
        {
            MaxComboText.text = $"MAX COMBO: {maxCombo}";
        }
    }

    public void UpdateCurrentCombo(int currentCombo)
    {
        if (CurrentComboText != null)
        {
            if (currentCombo > 0)
            {
                CurrentComboText.text = $"{currentCombo} COMBO";
            }
            else
            {
                CurrentComboText.text = "";
            }
        }
    }

    public void ShowJudgement(JudgementType judgement)
    {
        if (JudgementText == null)
            return;

        switch (judgement)
        {
            case JudgementType.Perfect:
                JudgementText.text = "PERFECT!";
                JudgementText.color = Color.yellow;
                break;

            case JudgementType.Good:
                JudgementText.text = "GOOD";
                JudgementText.color = Color.green;
                break;

            case JudgementType.Miss:
                JudgementText.text = "MISS";
                JudgementText.color = Color.red;
                break;
        }

        judgementTimer = JudgementDisplayDuration;
    }

    public void ResetUI()
    {
        UpdateLife(3, 3);
        UpdateScore(0);
        UpdateMaxCombo(0);
        UpdateCurrentCombo(0);

        if (JudgementText != null)
        {
            JudgementText.text = "";
        }
    }
}