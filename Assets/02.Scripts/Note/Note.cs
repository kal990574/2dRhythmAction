using UnityEngine;

[System.Serializable]
public class Note
{
    public NoteType Type;
    public float TargetTime;        // judge까지 시간
    public float SpawnTime;         // 노트 생성 시간
    public bool IsProcessed = false; // hit 처리
    public GameObject NoteObject; 
    
    public Note(NoteType type, float targetTime)
    {
        Type = type;
        TargetTime = targetTime;
    }

    public void CalculateSpawnTime(float travelTime)
    {
        SpawnTime = TargetTime - travelTime;
    }
}