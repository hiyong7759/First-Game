using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private Camera mainCamera;
    private float screenWidthInUnits;
    private float objectWidth;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 카메라의 정확한 너비를 월드 유닛으로 계산
        float cameraHeight = 2f * mainCamera.orthographicSize;
        screenWidthInUnits = cameraHeight * mainCamera.aspect;
        
        // 스프라이트의 실제 너비 계산
        objectWidth = spriteRenderer.bounds.size.x;
    }

    // Update is called once per frame
    private void Update()
    {
        // 오브젝트가 화면 왼쪽 경계를 완전히 벗어났을 때 제거
        if (transform.position.x < (-screenWidthInUnits / 2f) - (objectWidth / 2f))
        {
            Destroy(gameObject);
        }
    }
}
