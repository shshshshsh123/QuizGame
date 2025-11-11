using UnityEngine;
using System.Collections;

public class PoopSpawner : MonoBehaviour
{
    [Header("일반 똥, 황금 똥")]

    [Tooltip("일반 똥, 황금 똥 프리팹")]
    public GameObject[] normalPoopPrefabs;

    [Tooltip("일반 똥, 황금 똥 생성 주기")]
    public float normalSpawnInterval = 1.0f;

    [Header("대왕 똥")]

    [Tooltip("대왕 똥 프리팹")]
    public GameObject giantPoopPrefab;

    [Tooltip("대왕 똥 생성 주기")]
    public float giantSpawnInterval = 10.0f;

    private float _cameraHalfWidth;

    void Start()
    {
        // 카메라 가로 절반 폭을 계산
        float cameraHalfHeight = Camera.main.orthographicSize;
        _cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;

        // 일반 똥, 황금 똥 생성 코루틴 시작
        StartCoroutine(SpawnNormalPoopsRoutine());

        // 대왕똥 생성 코루틴 시작
        StartCoroutine(SpawnGiantPoopRoutine());
    }

    /// <summary>
    /// 일반 똥 생성 코루틴 (X값은 랜덤으로 설정)
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnNormalPoopsRoutine()
    {
        // 게임 시작 후 바로 스폰되지 않도록 잠시 대기 (1초)
        yield return new WaitForSeconds(normalSpawnInterval);

        while (true)
        {
            // 화면 가로 범위 내에서 X값을 랜덤으로 설정
            float randomX = Random.Range(-_cameraHalfWidth, _cameraHalfWidth);
            Vector3 spawnPosition = new Vector3(randomX, transform.position.y, 0);

            // 일반 똥, 황금 똥 중에서 랜덤으로 선택
            int randomIndex = Random.Range(0, normalPoopPrefabs.Length);
            GameObject selectedPoop = normalPoopPrefabs[randomIndex];

            Instantiate(selectedPoop, spawnPosition, Quaternion.identity);

            // 다음 스폰까지 대기(1초)
            yield return new WaitForSeconds(normalSpawnInterval);
        }
    }

    /// <summary>
    /// 대왕 똥 생성 코루틴 (X값을 0으로 고정(화면 중앙)시켜서 피할 수 없도록 설정)
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnGiantPoopRoutine()
    {
        // 게임 시작 후 바로 스폰되지 않도록 잠시 대기 (5초)
        yield return new WaitForSeconds(giantSpawnInterval / 2);

        while (true)
        {
            // 대왕똥은 항상 화면 중앙 (X=0)에서 생성
            Vector3 spawnPosition = new Vector3(0, transform.position.y, 0);

            if (giantPoopPrefab != null)
            {
                Instantiate(giantPoopPrefab, spawnPosition, Quaternion.identity);
            }

            // 다음 스폰까지 대기 (10초)
            yield return new WaitForSeconds(giantSpawnInterval);
        }
    }
}