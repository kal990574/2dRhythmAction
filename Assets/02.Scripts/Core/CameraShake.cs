using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Settings")]
    public float DefaultDuration = 0.15f;
    public float DefaultMagnitude = 0.1f;

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

    public void Shake()
    {
        Shake(DefaultDuration, DefaultMagnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }
    }

    public void ShakeByJudgement(JudgementType judgement)
    {
        switch (judgement)
        {
            case JudgementType.Perfect:
                Shake(0.2f, 0.15f);  // 강한 흔들림
                break;

            case JudgementType.Good:
                Shake(0.15f, 0.1f);  // 중간 흔들림
                break;

            case JudgementType.Miss:
                // Miss는 흔들림 없음
                break;
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