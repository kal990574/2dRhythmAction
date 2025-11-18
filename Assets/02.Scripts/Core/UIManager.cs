using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Life UI (Hearts)")]
    public Transform HeartContainer;
    public GameObject HeartPrefab;
    public Sprite FullHeartSprite;
    public Sprite EmptyHeartSprite;

    [Header("UI Text References")]
    public TextMeshProUGUI ScoreTextUI;
    public TextMeshProUGUI MaxComboText1UI;
    public TextMeshProUGUI MaxComboText2UI;
    public TextMeshProUGUI CurrentComboTextUI;

    [Header("Judgement Display")]
    public TextMeshProUGUI JudgementText;
    public float JudgementDisplayDuration = 0.5f;

    private List<Image> heartImages = new List<Image>();
    private float judgementTimer = 0f;
    private int maxLife = 3;

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

    public void InitializeHearts(int maxLifeCount)
    {
        if (HeartContainer == null || HeartPrefab == null)
        {
            Debug.LogError("HeartContainer 또는 HeartPrefab이 할당되지 않았습니다!");
            return;
        }

        maxLife = maxLifeCount;

        // 기존 하트 제거
        foreach (Transform child in HeartContainer)
        {
            Destroy(child.gameObject);
        }
        heartImages.Clear();

        // MaxLife만큼 하트 생성
        for (int i = 0; i < maxLife; i++)
        {
            GameObject heartObj = Instantiate(HeartPrefab, HeartContainer);
            Image heartImage = heartObj.GetComponent<Image>();

            if (heartImage != null)
            {
                heartImage.sprite = FullHeartSprite;
                heartImages.Add(heartImage);
            }
            else
            {
                Debug.LogError("HeartPrefab에 Image 컴포넌트가 없습니다!");
            }
        }

        Debug.Log($"하트 UI 초기화 완료: {maxLife}개");
    }

    public void UpdateLife(int currentLife, int maxLife)
    {
        // 하트가 초기화되지 않았으면 초기화
        if (heartImages.Count == 0)
        {
            InitializeHearts(maxLife);
        }

        // 하트 개수가 다르면 재초기화
        if (heartImages.Count != maxLife)
        {
            InitializeHearts(maxLife);
        }

        // currentLife에 따라 하트 Sprite 변경
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentLife)
            {
                // 살아있는 하트
                heartImages[i].sprite = FullHeartSprite;
            }
            else
            {
                // 비어있는 하트
                heartImages[i].sprite = EmptyHeartSprite;
            }
        }
    }

    public void UpdateScore(int score)
    {
        if (ScoreTextUI != null)
        {
            ScoreTextUI.text = $"{score:N0}";
        }
    }

    public void UpdateMaxCombo(int maxCombo)
    {
        if (MaxComboText2UI != null)
        {
            MaxComboText2UI.text = $"{maxCombo}";
        }
    }

    public void UpdateCurrentCombo(int currentCombo)
    {
        if (CurrentComboTextUI != null)
        {
            if (currentCombo > 0)
            {
                CurrentComboTextUI.text = $"{currentCombo} COMBO";
            }
            else
            {
                CurrentComboTextUI.text = "";
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