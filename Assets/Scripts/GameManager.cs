using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentScore = 0;
    public int goldenPoops = 0;
    public GameObject quizPanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 이 씬(GameScene) 안에서만 쓸 거면 필요 없음
        }
    }

    public void ShowQuiz()
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(true); // 퀴즈 패널 활성화
            Time.timeScale = 0f;       // 게임 일시정지
        }

        // TODO: 퀴즈 내용 표시, 타이머 시작 등 QuizManager 로직 호출
    }

    public void HideQuiz(bool isCorrect) // 퀴즈 정답/오답 여부에 따라 호출 (나중에 구현)
    {
        Debug.Log("퀴즈 패널을 닫습니다.");
        if (quizPanel != null)
        {
            quizPanel.SetActive(false); // 퀴즈 패널 비활성화
            Time.timeScale = 1f;        // 배경 게임 재개
        }

        if (isCorrect)
        {
            currentScore += 5;
            AddGoldenPoop(1);
            Debug.Log("정답! 현재 점수: " + currentScore);
        }
        else
        {
            Debug.Log("오답! 게임 오버 처리");
            
            // TODO: 게임 오버 화면 띄우기 함수 호출
            // ShowGameOver(); 
        }
    }

    public void AddGoldenPoop(int amount)
    {
        goldenPoops += amount;
        Debug.Log("현재 황금똥: " + goldenPoops);

        // TODO: UI 업데이트 (Text 컴포넌트에 현재 황금똥 개수 표시)
    }
}