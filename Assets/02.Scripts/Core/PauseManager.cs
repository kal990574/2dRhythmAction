using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Pause UI")]
    public GameObject PauseMenuUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // ESC 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.CurrentState == GameState.Playing)
            {
                Pause();
            }
            else if (GameManager.Instance.CurrentState == GameState.Paused)
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        if (GameManager.Instance == null)
            return;

        // 시간 정지
        Time.timeScale = 0f;

        // 오디오 정지
        AudioListener.pause = true;

        // 게임 상태 변경
        GameManager.Instance.CurrentState = GameState.Paused;

        // Pause UI 표시
        if (PauseMenuUI != null)
        {
            PauseMenuUI.SetActive(true);
        }

        Debug.Log("게임 일시정지");
    }

    public void Resume()
    {
        if (GameManager.Instance == null)
            return;

        // 시간 재개
        Time.timeScale = 1f;

        // 오디오 재개
        AudioListener.pause = false;

        // 게임 상태 변경
        GameManager.Instance.CurrentState = GameState.Playing;

        // Pause UI 숨김
        if (PauseMenuUI != null)
        {
            PauseMenuUI.SetActive(false);
        }

        Debug.Log("게임 재개");
    }
}