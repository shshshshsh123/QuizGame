using UnityEngine;
using UnityEngine.UI; // Button, Image 등 UI 컴포넌트 사용
using TMPro; // TextMeshPro (최신 텍스트 컴포넌트) 사용
using System.Collections; // Coroutine (카운트다운) 사용
using System.Collections.Generic; // List<T> 사용

// [System.Serializable]을 붙여야 Inspector 창에서 보입니다.
[System.Serializable]
public class QuizItem
{
    public string question; // 퀴즈 질문
    public bool answerIsO;  // 정답이 O면 true, X면 false
}

public class QuizManager : MonoBehaviour
{
    [Header("퀴즈 UI 요소")]
    public TextMeshProUGUI questionText; // 퀴즈 질문을 표시할 텍스트
    public TextMeshProUGUI countdownText; // 5초 카운트다운을 표시할 텍스트
    public Button oButton; // O 버튼
    public Button xButton; // X 버튼

    [Header("퀴즈 데이터")]
    public List<QuizItem> quizList; // 퀴즈 목록 (Inspector에서 직접 입력)

    private QuizItem currentQuestion; // 현재 출제된 퀴즈
    private Coroutine countdownCoroutine; // 카운트다운 코루틴 저장 변수
    private GameManager gameManager;

    // Start는 이 오브젝트가 활성화될 때 한 번 호출됩니다.
    void Start()
    {
        // GameManager의 인스턴스를 찾아둡니다.
        gameManager = GameManager.Instance;
    }

    // 이 오브젝트(QuizPanel)가 활성화될 때마다 호출됩니다. (매우 중요!)
    void OnEnable()
    {
        // 1. 새로운 퀴즈를 랜덤하게 선택
        currentQuestion = quizList[Random.Range(0, quizList.Count)];

        // 2. 퀴즈 질문 UI 업데이트
        questionText.text = currentQuestion.question;

        // 3. 5초 카운트다운 시작
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // 기존 코루틴이 있다면 중지
        }
        countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    // 5초 카운트다운 코루틴
    IEnumerator CountdownCoroutine()
    {
        float timer = 5.0f;

        while (timer > 0f)
        {
            // 텍스트 업데이트 (소수점 올림)
            countdownText.text = Mathf.CeilToInt(timer).ToString();

            // Time.timeScale이 0이므로, unscaledDeltaTime을 사용해야 합니다.
            timer -= Time.unscaledDeltaTime;

            yield return null; // 1프레임 대기
        }

        // 5초가 지나면 오답 처리
        countdownText.text = "0";
        CheckAnswer(false, true); // (시간 초과로 오답)
    }

    // O 버튼을 눌렀을 때 호출될 함수
    public void OnOButtonPressed()
    {
        CheckAnswer(true, false); // (O를 선택함, 시간 초과 아님)
    }

    // X 버튼을 눌렀을 때 호출될 함수
    public void OnXButtonPressed()
    {
        CheckAnswer(false, false); // (X를 선택함, 시간 초과 아님)
    }

    // 정답/오답 확인
    // playerChoseO: 플레이어가 O를 선택했으면 true, X를 선택했으면 false
    // timeOver: 시간 초과면 true
    private void CheckAnswer(bool playerChoseO, bool timeOver)
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // 카운트다운 중지
            countdownCoroutine = null;
        }

        bool isCorrect = false;

        if (timeOver)
        {
            isCorrect = false; // 시간 초과는 무조건 오답
        }
        else
        {
            // 정답 확인 (플레이어 선택 == 정답)
            isCorrect = (playerChoseO == currentQuestion.answerIsO);
        }

        // GameManager에게 퀴즈가 끝났음을 알림 (정답/오답 여부 전달)
        if (gameManager != null)
        {
            gameManager.HideQuiz(isCorrect);
        }
    }
}