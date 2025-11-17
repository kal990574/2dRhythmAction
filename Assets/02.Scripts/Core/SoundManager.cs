using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Header("BPM Settings")]
    public float BPM = 120f;
    public float MusicStartDelay = 2f;

    [Header("Audio")]
    public AudioClip MusicClip;

    private AudioSource audioSource;
    private float songPosition;      // 현재 음악 재생 위치 
    private float secPerBeat;        // 1비트당 시간
    private float dspSongTime;       // DSP 기준 음악 시작 시간

    public float SongPosition => songPosition;
    public float SecPerBeat => secPerBeat;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        secPerBeat = 60f / BPM;
    }

    private void Start()
    {
        audioSource.clip = MusicClip;
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
}