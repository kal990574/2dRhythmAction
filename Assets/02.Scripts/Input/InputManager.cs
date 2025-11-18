using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("References")]
    public NoteController NoteController;

    [Header("Input Settings")]
    public float MaxInputWindow = 0.15f;

    private SoundManager soundManager;
    private JudgementManager judgementManager;

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
        soundManager = SoundManager.Instance;
        judgementManager = JudgementManager.Instance;

        if (soundManager == null)
        {
            Debug.LogError("SoundManager를 찾을 수 없습니다!");
        }

        if (judgementManager == null)
        {
            Debug.LogError("JudgementManager를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        CheckInput();
    }

    private void CheckInput()
    {
        // W 또는 ↑
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlayerAnimationController.Instance?.PlayAttackAnimation(NoteType.Up);
            ProcessInput(NoteType.Up);
        }

        // S 또는 ↓
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerAnimationController.Instance?.PlayAttackAnimation(NoteType.Down);
            ProcessInput(NoteType.Down);
        }

        // A 또는 ←
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerAnimationController.Instance?.PlayAttackAnimation(NoteType.Left);
            ProcessInput(NoteType.Left);
        }

        // D 또는 →
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerAnimationController.Instance?.PlayAttackAnimation(NoteType.Right);
            ProcessInput(NoteType.Right);
        }
    }

    private void ProcessInput(NoteType inputType)
    {
        float currentTime = soundManager.SongPosition;

        // 해당 타입의 가장 가까운 노트 찾기
        Note closestNote = NoteController.GetClosestNote(inputType, currentTime, MaxInputWindow);

        if (closestNote != null)
        {
            float timeDiff = currentTime - closestNote.TargetTime;

            // 판정 처리
            JudgementType judgement = judgementManager.ProcessJudgement(closestNote, timeDiff);

            // 노트 위치 저장
            Vector3 notePosition = closestNote.NoteObject != null
                ? closestNote.NoteObject.transform.position
                : Vector3.zero;

            // 노트 Hit 처리
            NoteController.OnNoteHit(closestNote, timeDiff, judgement);

            // PlayerController에 판정 전달
            PlayerController.Instance?.OnJudgementReceived(judgement);

            // UI에 판정 표시
            UIManager.Instance?.ShowJudgement(judgement);

            // 카메라 흔들림
            CameraShake.Instance?.ShakeByJudgement(judgement);

            // 파티클 효과
            ParticleManager.Instance?.PlayParticle(judgement, notePosition);

            Debug.Log($"입력 성공: {inputType} - {judgement}");
        }
        else
        {
            // 입력했지만 맞는 노트가 없음
            Debug.Log($"빈 입력: {inputType}");
        }
    }
}