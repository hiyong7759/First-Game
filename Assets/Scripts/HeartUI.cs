using UnityEngine;

public class HeartUI : MonoBehaviour
{
    public Sprite heartOn;    // 생명력이 있을 때 이미지
    public Sprite heartOff;   // 생명력이 없을 때 이미지
    public SpriteRenderer[] hearts;    // 하트 스프라이트 배열

    private PlayerController player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
        }
        
        // 최대 생명력만큼 하트 이미지 초기화
        UpdateHearts();
    }

    private void Update()
    {
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        // 각 하트 이미지 업데이트
        for (int i = 0; i < hearts.Length; i++)
        {
            // 현재 생명력보다 작은 인덱스는 on 이미지
            if (i < player.currentHealth)
            {
                hearts[i].sprite = heartOn;
            }
            // 현재 생명력보다 큰 인덱스는 off 이미지
            else
            {
                hearts[i].sprite = heartOff;
            }
        }
    }
} 