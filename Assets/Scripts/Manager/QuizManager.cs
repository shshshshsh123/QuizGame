using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("퀴즈 UI 패널")]

    [Tooltip("질문 텍스트")]
    public TextMeshProUGUI questionText;
    
    [Tooltip("카운트 다운 숫자 표시")]
    public TextMeshProUGUI countdownText;
    
    [Tooltip("O(정답) 영역")]
    public GameObject area_o;
    
    [Tooltip("X(오답) 영역")]
    public GameObject area_x;

    private List<string> _questionList = new List<string>(); // CSV 파일에서 읽어온 퀴즈 질문(문자열)들을 순서대로 저장할 리스트
    private List<bool> _answerList = new List<bool>(); // 퀴즈의 정답(O이면 true, X이면 false)을 순서대로 저장할 리스트
    private int _currentQuizIndex; // 퀴즈가 questionList의 몇 번째 항목인지 그 순번(인덱스)을 저장할 정수형 변수

    private Coroutine _countdown;
    private GameManager _gameManager;
    private GameObject _player;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _player = GameObject.FindGameObjectWithTag("Player");
        LoadQuizData();
    }

    /// <summary>
    /// CSV 파일에서 퀴즈 데이터를 읽어와 리스트에 저장
    /// </summary>
    void LoadQuizData()
    {
        // 두 개의 리스트를 모두 초기화
        _questionList.Clear();
        _answerList.Clear();

        // Resources 폴더의 QuizData 파일을 불러옴
        TextAsset quizDataFile = Resources.Load<TextAsset>("QuizData");

        // 파일을 찾지 못했다면 에러를 출력하고 함수를 종료
        if (quizDataFile == null)
        {
            Debug.LogError("Resources 폴더에서 'QuizData.csv' 파일을 찾을 수 없습니다!");
            return;
        }

        // 불러온 텍스트 파일의 전체 내용을 줄바꿈 문자를 기준으로 쪼개어, 각 줄을 문자열 배열인 lines에 저장
        string[] lines = quizDataFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            // 앞뒤 공백, 줄바꿈 문자를 제거
            string line = lines[i].Trim();

            // 빈 줄이면 무시하고 다음 순서
            if (string.IsNullOrEmpty(line)) continue;

            // 콤마(,) 기준으로 줄을 나눔
            string[] columns = line.Split(',');

            // 데이터가 2개 이상인지 확인 (데이터가 깨진 줄을 걸러내기 위해)
            if (columns.Length >= 2)
            {
                // 질문의 공백을 제거하고 _questionList에 추가
                _questionList.Add(columns[0].Trim());

                // 정답의 공백을 제거하고 대문자로 바꾼 뒤 _answerString에 저장
                string answerString = columns[1].Trim().ToUpper();

                // true 또는 false 값을 answerList 리스트에 추가
                _answerList.Add(answerString == "O");
            }
        }
    }

    /// <summary>
    /// 퀴즈 패널이 활성화될 때마다 호출
    /// </summary>
    void OnEnable()
    {
        // 로드된 퀴즈가 하나도 없다면 즉시 패널을 닫음
        if (_questionList.Count == 0)
        {
            questionText.text = "퀴즈 로딩 실패!";
            
            if (_gameManager != null)
            {
                _gameManager.HideQuiz(false);
            }
            
            return;
        }

        // 출제할 퀴즈의 순번을 랜덤으로 설정
        _currentQuizIndex = Random.Range(0, _questionList.Count);

        // 퀴즈 텍스트를 해당하는 질문으로 변경
        questionText.text = _questionList[_currentQuizIndex];

        // O/X 패널 활성화
        if (area_o != null) area_o.SetActive(true);
        if (area_x != null) area_x.SetActive(true);

        // 이전에 실행 중이던 카운트다운 코루틴이 있다면 중지
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
        }

        // 코루틴 새로 시작
        _countdown = StartCoroutine(Countdown());
    }

    /// <summary>
    /// 퀴즈 패널이 비활성화될 때마다 호출
    /// </summary>
    void OnDisable()
    {
        // O/X 영역 패널을 비활성화
        if (area_o != null) area_o.SetActive(false);
        if (area_x != null) area_x .SetActive(false);

        // 코루틴이 실행 중이라면 정지하고 null로 초기화
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
            _countdown = null;
        }
    }

    /// <summary>
    /// 5초 카운트다운을 처리하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Countdown()
    {
        float timer = 5.0f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            // 소수점 첫째 자리까지 표시
            countdownText.text = timer.ToString("F1");

            // 타이머가 0초 이하가 되면,
            if (timer <= 0f)
            {
                timer = 0f;
                countdownText.text = "0.0";
                // 정답을 확인하는 함수를 호출
                CheckPlayerPosition();
                yield break; // 코루틴 종료
            }
            yield return null;
        }
    }

    /// <summary>
    /// 플레이어의 위치를 확인하여 정답/오답을 판정하는 함수
    /// </summary>
    private void CheckPlayerPosition()
    {
        // 카운트다운 코루틴이 실행 중이라면 중지
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
            _countdown = null;
        }

        // 플레이어가 없으면 즉시 종료
        if (_player == null)
        {
            _gameManager.HideQuiz(false);
            return;
        }

        // 플레이어 오브젝트의 x축 위치 값을 가져옴 (x < 0 이면 O영역(true), x>=0 이면 X영역(false)) 
        bool playerIsInOArea = (_player.transform.position.x < 0);

        // 정답/오답을 판정
        bool isCorrect = (playerIsInOArea == _answerList[_currentQuizIndex]);

        // 최종 정답/오답 결과값 전달
        if (_gameManager != null)
        {
            _gameManager.HideQuiz(isCorrect);
        }
    }
}