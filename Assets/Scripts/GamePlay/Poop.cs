using UnityEngine;

public class Poop : MonoBehaviour
{
    private Rigidbody2D _rigid;
    private Vector2 _normalVelocity; // 멈추기 직전의 속도

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 7f);
    }

    private void Start()
    {
        if (gameObject.CompareTag("Giant Poop"))
        {
            Resize();
        }
    }

    /// <summary>
    /// 대왕 똥을 화면 가로 크기에 딱 맞게(혹은 꽉 차게) 늘려주는 함수
    /// </summary>
    void Resize()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // 화면의 세로 높이
        float worldScreenHeight = Camera.main.orthographicSize * 2f;

        // 화면의 가로 너비
        float worldScreenWidth = worldScreenHeight * Camera.main.aspect;

        // 대왕 똥 원본 가로 길이
        float spriteWidth = sr.sprite.bounds.size.x;

        // 화면 가로 길이를 이미지 가로 길이로 나눠서 확대 비율 계산
        float scaleFactor = worldScreenWidth / spriteWidth;

        // 계산된 비율을 적용
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    /// <summary>
    /// 똥 오브젝트의 움직임을 멈추거나 다시 움직이도록 하는 함수
    /// </summary>
    /// <param name="isPaused"></param>
    public void SetPaused(bool isPaused)
    {
        // 예외 처리
        if (_rigid == null)
            return;

        // 현재 속도를 저장, 움직임을 멈춤
        if (isPaused)
        {
            _normalVelocity = _rigid.linearVelocity;
            _rigid.simulated = false;
        }

        // 원래 속도 적용, 다시 움직임
        else
        {
            _rigid.simulated = true;
            _rigid.linearVelocity = _normalVelocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            if (GameManager.Instance != null)
            {
                // 일반 똥이 땅에 닿아서 사라지면 점수 +1
                if (gameObject.CompareTag("Basic Poop"))
                {
                    GameManager.Instance.AddScore(1);
                }

                // 황금 똥이 땋에 닿아서 사라지면 점수 -1
                else if (gameObject.CompareTag("Golden Poop"))
                {
                    GameManager.Instance.AddScore(-1);
                }
            }
            Destroy(gameObject);
        }
    }
}