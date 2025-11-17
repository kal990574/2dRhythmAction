using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [Header("Particle Prefabs")]
    public GameObject PerfectParticlePrefab;
    public GameObject GoodParticlePrefab;
    public GameObject MissParticlePrefab;

    [Header("Spawn Position")]
    public Transform ParticleSpawnPosition;

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
        }
    }

    public void PlayParticle(JudgementType judgement, Vector3 position)
    {
        GameObject particlePrefab = GetParticlePrefab(judgement);

        if (particlePrefab == null)
        {
            Debug.LogWarning($"파티클 프리팹이 없습니다: {judgement}");
            return;
        }

        // 파티클 생성
        GameObject particleObj = Instantiate(particlePrefab, position, Quaternion.identity);

        // 파티클 시스템 재생
        ParticleSystem ps = particleObj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();

            // 재생 후 자동 삭제
            Destroy(particleObj, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            // ParticleSystem이 없으면 2초 후 삭제
            Destroy(particleObj, 2f);
        }
    }

    public void PlayParticleAtJudgeLine(JudgementType judgement)
    {
        Vector3 position = ParticleSpawnPosition != null
            ? ParticleSpawnPosition.position
            : Vector3.zero;

        PlayParticle(judgement, position);
    }

    private GameObject GetParticlePrefab(JudgementType judgement)
    {
        return judgement switch
        {
            JudgementType.Perfect => PerfectParticlePrefab,
            JudgementType.Good => GoodParticlePrefab,
            JudgementType.Miss => MissParticlePrefab,
            _ => null
        };
    }
}