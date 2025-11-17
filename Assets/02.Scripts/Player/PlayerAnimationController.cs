using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public static PlayerAnimationController Instance { get; private set; }

    [Header("Animator")]
    public Animator animator;

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
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator를 찾을 수 없습니다!");
        }
    }

    public void PlayAttackAnimation(NoteType noteType)
    {
        if (animator == null)
            return;

        switch (noteType)
        {
            case NoteType.Up:
            case NoteType.Down:
                // W, S → HitB
                animator.SetTrigger("HitB");
                Debug.Log("HitB 애니메이션 재생");
                break;

            case NoteType.Left:
            case NoteType.Right:
                // A, D → HitA
                animator.SetTrigger("HitA");
                Debug.Log("HitA 애니메이션 재생");
                break;
        }
    }
}