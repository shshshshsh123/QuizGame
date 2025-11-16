using UnityEngine;
using System.Collections;

public class PoopSpawner : MonoBehaviour
{
    [Header("일반 똥, 황금 똥")]

    [Tooltip("일반 똥, 황금똥 프리팹")]
    public GameObject[] smallpoopPrefabs;

    [Tooltip("일반 똥, 황금 똥 생성 간격")]
    public float spawnInterval = 1.0f;

    [Header("대왕 똥")]

    [Tooltip("대왕 똥 프리팹")]
    public GameObject giantPoopPrefab;

    [Tooltip("대왕 똥 생성 간격")]
    public float giantSpawnInterval = 10.0f;

    [Header("시간에 따른 난이도 설정")]

    [Tooltip("일반 똥, 황금 똥 최소 스폰 간격")]
    public float minNormalSpawnInterval = 0.5f;

    [Tooltip("대왕 똥의 최소 스폰 간격")]
    public float minGiantSpawnInterval = 5.0f;

    [Tooltip("최대 난이도에 도달하기까지 걸리는 시간")]
    public float timeToReachMaxDifficulty = 120.0f;

    private float cameraHalfWidth; // 똥이 생성될 X축 범위
    private bool isPaused = false; // 게임의 일시정지 상태

    private float _initialNormalInterval; // 게임 시작 시 일반 똥, 황금 똥 생성 간격
    private float _initialGiantInterval; // 게임 시작 시 대왕 똥 생성 간격
    private float _startTime; // 게임 시작 시간

    void Start()
    {
        // 카메라의 세로 크기 (전체 높이의 절반)
        float cameraHalfHeight = Camera.main.orthographicSize;

        // 카메라의 가로 크기 (전체 폭의 절반)
        cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;

        // 시작 시간 및 초기 생성 간격 저장
        _startTime = Time.time;
        _initialNormalInterval = spawnInterval;
        _initialGiantInterval = giantSpawnInterval;

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

        // 난이도 (경과 시간 0초 = 0%, 경과 시간 60초 = 50%, 경과 시간 120초 = 100%)
        float difficulty = elapsedTime / timeToReachMaxDifficulty;

        // 최대 난이도를 달성하면 난이도 계속 유지
        difficulty = Mathf.Clamp01(difficulty);

        // 계산된 비율로 생성 간격 설정 (똥 생성 속도가 점점 빨라지도록)
        spawnInterval = Mathf.Lerp(_initialNormalInterval, minNormalSpawnInterval, difficulty);
        giantSpawnInterval = Mathf.Lerp(_initialNormalInterval, minGiantSpawnInterval, difficulty);
    }

    /// <summary>
    /// 똥 생성을 멈추거나 재개
    /// </summary>
    /// <param name="pause"></param>
    public void SetPaused(bool pause)
    {
        isPaused = pause;
    }

    /// <summary>
    /// 일반 똥, 황금 똥 생성 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnNormalPoops()
    {
        yield return new WaitForSeconds(spawnInterval);

        while (true)
        {
            // 일시정지 상태라면 실행 X
            if (isPaused)
            {
                yield return null;
                continue;
            }

            // 화면 왼쪽 끝부터 화면 오른쪽 끝사이의 랜덤한 X좌표 설정
            float randomX = Random.Range(-cameraHalfWidth, cameraHalfWidth);

            // 똥이 생성될 위치
            Vector3 spawnPosition = new Vector3(randomX, transform.position.y, 0);

            // 일반 똥, 황금 똥 중 랜덤으로 결정
            int randomIndex = Random.Range(0, smallpoopPrefabs.Length);

            // 생성
            Instantiate(smallpoopPrefabs[randomIndex], spawnPosition, Quaternion.identity);

            // 생성 간격만큼 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// 대왕 똥 생성 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnGiantPoop()
    {
        yield return new WaitForSeconds(giantSpawnInterval / 2);

        while (true)
        {
            // 일시정지 상태라면 실행 X
            if (isPaused)
            {
                yield return null;
                continue;
            }

            // 똥이 생성될 위치 (일반 똥, 황금 똥과 달리 X좌표를 항상 화면 중앙으로 고정)
            Vector3 spawnPosition = new Vector3(0, transform.position.y, 0);
            
            // 생성
            Instantiate(giantPoopPrefab, spawnPosition, Quaternion.identity);

            // 생성 간격만큼 대기
            yield return new WaitForSeconds(giantSpawnInterval);
        }
    }
}