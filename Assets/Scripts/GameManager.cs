using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 전반적인 상태를 관리하는 싱글톤 클래스
/// 점수, 게임 속도, UI 상태 등을 관리합니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }
    
    [Header("UI References")]
    public Text scoreText;        // 현재 점수를 표시하는 UI 텍스트
    public Text highScoreText;    // 최고 점수를 표시하는 UI 텍스트
    public GameObject gameOverPanel;  // 게임 오버 시 표시될 패널
    public Button restartButton;      // 게임 재시작 버튼
    
    [Header("Game Settings")]
    public float initialSpeed = 5f;           // 게임 시작 시 초기 속도
    public float speedIncreaseRate = 0.1f;    // 시간에 따른 속도 증가율
    public float maxSpeed = 15f;              // 최대 속도 제한
    
    private float currentSpeed;    // 현재 게임 속도
    private float score;          // 현재 점수
    private float highScore;      // 최고 점수
    private bool isGameOver;      // 게임 오버 상태
    
    /// <summary>
    /// 싱글톤 패턴 구현을 위한 Awake 메서드
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);  // 중복 인스턴스 제거
        }
    }
    
    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void Start()
    {
        InitializeGame();
    }
    
    /// <summary>
    /// 매 프레임마다 실행되는 업데이트
    /// 점수와 속도를 업데이트합니다.
    /// </summary>
    private void Update()
    {
        if (!isGameOver)
        {
            UpdateScore();
            UpdateSpeed();
        }
    }
    
    /// <summary>
    /// 게임의 초기 상태를 설정합니다.
    /// </summary>
    private void InitializeGame()
    {
        currentSpeed = initialSpeed;
        score = 0;
        highScore = PlayerPrefs.GetFloat("HighScore", 0);  // 저장된 최고 점수 불러오기
        isGameOver = false;
        gameOverPanel.SetActive(false);
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);  // 재시작 버튼 이벤트 연결
        }
        
        UpdateUI();
    }
    
    /// <summary>
    /// 점수를 업데이트합니다.
    /// </summary>
    private void UpdateScore()
    {
        score += Time.deltaTime;  // 시간에 따라 점수 증가
        UpdateUI();
    }
    
    /// <summary>
    /// 게임 속도를 업데이트합니다.
    /// </summary>
    private void UpdateSpeed()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += speedIncreaseRate * Time.deltaTime;  // 시간에 따라 속도 증가
        }
    }
    
    /// <summary>
    /// UI 요소들을 업데이트합니다.
    /// </summary>
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Mathf.Floor(score)}";
        }
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {Mathf.Floor(highScore)}";
        }
    }
    
    /// <summary>
    /// 게임 오버 상태로 전환합니다.
    /// </summary>
    public void GameOver()
    {
        isGameOver = true;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);  // 최고 점수 저장
        }
        gameOverPanel.SetActive(true);
    }
    
    /// <summary>
    /// 게임을 재시작합니다.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// 현재 게임 속도를 반환합니다.
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
} 