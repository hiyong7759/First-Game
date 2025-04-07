using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 관리를 위한 네임스페이스 추가

/// <summary>
/// 플레이어 캐릭터의 움직임과 상태를 관리하는 클래스
/// 점프, 이동, 데미지 처리 등을 담당합니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;        // 점프 힘

    [Header("Health Settings")]
    private int maxHealth = 3;
    public int currentHealth { get; private set; }  // 외부에서 읽기만 가능하도록 수정
    public bool isInvincible = false;  // 무적 상태
    public float invincibleTimer = 0f; // 무적 타이머
    private const float INVINCIBLE_DURATION = 5f; // 무적 지속시간

    private Rigidbody2D rb;
    private Animator animator;          // 애니메이터 컴포넌트
    private bool isGrounded;
    private bool wasGrounded;           // 이전 프레임의 지면 상태
    private int jumpCount = 0;          // 현재 점프 횟수
    private const int MAX_JUMPS = 2;    // 최대 점프 횟수

    // 애니메이션 상태 값
    private const string ANIM_STATE = "state";
    private const int STATE_RUN = 0;
    private const int STATE_JUMP = 1;
    private const int STATE_LAND = 2;

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;  // 생명력 초기화
    }
    
    /// <summary>
    /// 매 프레임마다 실행되는 업데이트
    /// 지면 체크, 점프 입력 처리
    /// </summary>
    private void Update()
    {
        // 무적 시간 업데이트
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
            }
        }

        // Playing 상태일 때만 점프 입력 받기
        if (GameManager.Instance.currentState == GameState.Playing)
        {
            // 점프 입력 처리
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 첫 번째 점프는 지면에 있을 때만 가능
                if (isGrounded)
                {
                    Jump();
                    jumpCount = 1;
                }
                // 두 번째 점프는 공중에서 가능 (이중 점프)
                else if (jumpCount < MAX_JUMPS)
                {
                    Jump();
                    jumpCount = MAX_JUMPS;  // 최대 점프 횟수에 도달했음을 표시
                }
            }
        }

        // 착지 감지: 이전에는 공중에 있었는데 지금은 땅에 있는 경우
        if (!wasGrounded && isGrounded)
        {
            animator.SetInteger(ANIM_STATE, STATE_LAND);
        }

        // 현재 상태를 저장
        wasGrounded = isGrounded;
    }

    private void FixedUpdate()
    {
        // Playing 상태일 때만 물리 업데이트
        if (GameManager.Instance.currentState == GameState.Playing)
        {
            // 물리 업데이트 코드...
        }
    }

    /// <summary>
    /// 점프 동작을 실행합니다.
    /// </summary>
    private void Jump()
    {
        // 현재 수직 속도를 0으로 초기화하고 일정한 점프력 적용
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
        // 점프 애니메이션 상태로 전환
        animator.SetInteger(ANIM_STATE, STATE_JUMP);
        
        // 점프했으므로 공중에 있음을 표시
        isGrounded = false;
    }
    
    /// <summary>
    /// 충돌 처리를 합니다.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 지면과 충돌했을 때
        if (collision.gameObject.name == "Platform")
        {
            isGrounded = true;
            jumpCount = 0;  // 점프 횟수 초기화
        }
    }

    /// <summary>
    /// 트리거 충돌을 처리합니다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 무적이 아닐 때만 적과의 충돌 처리
        if (other.CompareTag("enemy") && !isInvincible)
        {
            ModifyHealth(-1);
            Destroy(other.gameObject);  // 적 제거
        }
        // Golden Baechu와 충돌
        else if (other.CompareTag("golden"))
        {
            isInvincible = true;
            invincibleTimer = INVINCIBLE_DURATION;
            Destroy(other.gameObject);
        }
        // 일반 음식과 충돌
        else if (other.CompareTag("food"))
        {
            ModifyHealth(1);
            Destroy(other.gameObject);
        }
    }
    
    /// <summary>
    /// 다른 콜라이더에서 떨어질 때 호출됩니다.
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 플랫폼에서 떨어질 때
        if (collision.gameObject.name == "Platform")
        {
            isGrounded = false;
            
            // 점프 키를 누르지 않고 떨어지는 경우에도 점프 애니메이션 상태로 전환
            if (animator.GetInteger(ANIM_STATE) != STATE_JUMP)
            {
                animator.SetInteger(ANIM_STATE, STATE_JUMP);
            }
        }
    }

    /// <summary>
    /// 플레이어의 생명력을 수정합니다.
    /// </summary>
    private void ModifyHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        isInvincible = false;
        invincibleTimer = 0f;
        jumpCount = 0;
        isGrounded = false;
        
        // 위치 초기화
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector3(-6f, -2f, 0f); // 시작 위치로
        }
        
        // 애니메이션 초기화
        if (animator != null)
        {
            animator.SetInteger(ANIM_STATE, STATE_RUN);
        }
    }
}