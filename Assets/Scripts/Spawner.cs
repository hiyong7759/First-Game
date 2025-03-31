using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("생성할 오브젝트")]
    public GameObject[] spawnPrefabs;

    [Header("Spawn Settings")]
    [Tooltip("최소 스폰 간격 (초)")]
    public float minSpawnDelay = 1f;

    [Tooltip("최대 스폰 간격 (초)")]
    public float maxSpawnDelay = 1.5f;

    private Camera mainCamera;
    private float screenWidthInUnits;
    private float nextSpawnTime;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // 카메라의 너비를 월드 유닛으로 계산
        float cameraHeight = 2f * mainCamera.orthographicSize;
        screenWidthInUnits = cameraHeight * mainCamera.aspect;

        // 첫 스폰 시간 설정
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            Spawn();
            SetNextSpawnTime();
        }
    }

    private void Spawn()
    {
        if (spawnPrefabs.Length == 0) return;

        // 랜덤한 프리팹 선택
        int randomIndex = Random.Range(0, spawnPrefabs.Length);
        GameObject prefab = spawnPrefabs[randomIndex];

        // 스폰 위치 계산
        float objectWidth = prefab.GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = (screenWidthInUnits / 2f) + (objectWidth / 2f);
        spawnPosition.y = prefab.transform.position.y;  // 프리팹의 원래 y축 값 사용
        spawnPosition.z = prefab.transform.position.z;  // 프리팹의 원래 z축 값 사용

        // 오브젝트 생성
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = spawnPosition;
    }

    private void SetNextSpawnTime()
    {
        // 다음 스폰 시간을 랜덤하게 설정
        nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
    }
}
