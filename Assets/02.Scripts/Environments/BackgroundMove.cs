using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundMove : MonoBehaviour
{
    [Header("Fit To Camera")]
    public bool FitToCamera = true;

    [Header("BPM Pulse Settings")]
    public bool EnablePulse = true;
    public float PulseAmount = 0.06f;
    public AnimationCurve PulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Scroll Settings")]
    public bool EnableScroll = true;
    public float ScrollSpeed = 0.5f;
    public Vector2 ScrollDirection = Vector2.left;

    private Material material;
    private Vector2 offset;
    private Vector3 baseScale;
    private SoundManager soundManager;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer를 찾을 수 없습니다!");
            return;
        }

        spriteRenderer.drawMode = SpriteDrawMode.Simple;

        if (FitToCamera)
        {
            float worldScreenHeight = Camera.main.orthographicSize * 2f;
            float worldScreenWidth = worldScreenHeight * Camera.main.aspect;

            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

            baseScale = new Vector3(
                worldScreenWidth / spriteSize.x,
                worldScreenHeight / spriteSize.y,
                1f
            );

            transform.localScale = baseScale;
        }
        else
        {
            baseScale = transform.localScale;
        }

        if (EnablePulse)
        {
            soundManager = SoundManager.Instance;
        }

        // 스크롤 기능을 위한 Material 설정
        if (EnableScroll)
        {
            material = new Material(spriteRenderer.material);
            spriteRenderer.material = material;

            if (spriteRenderer.sprite != null && spriteRenderer.sprite.texture != null)
            {
                spriteRenderer.sprite.texture.wrapMode = TextureWrapMode.Repeat;

                Debug.Log("BackgroundMove 스크롤 초기화 완료");
            }
            else
            {
                Debug.LogError("Sprite 또는 Texture가 없습니다!");
            }

            offset = Vector2.zero;
        }
    }

    private void Update()
    {
        // BPM 펄스 효과
        if (EnablePulse && soundManager != null)
        {
            if (GameManager.Instance?.CurrentState == GameState.Playing)
            {
                float beatProgress = GetBeatProgress();
                float curveValue = PulseCurve.Evaluate(beatProgress);
                float scale = 1f + (PulseAmount * curveValue);
                transform.localScale = baseScale * scale;
            }
        }

        // 스크롤 효과
        if (EnableScroll && material != null)
        {
            offset += ScrollDirection * ScrollSpeed * Time.deltaTime;
            material.mainTextureOffset = offset;
        }
    }

    private float GetBeatProgress()
    {
        float beatDuration = 60f / soundManager.BPM;
        float beatPosition = soundManager.SongPosition / beatDuration;
        return beatPosition % 1f;
    }
}
