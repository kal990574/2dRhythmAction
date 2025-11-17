using UnityEngine;

public class JudgementManager : MonoBehaviour
{
    public static JudgementManager Instance { get; private set; }

    [Header("Judgement Timing Windows")]
    public float PerfectWindow = 0.05f;
    public float GoodWindow = 0.1f;
    public float MissWindow = 0.15f;

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

    public JudgementType ProcessJudgement(Note note, float timeDiff)
    {
        float absDiff = Mathf.Abs(timeDiff);
        JudgementType judgement;

        if (absDiff <= PerfectWindow)
        {
            judgement = JudgementType.Perfect;
        }
        else if (absDiff <= GoodWindow)
        {
            judgement = JudgementType.Good;
        }
        else
        {
            judgement = JudgementType.Miss;
        }

        Debug.Log($"판정: {judgement} (차이: {timeDiff:F3}초)");

        return judgement;
    }
}