using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TextMeshPro scoreText;
    private GameManager gameManager;

    private void Start()
    {
        scoreText = GetComponent<TextMeshPro>();
        if (scoreText == null)
        {
            Debug.LogError("TextMeshPro 컴포넌트가 없습니다!");
            return;
        }

        gameManager = GameManager.Instance;
        
        // TextMeshPro 설정
        scoreText.alignment = TextAlignmentOptions.Left;
        scoreText.fontSize = 36;
        scoreText.color = Color.white;
        
        // 초기 점수 표시
        UpdateScore(0);
    }

    private void Update()
    {
        if (scoreText == null || gameManager == null) return;
        UpdateScore(gameManager.GetScore());
    }

    private void UpdateScore(float score)
    {
        scoreText.text = $"SCORE: {score:F0}";
    }
} 