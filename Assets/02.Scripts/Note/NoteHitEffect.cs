using UnityEngine;
using System.Collections;

public class NoteHitEffect : MonoBehaviour
{
    [Header("Hit Effect Settings")]
    public float FlySpeed = 20f;
    public float FlyDuration = 1f;
    public AnimationCurve FlySpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public void PlayHitEffect(NoteType noteType, JudgementType judgement)
    {
        Vector3 flyDirection = GetFlyDirection(judgement);
        StartCoroutine(HitEffectCoroutine(flyDirection));
    }

    private Vector3 GetFlyDirection(JudgementType judgement)
    {
        Vector3 direction = Vector3.zero;
        direction = new Vector3(Random.Range(10f, 15f), Random.Range(-1f, 10f), 0);

        // 판정에 따라 속도 배율
        float speedMultiplier = judgement switch
        {
            JudgementType.Perfect => 1.5f,  // 더 강하게 날아감
            JudgementType.Good => 1.0f,
            _ => 0.5f
        };

        return direction.normalized * speedMultiplier;
    }

    private IEnumerator HitEffectCoroutine(Vector3 flyDirection)
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        while (elapsed < FlyDuration)
        {
            float t = elapsed / FlyDuration;
            float curveValue = FlySpeedCurve.Evaluate(t);

            // 날아가기
            transform.position += flyDirection * FlySpeed * curveValue * Time.deltaTime;

            // 페이드아웃
            if (spriteRenderer != null)
            {
                Color color = startColor;
                color.a = 1f - t;
                spriteRenderer.color = color;
            }

            // 회전 효과 (선택 사항)
            transform.Rotate(0, 0, 360f * Time.deltaTime * 3f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 효과 끝나면 삭제
        Destroy(gameObject);
    }
}