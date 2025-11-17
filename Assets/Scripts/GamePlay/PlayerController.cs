using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("플레이어 초당 이동 속도")]
    public float moveSpeed = 5f;

    [Tooltip("플레이어 가로 폭 절반 값")]
    public float playerHalfWidth = 0.5f; // 화면 경계를 계산할 때 사용

    private float _minX; // 플레이어가 움직일 수 있는 최소 x좌표(왼쪽 끝)
    private float _maxX; // 플레이어가 움직일 수 있는 최대 x좌표(오른쪽 끝)
    private float _moveDirection = 0f; // 현재 이동 방향을 저장 (1: 오른쪽, -1: 왼쪽, 0: 멈춤)

    void Start()
    {
        // 카메라의 세로 크기 (전체 높이의 절반)
        float cameraHalfHeight = Camera.main.orthographicSize;

        // 카메라의 가로 크기 (전체 폭의 절반)
        float cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;

        // 플레이어가 갈 수 있는 왼쪽 끝 좌표
        _minX = -cameraHalfWidth + playerHalfWidth;

        // 플레이어가 갈 수 있는 오른쪽 끝 좌표
        _maxX = cameraHalfWidth - playerHalfWidth;
    }

    void Update()
    {
        // 왼쪽 버튼 or 오른쪽 버튼을 누른다면
        if (_moveDirection != 0f)
        {
            // 이동량을 계산하고 플레이어 위치에 적용
            transform.Translate(Vector3.right * _moveDirection * moveSpeed * Time.deltaTime);

            // 플레이어의 현재 위치 정보
            Vector3 currentPosition = transform.position;

            // 플레이어의 x좌표가 화면 밖으로 나가지 않도록
            currentPosition.x = Mathf.Clamp(currentPosition.x, _minX, _maxX);

            // 보정된 위치 값으로 플레이어의 실제 위치를 다시 설정
            transform.position = currentPosition;
        }
    }

    /// <summary>
    /// 왼쪽 버튼을 눌렀을 때 호출
    /// </summary>
    public void OnLeftButtonDown()
    {
        _moveDirection = -1f;
    }

    /// <summary>
    /// 왼쪽 버튼에서 손을 뗐을 때 호출
    /// </summary>
    public void OnLeftButtonUp()
    {
        if (_moveDirection == -1f)
        {
            _moveDirection = 0f;
        }
    }

    /// <summary>
    /// 오른쪽 버튼을 눌렀을 때 호출
    /// </summary>
    public void OnRightButtonDown()
    {
        _moveDirection = 1f;
    }

    /// <summary>
    /// 오른쪽 버튼에서 손을 뗐을 때 호출
    /// </summary>
    public void OnRightButtonUp()
    {
        if (_moveDirection == 1f)
        {
            _moveDirection = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 예외 처리
        if (GameManager.Instance == null)
            return;

        // 황금 똥과 충돌
        if (other.CompareTag("Golden Poop"))
        {
            GameManager.Instance.AddGoldenPoop(1);
            Destroy(other.gameObject);
        }

        // 일반 똥, 대왕 똥과 충돌
        else if (other.CompareTag("Basic Poop") || other.CompareTag("Giant Poop"))
        {
            GameManager.Instance.ShowQuiz();
            Destroy(other.gameObject);
        }
    }
}