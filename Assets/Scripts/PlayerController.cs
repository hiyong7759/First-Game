using UnityEngine;

/// <summary>
/// 플레이어 캐릭터의 움직임과 상태를 관리하는 클래스
/// 점프, 이동, 데미지 처리 등을 담당합니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;        // 점프 힘

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
        Debug.Log("초기 점프 횟수: " + jumpCount);
    }
    
    /// <summary>
    /// 매 프레임마다 실행되는 업데이트
    /// 지면 체크, 점프 입력 처리
    /// </summary>
    private void Update()
    {
        // 점프 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 첫 번째 점프는 지면에 있을 때만 가능
            if (isGrounded)
            {
                Jump();
                jumpCount = 1;
                Debug.Log("첫 번째 점프! 점프 횟수: " + jumpCount);
            }
            // 두 번째 점프는 공중에서 가능 (이중 점프)
            else if (jumpCount < MAX_JUMPS)
            {
                Jump();
                jumpCount = MAX_JUMPS;  // 최대 점프 횟수에 도달했음을 표시
                Debug.Log("두 번째 점프! 점프 횟수: " + jumpCount);
            }
            else
            {
                Debug.Log("점프 불가! 최대 점프 횟수 도달. 현재 점프 횟수: " + jumpCount);
            }
        }

        // 착지 감지: 이전에는 공중에 있었는데 지금은 땅에 있는 경우
        if (!wasGrounded && isGrounded)
        {
            animator.SetInteger(ANIM_STATE, STATE_LAND);
            Debug.Log("착지 애니메이션 재생!");
        }

        // 현재 상태를 저장
        wasGrounded = isGrounded;
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
            Debug.Log("지면에 착지! 점프 횟수 초기화: " + jumpCount);
            // 착지 애니메이션은 Update에서 처리하므로 여기서는 설정하지 않음
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
}