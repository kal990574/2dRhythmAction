using UnityEngine;
using System.Collections.Generic;

public class NoteController : MonoBehaviour
{
    [Header("Note Settings")]
    public NoteChart NoteChart;
    public float NoteTravelTime = 5f;

    [Header("Spawn & Judge Position")]
    public Transform SpawnPosition;
    public Transform JudgeLinePosition;

    [Header("Random Y Position")]
    public bool RandomizeYPosition = true;
    public float MinYOffset = -2f;
    public float MaxYOffset = 2f;

    [Header("Note Prefabs")]
    public GameObject NotePrefabUp;
    public GameObject NotePrefabDown;
    public GameObject NotePrefabLeft;
    public GameObject NotePrefabRight;

    private List<Note> activeNotes = new List<Note>();
    private int nextNoteIndex = 0;
    private SoundManager soundManager;
    private float noteSpeed;

    private void Start()
    {
        soundManager = SoundManager.Instance;

        if (soundManager == null)
        {
            Debug.LogError("SoundManager를 찾을 수 없습니다!");
            return;
        }

        // 노트 이동 속도 계산
        float distance = Vector3.Distance(SpawnPosition.position, JudgeLinePosition.position);
        noteSpeed = distance / NoteTravelTime;

        // 모든 노트의 SpawnTime 계산
        if (NoteChart != null)
        {
            foreach (var note in NoteChart.Notes)
            {
                note.CalculateSpawnTime(NoteTravelTime);
            }
            Debug.Log($"노트 SpawnTime 계산 완료");
        }
    }

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        SpawnNotes();
        MoveNotes();
        CheckMissedNotes();
    }

    private void SpawnNotes()
    {
        if (NoteChart == null || nextNoteIndex >= NoteChart.Notes.Count)
            return;

        float currentTime = soundManager.SongPosition;

        while (nextNoteIndex < NoteChart.Notes.Count)
        {
            Note note = NoteChart.Notes[nextNoteIndex];

            if (currentTime >= note.SpawnTime)
            {
                SpawnNote(note);
                nextNoteIndex++;
            }
            else
            {
                break;
            }
        }
    }

    private void SpawnNote(Note note)
    {
        GameObject prefab = GetNotePrefab(note.Type);
        if (prefab == null)
        {
            Debug.LogWarning($"노트 프리팹이 없습니다: {note.Type}");
            return;
        }

        Vector3 spawnPos = SpawnPosition.position;

        // Y값 랜덤화
        if (RandomizeYPosition)
        {
            float randomY = Random.Range(MinYOffset, MaxYOffset);
            spawnPos.y += randomY;
        }

        GameObject noteObj = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        note.NoteObject = noteObj;
        activeNotes.Add(note);

        Debug.Log($"노트 생성: {note.Type} | SpawnTime: {note.SpawnTime:F2}초 | TargetTime: {note.TargetTime:F2}초 | CurrentTime: {soundManager.SongPosition:F2}초");
    }

    private void MoveNotes()
    {
        foreach (var note in activeNotes)
        {
            if (note.NoteObject != null)
            {
                note.NoteObject.transform.position += Vector3.left * noteSpeed * Time.deltaTime;
            }
        }
    }

    private void CheckMissedNotes()
    {
        float currentTime = soundManager.SongPosition;

        // 음악 시작 전에는 Miss 체크 안 함
        if (currentTime < 0)
            return;

        float missThreshold = 0.15f;

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            Note note = activeNotes[i];

            if (note.IsProcessed || currentTime < note.TargetTime)
                continue;

            if (currentTime > note.TargetTime + missThreshold)
            {
                OnNoteMissed(note);
            }
        }
    }

    private void OnNoteMissed(Note note)
    {
        note.IsProcessed = true;
        Debug.Log($"Miss! Type: {note.Type}");

        // Miss 판정 전달
        PlayerController.Instance?.OnJudgementReceived(JudgementType.Miss);
        UIManager.Instance?.ShowJudgement(JudgementType.Miss);

        DestroyNote(note);
    }

    public void OnNoteHit(Note note, float timeDiff, JudgementType judgement)
    {
        note.IsProcessed = true;
        Debug.Log($"Hit! Type: {note.Type}, Diff: {timeDiff:F3}초");

        // Hit 효과 재생
        if (note.NoteObject != null)
        {
            NoteHitEffect hitEffect = note.NoteObject.GetComponent<NoteHitEffect>();
            if (hitEffect != null)
            {
                hitEffect.PlayHitEffect(note.Type, judgement);
                // activeNotes에서만 제거 (GameObject는 HitEffect가 알아서 삭제)
                activeNotes.Remove(note);
            }
            else
            {
                // HitEffect가 없으면 바로 삭제
                DestroyNote(note);
            }
        }
        else
        {
            activeNotes.Remove(note);
        }
    }

    private void DestroyNote(Note note)
    {
        if (note.NoteObject != null)
        {
            Destroy(note.NoteObject);
        }
        activeNotes.Remove(note);
    }

    private GameObject GetNotePrefab(NoteType type)
    {
        return type switch
        {
            NoteType.Up => NotePrefabUp,
            NoteType.Down => NotePrefabDown,
            NoteType.Left => NotePrefabLeft,
            NoteType.Right => NotePrefabRight,
            _ => null
        };
    }

    public Note GetClosestNote(NoteType type, float currentTime, float maxTimeDiff)
    {
        Note closestNote = null;
        float minDiff = float.MaxValue;

        foreach (var note in activeNotes)
        {
            if (note.IsProcessed || note.Type != type)
                continue;

            float diff = Mathf.Abs(currentTime - note.TargetTime);

            if (diff < minDiff && diff <= maxTimeDiff)
            {
                minDiff = diff;
                closestNote = note;
            }
        }

        return closestNote;
    }
}