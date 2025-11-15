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

    /// <summary>
    /// 똥 오브젝트의 움직임을 멈추거나 다시 움직이도록 하는 함수
    /// </summary>
    /// <param name="isPaused"></param>
    public void SetPaused(bool isPaused)
    {
        // 예외 처리
        if (_rigid == null)
            return;

        // 현재 속도를 저장하고 Kinematic으로 변경해서 움직임을 멈춤
        if (isPaused)
        {
            _normalVelocity = _rigid.linearVelocity;
            _rigid.bodyType = RigidbodyType2D.Kinematic;
            _rigid.linearVelocity = Vector2.zero;
        }

        // Dynamic으로 변경하고 원래 속도 적용
        else
        {
            _rigid.bodyType = RigidbodyType2D.Dynamic;
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