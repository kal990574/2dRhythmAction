using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

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

    [Header("DOTween Animation Settings")]
    [Header("Score Animation")]
    public float ScorePunchStrength = 0.15f;
    public float ScorePunchDuration = 0.2f;

    [Header("Combo Animation")]
    public float ComboPunchStrength = 0.3f;
    public float ComboPunchDuration = 0.3f;
    public float ComboFadeDuration = 0.3f;

    [Header("Max Combo Animation")]
    public float MaxComboPunchStrength = 0.2f;
    public float MaxComboPunchDuration = 0.3f;
    public float MaxComboFlashDuration = 0.15f;

    [Header("Judgement Animation")]
    public float JudgementScalePerfect = 1.5f;
    public float JudgementScaleGood = 1.2f;
    public float JudgementScaleDuration = 0.2f;

    [Header("Life Loss Animation")]
    public float LifeShakeDuration = 0.4f;
    public float LifeShakeStrength = 15f;
    public float LifeFlashDuration = 0.1f;
    public float LifeFlashReturnDuration = 0.2f;

    private List<Image> heartImages = new List<Image>();
    private float judgementTimer = 0f;
    private int maxLife = 3;
    private int previousLife = -1;
    private int previousScore = 0;
    private int previousMaxCombo = 0;

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
            previousLife = currentLife;
        }

        // 하트 개수가 다르면 재초기화
        if (heartImages.Count != maxLife)
        {
            InitializeHearts(maxLife);
        }

        // Life 감소 감지
        bool lifeLost = previousLife > currentLife;
        previousLife = currentLife;

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

        // Life 감소 시 애니메이션
        if (lifeLost)
        {
            AnimateLifeLoss();
        }
    }

    public void UpdateScore(int score)
    {
        if (ScoreTextUI != null)
        {
            ScoreTextUI.text = $"{score:N0}";

            // 점수가 실제로 증가했을 때만 애니메이션
            if (score > previousScore)
            {
                AnimateScore();
            }

            previousScore = score;
        }
    }

    public void UpdateMaxCombo(int maxCombo)
    {
        if (MaxComboText2UI != null)
        {
            MaxComboText2UI.text = $"{maxCombo}";

            // 값이 실제로 갱신되었을 때만 애니메이션
            if (maxCombo > previousMaxCombo)
            {
                AnimateMaxCombo();
            }

            previousMaxCombo = maxCombo;
        }
    }

    public void UpdateCurrentCombo(int currentCombo)
    {
        if (CurrentComboTextUI != null)
        {
            if (currentCombo > 0)
            {
                CurrentComboTextUI.text = $"{currentCombo} COMBO";
                AnimateCombo();
            }
            else
            {
                CurrentComboTextUI.text = "";
                AnimateComboEnd();
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
        AnimateJudgement(judgement);
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

    // ========== DOTween 애니메이션 함수들 ==========

    private void AnimateScore()
    {
        if (ScoreTextUI == null) return;

        // 작은 펀치 스케일 효과
        ScoreTextUI.transform.DOKill();
        ScoreTextUI.transform.localScale = Vector3.one; // Scale 리셋
        ScoreTextUI.transform.DOPunchScale(Vector3.one * ScorePunchStrength, ScorePunchDuration, 5, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    private void AnimateCombo()
    {
        if (CurrentComboTextUI == null) return;

        // 큰 펀치 스케일 + 살짝 튕기는 효과
        CurrentComboTextUI.transform.DOKill();
        CurrentComboTextUI.transform.localScale = Vector3.one; // Scale 리셋
        CurrentComboTextUI.transform.DOPunchScale(Vector3.one * ComboPunchStrength, ComboPunchDuration, 8, 0.8f)
            .SetEase(Ease.OutBack);
    }

    private void AnimateComboEnd()
    {
        if (CurrentComboTextUI == null) return;

        // 페이드 아웃 효과
        CurrentComboTextUI.DOKill();
        CurrentComboTextUI.DOFade(0f, ComboFadeDuration)
            .OnComplete(() => CurrentComboTextUI.alpha = 1f);
    }

    private void AnimateMaxCombo()
    {
        // 빛나는 효과
        if (MaxComboText1UI != null)
        {
            MaxComboText1UI.DOKill();
            MaxComboText1UI.transform.DOKill();
            MaxComboText1UI.transform.localScale = Vector3.one; // Scale 리셋
            Color originalColor1 = MaxComboText1UI.color;
            Sequence seq1 = DOTween.Sequence();
            seq1.Append(MaxComboText1UI.DOColor(Color.blue, MaxComboFlashDuration));
            seq1.Append(MaxComboText1UI.DOColor(originalColor1, MaxComboFlashDuration));
            seq1.Join(MaxComboText1UI.transform.DOPunchScale(Vector3.one * MaxComboPunchStrength, MaxComboPunchDuration));
        }

        if (MaxComboText2UI != null)
        {
            MaxComboText2UI.DOKill();
            MaxComboText2UI.transform.DOKill();
            MaxComboText2UI.transform.localScale = Vector3.one; // Scale 리셋
            Color originalColor2 = MaxComboText2UI.color;
            Sequence seq2 = DOTween.Sequence();
            seq2.Append(MaxComboText2UI.DOColor(Color.blue, MaxComboFlashDuration));
            seq2.Append(MaxComboText2UI.DOColor(originalColor2, MaxComboFlashDuration));
            seq2.Join(MaxComboText2UI.transform.DOPunchScale(Vector3.one * MaxComboPunchStrength, MaxComboPunchDuration));
        }
    }

    private void AnimateJudgement(JudgementType judgement)
    {
        if (JudgementText == null) return;

        JudgementText.transform.DOKill();

        // 판정에 따라 다른 스케일
        float targetScale = judgement == JudgementType.Perfect ? JudgementScalePerfect : JudgementScaleGood;

        // 스케일 인 애니메이션
        JudgementText.transform.localScale = Vector3.zero;
        JudgementText.transform.DOScale(targetScale, JudgementScaleDuration)
            .SetEase(Ease.OutBack);

        // 페이드 아웃
        JudgementText.DOKill();
        JudgementText.alpha = 1f;
        JudgementText.DOFade(0f, JudgementDisplayDuration)
            .SetDelay(JudgementDisplayDuration * 0.5f);
    }

    private void AnimateLifeLoss()
    {
        if (HeartContainer == null) return;

        // 하트 컨테이너 흔들림 + 깜빡임
        HeartContainer.DOKill();
        HeartContainer.DOShakePosition(LifeShakeDuration, LifeShakeStrength, 20, 90f);

        // 빨간색 플래시
        foreach (var heart in heartImages)
        {
            if (heart != null)
            {
                heart.DOKill();
                Color originalColor = heart.color;
                Sequence seq = DOTween.Sequence();
                seq.Append(heart.DOColor(Color.red, LifeFlashDuration));
                seq.Append(heart.DOColor(originalColor, LifeFlashReturnDuration));
            }
        }
    }
}