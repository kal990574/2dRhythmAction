using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
    [Header("BPM Pulse Settings")]
    public bool EnablePulse = true;
    public float PulseAmount = 0.03f;
    public AnimationCurve PulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector3 baseScale;
    private SoundManager soundManager;

    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        
        sr.drawMode = SpriteDrawMode.Simple;

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Camera.main.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size;

        baseScale = new Vector3(
            worldScreenWidth / spriteSize.x,
            worldScreenHeight / spriteSize.y,
            1f
        );

        transform.localScale = baseScale;

        // SoundManager 참조
        soundManager = SoundManager.Instance;
    }

    void Update()
    {
        if (!EnablePulse || soundManager == null)
            return;

        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        float beatProgress = GetBeatProgress();
        float curveValue = PulseCurve.Evaluate(beatProgress);
        float scale = 1f + (PulseAmount * curveValue);

        transform.localScale = baseScale * scale;
    }

    private float GetBeatProgress()
    {
        float beatDuration = 60f / soundManager.BPM;
        float beatPosition = soundManager.SongPosition / beatDuration;
        return beatPosition % 1f;
    }
}