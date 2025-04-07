using UnityEngine;
using TMPro;

public class DeadUI : MonoBehaviour
{
    public TextMeshPro scoreText;
    public TextMeshPro messageText;
    public TMP_FontAsset koreanFont; // 인스펙터에서 할당
    private bool isActive = false;

    private void Start()
    {
        if (scoreText == null || messageText == null)
        {
            Debug.LogError("텍스트 컴포넌트가 연결되지 않았습니다!");
        }
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    public void ShowDeadUI(float currentScore, float highScore)
    {
        Debug.Log("DeadUI 표시 시도");
        
        // 오브젝트 활성화 상태 확인
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Debug.Log("DeadUI 오브젝트 활성화됨");
        }
        
        isActive = true;
        
        // 텍스트 컴포넌트 확인 및 설정
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {currentScore:F0}\nHIGH SCORE: {highScore:F0}";
            Debug.Log($"점수 텍스트 설정: {scoreText.text}");
            
            // 텍스트 컴포넌트 상태 확인
            Debug.Log($"scoreText 활성화 상태: {scoreText.gameObject.activeSelf}, 렌더러 활성화: {scoreText.isActiveAndEnabled}");
        }
        else
        {
            Debug.LogError("scoreText가 null입니다! DeadUI 오브젝트에 TextMeshPro 컴포넌트를 확인하세요.");
        }
        
        if (messageText != null)
        {
            string message = currentScore < highScore ? "좀더 잘해봐~\n[SPACE] 다시하기" : "최고 점수 달성!\n[SPACE] 다시하기";
            messageText.text = message;
            Debug.Log($"메시지 텍스트 설정: {messageText.text}");
            
            // 텍스트 컴포넌트 상태 확인
            Debug.Log($"messageText 활성화 상태: {messageText.gameObject.activeSelf}, 렌더러 활성화: {messageText.isActiveAndEnabled}");
        }
        else
        {
            Debug.LogError("messageText가 null입니다! DeadUI 오브젝트에 TextMeshPro 컴포넌트를 확인하세요.");
        }

        // 텍스트 스타일 설정
        SetupTextComponent(scoreText, 36, Color.white);
        SetupTextComponent(messageText, 30, Color.white);
        
        // 카메라 방향으로 회전
        transform.forward = Camera.main.transform.forward;
        
        Debug.Log("DeadUI 설정 완료!");
        
        // 몇 초 후에 다시 확인 로그 출력
        Invoke("CheckUIStatus", 0.5f);
    }
    
    private void CheckUIStatus()
    {
        Debug.Log($"DeadUI 상태 확인 - 활성화: {gameObject.activeSelf}, isActive: {isActive}");
        if (scoreText != null)
        {
            Debug.Log($"scoreText 상태: {scoreText.text}, 활성화: {scoreText.gameObject.activeSelf}");
        }
        if (messageText != null)
        {
            Debug.Log($"messageText 상태: {messageText.text}, 활성화: {messageText.gameObject.activeSelf}");
        }
    }

    private void RestartGame()
    {
        isActive = false;
        gameObject.SetActive(false);
        
        // 플레이어 초기화
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ResetPlayer();
        }
        
        // 게임 매니저 초기화
        GameManager.Instance.StartGame();
    }

    private void SetupTextComponent(TextMeshPro tmp, float fontSize, Color color)
    {
        if (tmp != null)
        {
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.transform.forward = Camera.main.transform.forward;
            
            // 한글 폰트 적용
            if (koreanFont != null)
            {
                tmp.font = koreanFont;
                Debug.Log("한글 폰트 적용 성공!");
            }
            else
            {
                Debug.LogWarning("한글 폰트가 설정되지 않았습니다. 인스펙터에서 koreanFont 필드에 한글 TMP_FontAsset을 할당해주세요.");
            }
        }
    }
} 