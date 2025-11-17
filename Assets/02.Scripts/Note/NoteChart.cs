using UnityEngine;
using System.Collections.Generic;

public class NoteChart : MonoBehaviour
{
    [Header("Chart Info")]
    public float BPM = 120f;

    [Header("Auto Generation")]
    public bool AutoGenerate = true;
    public int AutoNoteCount = 30;
    public int StartBeat = 10;
    public int BeatsInterval = 2;
    public float NoteTravelTime = 5f;

    [Header("Notes")]
    public List<Note> Notes = new List<Note>();

    private void Start()
    {
        // 자동 생성 모드
        if (AutoGenerate)
        {
            GenerateRandomNotes(AutoNoteCount);
        }

        // 노트, TargetTime 기준 정렬
        SortNotes();
        Debug.Log($"NoteChart 로드 완료: {Notes.Count}개 노트");
    }

    public void SortNotes()
    {
        Notes.Sort((a, b) => a.TargetTime.CompareTo(b.TargetTime));
    }

    public void AddNoteAtBeat(NoteType type, int beatNumber)
    {
        float secPerBeat = 60f / BPM;
        float targetTime = beatNumber * secPerBeat;
        Notes.Add(new Note(type, targetTime));
    }

    public void ClearNotes()
    {
        Notes.Clear();
    }

    public void GenerateRandomNotes(int noteCount)
    {
        Notes.Clear();

        for (int i = 0; i < noteCount; i++)
        {
            int beat = StartBeat + (i * BeatsInterval);
            NoteType randomType = (NoteType)Random.Range(0, 4);
            AddNoteAtBeat(randomType, beat);
        }

        // SpawnTime 계산
        foreach (var note in Notes)
        {
            note.CalculateSpawnTime(NoteTravelTime);
        }

        Debug.Log($"{noteCount}개 랜덤 노트 자동 생성 완료! (시작: {StartBeat}비트)");
    }
}