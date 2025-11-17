using UnityEngine;
using System.Collections.Generic;

public class NoteChart : MonoBehaviour
{
    [Header("Chart Info")]
    public float BPM = 120f;

    [Header("Notes")]
    public List<Note> Notes = new List<Note>();

    private void Start()
    {
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
}