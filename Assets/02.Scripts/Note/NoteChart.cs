using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BeatSection
{
    public float StartTime;      // 구간 시작 시간 (초)
    public float EndTime;        // 구간 끝 시간 (초)
    public int BeatsInterval;    // 이 구간의 비트 간격

    public BeatSection(float startTime, float endTime, int beatsInterval)
    {
        StartTime = startTime;
        EndTime = endTime;
        BeatsInterval = beatsInterval;
    }
}

public class NoteChart : MonoBehaviour
{
    [Header("Chart Info")]
    public float BPM = 270f;

    [Header("Auto Generation")]
    public bool AutoGenerate = true;
    public int AutoNoteCount = 50;
    public int StartBeat = 15;
    public int BeatsInterval = 2;

    [Header("Section-based Generation")]
    public bool UseSections = false;
    public List<BeatSection> BeatSections = new List<BeatSection>();

    [Header("Notes")]
    public List<Note> Notes = new List<Note>();

    private void Start()
    {
        // 구간별 생성 모드 우선
        if (UseSections && BeatSections.Count > 0)
        {
            GenerateNotesFromSections();
        }
        // 자동 생성 모드
        else if (AutoGenerate)
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

        Debug.Log($"{noteCount}개 랜덤 노트 자동 생성 완료! (시작: {StartBeat}비트)");
    }

    public void GenerateNotesFromSections()
    {
        Notes.Clear();

        if (BeatSections.Count == 0)
        {
            Debug.LogWarning("BeatSections가 비어있습니다!");
            return;
        }

        float secPerBeat = 60f / BPM;
        int totalNotes = 0;

        foreach (var section in BeatSections)
        {
            // 시간을 비트로 변환
            int startBeat = Mathf.CeilToInt(section.StartTime / secPerBeat);
            int endBeat = Mathf.FloorToInt(section.EndTime / secPerBeat);

            // 구간 내에서 노트 생성
            for (int beat = startBeat; beat <= endBeat; beat += section.BeatsInterval)
            {
                NoteType randomType = (NoteType)Random.Range(0, 4);
                AddNoteAtBeat(randomType, beat);
                totalNotes++;
            }

            Debug.Log($"구간 [{section.StartTime}~{section.EndTime}초]: {section.BeatsInterval}비트 간격으로 생성");
        }

        Debug.Log($"구간별 노트 생성 완료! 총 {totalNotes}개");
    }
}