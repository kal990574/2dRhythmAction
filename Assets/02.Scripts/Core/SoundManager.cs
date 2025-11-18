using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BPM Settings")]
    public float BPM = 135f;
    public float MusicStartDelay = 1f;

    [Header("Audio")]
    public AudioClip MusicClip;

    [Header("Hit Sounds")]
    public AudioClip PerfectHitSound;
    public AudioClip GoodHitSound;

    [Header("Hit Sound Settings")]
    public float HitSoundVolume = 2f;
    public float PitchVariationAmount = 0.15f;

    private AudioSource audioSource;
    private AudioSource hitSoundSource;
    private float songPosition;      // 현재 음악 재생 위치
    private float secPerBeat;        // 1비트당 시간
    private float dspSongTime;       // DSP 기준 음악 시작 시간

    public float SongPosition => songPosition;
    public float SecPerBeat => secPerBeat;

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
            return;
        }

        audioSource = GetComponent<AudioSource>();
        secPerBeat = 60f / BPM;
    }

    private void Start()
    {
        audioSource.clip = MusicClip;

        // 히트 사운드용 AudioSource 생성
        hitSoundSource = gameObject.AddComponent<AudioSource>();
        hitSoundSource.playOnAwake = false;
        hitSoundSource.volume = HitSoundVolume;

        // 레이턴시 최소화 설정
        hitSoundSource.spatialBlend = 0f;  // 2D 사운드
        hitSoundSource.priority = 0;        // 최고 우선순위
        hitSoundSource.bypassEffects = true;  // 이펙트 무시
        hitSoundSource.bypassListenerEffects = true;
        hitSoundSource.bypassReverbZones = true;
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            // DSP 시간 기반으로 정확한 음악 위치 계산
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - MusicStartDelay);
        }
    }

    public void PlayMusic()
    {
        if (MusicClip == null)
        {
            Debug.LogWarning("MusicClip이 할당되지 않았습니다!");
            return;
        }

        dspSongTime = (float)AudioSettings.dspTime;
        audioSource.PlayScheduled(AudioSettings.dspTime + MusicStartDelay);
        Debug.Log($"음악 재생 시작 (BPM: {BPM}, Delay: {MusicStartDelay}초)");
    }

    public void StopMusic()
    {
        audioSource.Stop();
        songPosition = 0f;
    }

    public float GetTimeAtBeat(int beatNumber)
    {
        return beatNumber * secPerBeat;
    }

    public void PlayHitSound(JudgementType judgement, NoteType noteType)
    {
        if (hitSoundSource == null)
            return;

        // 판정에 따라 사운드 선택
        AudioClip clip = GetHitSoundClip(judgement);

        if (clip == null)
            return;

        // 노트 타입에 따라 Pitch 변조
        float pitch = GetPitchForNoteType(noteType);

        // 사운드 재생
        hitSoundSource.pitch = pitch;
        hitSoundSource.PlayOneShot(clip, HitSoundVolume);
    }

    private AudioClip GetHitSoundClip(JudgementType judgement)
    {
        return judgement switch
        {
            JudgementType.Perfect => PerfectHitSound,
            JudgementType.Good => GoodHitSound,
            _ => null  // Miss는 사운드 없음
        };
    }

    private float GetPitchForNoteType(NoteType noteType)
    {
        // 기본 Pitch에서 노트 타입별로 약간씩 변조
        return noteType switch
        {
            NoteType.Up => 1.0f + PitchVariationAmount,      // +0.15 (높은음)
            NoteType.Down => 1.0f - PitchVariationAmount,    // -0.15 (낮은음)
            NoteType.Left => 1.0f - (PitchVariationAmount * 0.5f),  // -0.075
            NoteType.Right => 1.0f + (PitchVariationAmount * 0.5f), // +0.075
            _ => 1.0f
        };
    }
}