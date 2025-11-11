using UnityEngine;

public class Poop : MonoBehaviour
{
    void Start()
    {
        // 일정 시간이 지나도 똥이 사라지지 않으면 자동으로 파괴
        Destroy(gameObject, 8f); // 8초 후 자동 파괴
    }

    // 다른 Collider 2D와 충돌했을 때 (Is Trigger가 체크된 경우) 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트가 "Ground" 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Ground"))
        {
            // Debug.Log("똥이 땅에 닿았습니다: " + gameObject.name); // 테스트용 로그

            // 똥이 어떤 종류인지에 따라 GameManager에 점수를 알립니다.
            // GameManager가 아직 구현되지 않았다면 이 부분은 주석 처리하거나,
            // 나중에 GameManager 구현 후 연결해주세요.

            // if (GameManager.Instance != null)
            // {
            //     if (gameObject.CompareTag("BasicPoop"))
            //     {
            //         GameManager.Instance.AddScore(1); // 기본똥은 점수 +1
            //     }
            //     else if (gameObject.CompareTag("GoldenPoop"))
            //     {
            //         GameManager.Instance.AddScore(-1); // 황금똥은 점수 -1
            //     }
            //     // GiantPoop은 땅에 닿는 것으로 점수 변화는 없습니다. (피하면 점수 상승)
            // }

            // 똥 오브젝트를 파괴하여 사라지게 합니다.
            Destroy(gameObject);
        }
    }
}