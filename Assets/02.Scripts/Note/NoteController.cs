using UnityEngine;
using System.Collections.Generic;

public class NoteController : MonoBehaviour
{
    [Header("Note Settings")]
    public NoteChart NoteChart;
    public float NoteTravelTime = 2f;

    [Header("Spawn & Judge Position")]
    public Transform SpawnPosition;    
    public Transform JudgeLinePosition; 

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

        GameObject noteObj = Instantiate(prefab, SpawnPosition.position, Quaternion.identity, transform);
        note.NoteObject = noteObj;
        activeNotes.Add(note);

        Debug.Log($"노트 생성: {note.Type} at {soundManager.SongPosition:F2}초");
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

        DestroyNote(note);
    }

    public void OnNoteHit(Note note, float timeDiff)
    {
        note.IsProcessed = true;
        Debug.Log($"Hit! Type: {note.Type}, Diff: {timeDiff:F3}초");
        DestroyNote(note);
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