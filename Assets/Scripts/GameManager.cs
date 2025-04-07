using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 게임의 전반적인 상태를 관리하는 싱글톤 클래스
/// 점수, 게임 속도, UI 상태 등을 관리합니다.
/// </summary>
public enum GameState
{
    Intro,
    Playing,
    Dead
}

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }
    
    [Header("UI References")]
    public Text scoreText;        // 현재 점수를 표시하는 UI 텍스트
    public Text highScoreText;    // 최고 점수를 표시하는 UI 텍스트
    public GameObject deadUI;      // Dead UI 오브젝트
    
    [Header("Game Settings")]
    public float initialSpeed = 5f;           // 게임 시작 시 초기 속도
    public float speedIncreaseRate = 0.1f;    // 시간에 따른 속도 증가율
    public float maxSpeed = 15f;              // 최대 속도 제한
    
    [Header("Spawner References")]
    public GameObject foodSpawner;    // Food Spawner 오브젝트
    public GameObject enemySpawner;   // Enemy Spawner 오브젝트
    public GameObject introUI;        // IntroUI 오브젝트
    
    public GameState currentState { get; private set; } = GameState.Intro;
    public float currentScore { get; private set; }
    public float highScore { get; private set; }
    public float scoreMultiplier { get; private set; } = 1f;

    public float currentSpeed { get; private set; }
    
    /// <summary>
    /// 싱글톤 패턴 구현을 위한 Awake 메서드
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
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
        if (currentState == GameState.Playing)
        {
            UpdateScore();
            UpdateSpeed();
        }
    }
    
    /// <summary>
    /// 게임의 초기 상태를 설정합니다.
    /// </summary>
    public void InitializeGame()
    {
        currentState = GameState.Intro;
        currentScore = 0f;
        scoreMultiplier = 1f;
        currentSpeed = initialSpeed;
        highScore = PlayerPrefs.GetFloat("HighScore", 0);  // 저장된 최고 점수 불러오기
        Time.timeScale = 1f;
        
        if (deadUI != null)
        {
            deadUI.SetActive(false);
        }
        
        // 인트로 상태: 적/음식 스포너 비활성화
        SetSpawnersState(false, false);
        
        // IntroUI 활성화
        if (introUI != null)
        {
            introUI.SetActive(true);
        }
        
        UpdateUI();
    }
    
    /// <summary>
    /// 음식과 적 스포너의 상태를 설정합니다.
    /// </summary>
    private void SetSpawnersState(bool foodActive, bool enemyActive)
    {
        Debug.Log($"SetSpawnersState 호출: 음식={foodActive}, 적={enemyActive}");
        
        // 직접 레퍼런스 사용
        if (foodSpawner != null)
        {
            foodSpawner.SetActive(foodActive);
            Debug.Log($"Food Spawner 상태 변경: {foodActive}");
        }
        else
        {
            Debug.LogError("Food Spawner 레퍼런스가 없습니다!");
        }
        
        if (enemySpawner != null)
        {
            enemySpawner.SetActive(enemyActive);
            Debug.Log($"Enemy Spawner 상태 변경: {enemyActive}");
        }
        else
        {
            Debug.LogError("Enemy Spawner 레퍼런스가 없습니다!");
        }
    }
    
    /// <summary>
    /// 점수를 업데이트합니다.
    /// </summary>
    private void UpdateScore()
    {
        if (currentState == GameState.Playing)
        {
            currentScore += Time.deltaTime * scoreMultiplier;
        }
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
            scoreText.text = $"Score: {Mathf.Floor(currentScore)}";
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
        Debug.Log("GameOver 호출됨!");
        currentState = GameState.Dead;
        
        // 화면에 남아있는 적과 음식 오브젝트 제거
        DestroyRemainingObjects();
        
        // 데드 상태: 음식/적 스포너 비활성화
        SetSpawnersState(false, false);
        
        // 최고 점수 갱신 체크
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetFloat("HighScore", highScore);
            Debug.Log($"최고 점수 갱신: {highScore}");
        }

        // DeadUI 표시
        if (deadUI != null)
        {
            Debug.Log("직접 레퍼런스로 deadUI 활성화");
            
            // 먼저 오브젝트 활성화
            deadUI.SetActive(true);
            Debug.Log($"deadUI 활성화 상태: {deadUI.activeSelf}");
            
            // 약간의 딜레이 후 DeadUI 구성 요소 찾기 (객체가 완전히 활성화되는데 시간이 필요할 수 있음)
            StartCoroutine(SetupDeadUIWithDelay());
        }
        else
        {
            Debug.LogError("deadUI 레퍼런스가 null입니다!");
        }
    }
    
    private System.Collections.IEnumerator SetupDeadUIWithDelay()
    {
        // 한 프레임 기다리기
        yield return null;
        
        DeadUI deadUIComponent = deadUI.GetComponent<DeadUI>();
        if (deadUIComponent != null)
        {
            Debug.Log("DeadUI 컴포넌트를 찾음, ShowDeadUI 호출");
            deadUIComponent.ShowDeadUI(currentScore, highScore);
        }
        else
        {
            Debug.LogError("deadUI에 DeadUI 컴포넌트가 없습니다!");
        }
    }
    
    /// <summary>
    /// 화면에 남아있는 적과 음식 오브젝트를 모두 제거합니다.
    /// </summary>
    private void DestroyRemainingObjects()
    {
        // 태그가 "enemy"인 모든 오브젝트 찾아서 제거
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Debug.Log($"적 오브젝트 제거: {enemy.name}");
            Destroy(enemy);
        }
        
        // 태그가 "food"인 모든 오브젝트 찾아서 제거
        GameObject[] foods = GameObject.FindGameObjectsWithTag("food");
        foreach (GameObject food in foods)
        {
            Debug.Log($"음식 오브젝트 제거: {food.name}");
            Destroy(food);
        }
        
        // 태그가 "golden"인 모든 오브젝트 찾아서 제거
        GameObject[] goldenFoods = GameObject.FindGameObjectsWithTag("golden");
        foreach (GameObject golden in goldenFoods)
        {
            Debug.Log($"골든 음식 오브젝트 제거: {golden.name}");
            Destroy(golden);
        }
        
        Debug.Log("남아있는 모든 오브젝트 제거 완료");
    }
    
    /// <summary>
    /// 현재 게임 속도를 반환합니다.
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void StartGame()
    {
        Debug.Log("StartGame 호출됨!");
        currentState = GameState.Playing;
        currentScore = 0f;
        scoreMultiplier = 1f;
        
        Debug.Log("플레이 상태로 전환: 모든 스포너 활성화 시도");
        // 플레이 상태: 모든 스포너 활성화
        SetSpawnersState(true, true);
        
        // IntroUI 비활성화
        if (introUI != null)
        {
            introUI.SetActive(false);
            Debug.Log("IntroUI 비활성화");
        }
        else
        {
            Debug.LogError("introUI 레퍼런스가 없습니다!");
        }
        
        Debug.Log("게임 시작 완료!");
    }

    public float GetScore()
    {
        return currentScore;
    }

    public void AddScore(float amount)
    {
        if (currentState != GameState.Playing) return;  // 게임 시작 전이나 게임오버 상태에서는 점수 증가 안함
        
        currentScore += amount;
    }
} 