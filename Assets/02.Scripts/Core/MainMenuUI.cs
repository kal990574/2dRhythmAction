using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string GameSceneName = "GameScene";

    public void OnGameStartButtonClicked()
    {
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
