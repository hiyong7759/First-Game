using UnityEngine;

/// <summary>
/// 배경을 자동으로 스크롤하는 클래스
/// 게임의 진행 속도에 맞춰 배경이 움직입니다.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
    [Header("Scrolling Settings")]
    public float scrollSpeed = 0.5f;    // 배경 스크롤 속도
    
    private MeshRenderer meshRenderer;
    private Material material;
    private Vector2 offset;
    
    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            material = meshRenderer.material;
            offset = Vector2.zero;
        }
    }
    
    /// <summary>
    /// 매 프레임마다 배경을 스크롤합니다.
    /// 게임 속도에 맞춰 배경이 움직입니다.
    /// </summary>
    private void Update()
    {
        if (material != null)
        {
            offset.x += scrollSpeed * Time.deltaTime;
            material.mainTextureOffset = offset;
        }
    }
    
    /// <summary>
    /// 게임 오브젝트가 비활성화될 때 정리합니다.
    /// </summary>
    private void OnDisable()
    {
        if (material != null)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }
} 