using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Settings")]
    public float PerfectDuration = 0.3f;
    public float PerfectMagnitude = 0.7f;
    public float GoodDuration = 0.2f;
    public float GoodMagnitude = 0.5f;

    private Vector3 originalPosition;
    private bool isShaking = false;

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

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void ShakeByJudgement(JudgementType judgement)
    {
        switch (judgement)
        {
            case JudgementType.Perfect:
                Shake(PerfectDuration, PerfectMagnitude); 
                break;

            case JudgementType.Good:
                Shake(GoodDuration, GoodMagnitude);  
                break;

            case JudgementType.Miss:
                break;
        }
    }
    
    public void Shake(float duration, float magnitude)
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }
}