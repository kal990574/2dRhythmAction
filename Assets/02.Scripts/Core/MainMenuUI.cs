using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string GameSceneName = "GameScene";

    [Header("Title Animation")]
    public TextMeshProUGUI TitleTextUI;

    [Header("Scale Pulse Settings")]
    public float ScalePulseAmount = 0.1f;
    public float ScalePulseDuration = 0.5f;

    [Header("Color Gradient Settings")]
    public Color[] GradientColors = new Color[]
    {
        new Color(1f, 0.3f, 0.3f),  // Red
        new Color(1f, 0.8f, 0.3f),  // Orange
        new Color(1f, 1f, 0.3f),    // Yellow
        new Color(0.3f, 1f, 0.3f),  // Green
        new Color(0.3f, 0.8f, 1f),  // Cyan
        new Color(0.6f, 0.3f, 1f),  // Purple
        new Color(1f, 0.3f, 0.7f)   // Pink
    };
    public float ColorChangeDuration = 3f;

    private void Start()
    {
        if (TitleTextUI != null)
        {
            PlayTitleAnimation();
        }
    }

    private void PlayTitleAnimation()
    {
        // Scale Pulse
        TitleTextUI.transform.DOScale(Vector3.one * (1f + ScalePulseAmount), ScalePulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Color Gradient: 무지개 색상 변화
        if (GradientColors != null && GradientColors.Length > 0)
        {
            Sequence colorSequence = DOTween.Sequence();

            for (int i = 0; i < GradientColors.Length; i++)
            {
                colorSequence.Append(TitleTextUI.DOColor(GradientColors[i], ColorChangeDuration));
            }

            colorSequence.SetLoops(-1, LoopType.Restart);
        }

        Debug.Log("타이틀 애니메이션 시작!");
    }

    public void OnGameStartButtonClicked()
    {
        // 타이틀 애니메이션 정리 (Transform + Component 모두)
        if (TitleTextUI != null)
        {
            TitleTextUI.transform.DOKill();
            TitleTextUI.DOKill();
        }

        Debug.Log($"게임 시작! {GameSceneName} 씬으로 이동합니다.");
        SceneManager.LoadScene(GameSceneName);
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("게임 종료!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
