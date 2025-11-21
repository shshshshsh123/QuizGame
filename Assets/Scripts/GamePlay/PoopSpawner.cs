using UnityEngine;
using System.Collections;

public class PoopSpawner : MonoBehaviour
{
    [Header("일반 똥, 황금 똥")]

    [Tooltip("일반 똥, 황금똥 프리팹")]
    public GameObject[] smallpoopPrefabs;

    [Tooltip("일반 똥, 황금 똥 생성 간격")]
    public float spawnInterval = 3.0f;

    [Header("대왕 똥")]

    [Tooltip("대왕 똥 프리팹")]
    public GameObject giantPoopPrefab;

    [Tooltip("대왕 똥 생성 간격")]
    public float giantSpawnInterval = 10.0f;

    [Header("시간에 따른 난이도 설정")]

    [Tooltip("일반 똥, 황금 똥 최소 스폰 간격 (가장 빨라졌을 때)")]
    public float minNormalSpawnInterval = 0.5f;

    [Tooltip("대왕 똥의 최소 스폰 간격 (가장 빨라졌을 때)")]
    public float minGiantSpawnInterval = 5.0f;

    [Tooltip("최대 난이도에 도달하기까지 걸리는 시간")]
    public float timeToReachMaxDifficulty = 120.0f;

    [Header("겹침 방지")]

    [Tooltip("대왕 똥 생성 후 일반 똥과 황금 똥의 생성을 멈추는 시간")]
    public float giantPoopSafeTime = 2.0f;

    private float cameraHalfWidth; // 똥이 생성될 X축 범위
    private bool isPaused = false; // 게임의 일시정지 상태

    private float _initialNormalInterval; // 게임 시작 시 일반 똥 생성 간격
    private float _initialGiantInterval; // 게임 시작 시 대왕 똥 생성 간격
    private float _startTime; // 게임 시작 시간

    // 실시간으로 변경하는 값을 저장할 변수
    private float currentNormalInterval;
    private float currentGiantInterval;

    // 대왕 똥이 마지막으로 생성된 시간, -999 = 아직 생성 안됨
    private float lastGiantPoopSpawnTime = -999f;

    void Start()
    {
        // 카메라의 가로 크기
        float cameraHalfHeight = Camera.main.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;

        // 시작 시간 및 초기 생성 간격 저장
        _startTime = Time.time;
        _initialNormalInterval = spawnInterval;
        _initialGiantInterval = giantSpawnInterval;

        // 똥 생성 간격 변수 초기화
        currentNormalInterval = spawnInterval;
        currentGiantInterval = giantSpawnInterval;

        StartCoroutine(SpawnNormalPoops());
        StartCoroutine(SpawnGiantPoop());
    }

    void Update()
    {
        // 일시정지 상태이면 실행 X
        if (isPaused || timeToReachMaxDifficulty <= 0)
        {
            return;
        }

        // 경과 시간
        float elapsedTime = Time.time - _startTime;

        // 난이도 (0.0 ~ 1.0)
        float difficulty = Mathf.Clamp01(elapsedTime / timeToReachMaxDifficulty);

        // 똥 생성 간격 변수의 값을 매 프레임 업데이트
        currentNormalInterval = Mathf.Lerp(_initialNormalInterval, minNormalSpawnInterval, difficulty);
        currentGiantInterval = Mathf.Lerp(_initialGiantInterval, minGiantSpawnInterval, difficulty);
    }

    /// <summary>
    /// 똥 생성을 멈추거나 재개
    /// </summary>
    public void SetPaused(bool pause)
    {
        isPaused = pause;
    }

    /// <summary>
    /// 일반 똥, 황금 똥 생성 코루틴
    /// </summary>
    IEnumerator SpawnNormalPoops()
    {
        float timer = currentNormalInterval;

        while (true)
        {
            // 일시정지 상태면 1프레임 대기
            if (isPaused)
            {
                yield return null;
                continue;
            }

            // 대왕 똥이 나왔는지 확인
            float timeSinceLastGiant = Time.time - lastGiantPoopSpawnTime;

            // 대왕 똥이 나온지 2초가 지나지 않으면 일반 똥과 황금 똥을 생성하지 않음
            if (lastGiantPoopSpawnTime != -999f && timeSinceLastGiant < giantPoopSafeTime)
            {
                yield return null;
                continue;
            }

            // 대기 시간이 지났으면 타이머 감소
            timer -= Time.deltaTime;

            // 타이머가 0이 되면 일반똥, 황금 똥 중 랜덤으로 생성
            if (timer <= 0f)
            {
                float randomX = Random.Range(-cameraHalfWidth, cameraHalfWidth);
                Vector3 spawnPosition = new Vector3(randomX, transform.position.y, 0);
                int randomIndex = Random.Range(0, smallpoopPrefabs.Length);
                Instantiate(smallpoopPrefabs[randomIndex], spawnPosition, Quaternion.identity);

                // 타이머 리셋
                timer = currentNormalInterval + timer;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 대왕 똥 생성 코루틴
    /// </summary>
    IEnumerator SpawnGiantPoop()
    {
        float timer = currentGiantInterval / 2;

        while (true)
        {
            // 일시정지 상태면 1프레임 대기
            if (isPaused)
            {
                yield return null;
                continue;
            }

            timer -= Time.deltaTime;

            // 타이머가 0이 되면 대왕 똥 생성 (대왕 똥은 항상 화면 중앙에서 생성)
            if (timer <= 0f)
            {
                Vector3 spawnPosition = new Vector3(0, transform.position.y, 0);
                Instantiate(giantPoopPrefab, spawnPosition, Quaternion.identity);

                // 대왕똥 생성 시간을 기록
                lastGiantPoopSpawnTime = Time.time;
                timer = currentGiantInterval + timer;
            }

            yield return null;
        }
    }
}