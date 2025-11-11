using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PC에서 키보드 테스트를 위한 이동 속도
    public float moveSpeed = 5f;

    // 플레이어의 가로 폭 절반 (화면 경계 계산에 사용)
    // 인스펙터에서 직접 조절하거나, Start()에서 자동으로 계산할 수 있습니다.
    public float playerHalfWidth = 0.5f;

    // 화면 경계 값 (게임 월드 좌표)
    private float minX, maxX;

    void Start()
    {
        // 메인 카메라의 Orthographic Size (카메라 높이 절반)를 이용해 화면 경계를 계산합니다.
        float cameraHalfHeight = Camera.main.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * Camera.main.aspect; // 화면 가로 절반

        minX = -cameraHalfWidth + playerHalfWidth; // 화면 왼쪽 경계 + 플레이어 절반 폭
        maxX = cameraHalfWidth - playerHalfWidth;  // 화면 오른쪽 경계 - 플레이어 절반 폭
    }

    void Update()
    {
        // ------------------------------------
        // 1. 모바일 터치(드래그) 이동
        // ------------------------------------
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 터치한 위치를 게임 월드 좌표로 변환합니다.
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            // 플레이어의 X축 위치를 터치한 곳으로 이동시킵니다.
            // 이때 X축 위치가 화면 경계 안쪽에 있도록 Clamp합니다.
            float targetX = Mathf.Clamp(touchPosition.x, minX, maxX);

            // Y축(세로)은 현재 위치를 그대로 유지합니다.
            transform.position = new Vector3(targetX, transform.position.y, 0);
        }

        // ------------------------------------
        // 2. PC (에디터)에서 키보드 테스트용 (필요하면 주석 해제)
        // ------------------------------------
         #if UNITY_EDITOR
             float horizontal = Input.GetAxis("Horizontal");
             Vector3 moveVector = Vector3.right * horizontal * moveSpeed * Time.deltaTime;
             transform.Translate(moveVector);

             // PC 테스트 시에도 화면 경계 적용
            Vector3 currentPosition = transform.position;
             currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
             transform.position = currentPosition;
         #endif
    }
}