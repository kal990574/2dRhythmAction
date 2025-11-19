using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState CurrentState = GameState.Ready;
    
    // 싱글톤 패턴
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
        CurrentState = GameState.Ready;
        Debug.Log("GameManager 초기화 완료");
        StartGame();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        SoundManager.Instance?.PlayMusic();
        Debug.Log("게임 시작");
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;

        // 음악 정지
        SoundManager.Instance?.StopMusic();

        // GameOver UI 표시
        UIManager.Instance?.ShowGameOver();

        Debug.Log("게임 오버");
    }

    public void GameClear()
    {
        CurrentState = GameState.GameClear;

        // GameClear UI 표시
        UIManager.Instance?.ShowGameClear();

        Debug.Log("게임 클리어!");
    }
}

public enum GameState
{
    Ready,
    Playing,
    Paused,
    GameOver,
    GameClear
}