using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Tooltip("현재 점수")]
    public int currentScore = 0;

    [Tooltip("황금 똥 개수")]
    public int goldenPoops = 0;

    [Header("UI 패널")]
    public GameObject quizPanel;
    public GameObject gameOverPanel;
    public GameObject inGameUI;
    public GameObject gameOverUI;

    [Header("UI 텍스트")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goldenPoopText;
    public TextMeshProUGUI finalScoreText;

    [Header("# Player Costume")]
    public List<SpriteRenderer> playerCostumes;

    [Header("# Costume Data Database")]
    [SerializeField] CostumeSO[] _headCostumeDataList;
    [SerializeField] CostumeSO[] _bodyCostumeDataList;
    [SerializeField] CostumeSO[] _legsCostumeDataList;

    private PoopSpawner _poopSpawner;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // 기기에 저장된 황금 똥 개수를 불러오거나, 저장된 값이 없다면 0
        goldenPoops = DataManager.Instance.GoldAmount;

        // 게임이 시작될 때 UI 점수 텍스트 초기화
        UpdateScore();
        
        // 게임이 시작될 때 UI 황금 똥 텍스트 초기화
        UpdateGoldenPoop();

        _poopSpawner = Object.FindFirstObjectByType<PoopSpawner>();

        // UI 설정
        if (inGameUI != null)
        {
            inGameUI.SetActive(true);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // 플레이어 코스튬 입히기
        ApplySavedCostumes();
    }

    private void ApplySavedCostumes()
    {
        if (DataManager.Instance == null) return;

        // 1. DataManager에서 저장된 ID 가져오기
        int headId = DataManager.Instance.GetEquippedItemId(CostumeType.Head);
        int bodyId = DataManager.Instance.GetEquippedItemId(CostumeType.Body);
        int legsId = DataManager.Instance.GetEquippedItemId(CostumeType.Legs);

        // 2. ID에 해당하는 SO 파일 찾기
        CostumeSO headSO = GetCostumeByID(headId, _headCostumeDataList);
        CostumeSO bodySO = GetCostumeByID(bodyId, _bodyCostumeDataList);
        CostumeSO legsSO = GetCostumeByID(legsId, _legsCostumeDataList);

        // 3. 플레이어 렌더러에 이미지 갈아끼우기
        // playerCostumes[0]: 머리, [1]: 상의, [2]: 하의
        if (playerCostumes.Count >= 3)
        {
            if (headSO != null) playerCostumes[0].sprite = headSO.costumeSprite;
            if (bodySO != null) playerCostumes[1].sprite = bodySO.costumeSprite;
            if (legsSO != null) playerCostumes[2].sprite = legsSO.costumeSprite;
        }
    }

    private CostumeSO GetCostumeByID(int id, CostumeSO[] list)
    {
        if (list == null) return null;
        return list.FirstOrDefault(x => x.costumeID == id);
    }

    /// <summary>
    /// 플레이어가 일반 똥, 대왕 똥과 충돌했을 때
    /// </summary>
    public void ShowQuiz()
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
        }

        // 똥의 움직임과 똥 생성 멈춤
        PauseGame(true);
    }

    /// <summary>
    /// 퀴즈가 끝났을 때 호출
    /// </summary>
    /// <param name="isCorrect"></param>
    public void HideQuiz(bool isCorrect)
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        // 똥의 움직임과 똥 생성 다시 시작
        PauseGame(false);

        // 퀴즈 결과가 정답이면 점수 +5, 황금 똥 +1
        if (isCorrect)
        {
            AddScore(5);
            AddGoldenPoop(1);
        }

        // 퀴즈 결과가 오답이면 게임 종료
        else
        {
            GameOver();
        }
    }

    /// <summary>
    /// 퀴즈 틀렸을 때 호출
    /// </summary>
    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            int highScore = DataManager.Instance.MaxIQ;

            // 현재 점수가 최고 점수보다 높다면 갱신
            if (currentScore > highScore)
            {
                finalScoreText.text = "최고 IQ 갱신!\nIQ: " + currentScore;
                DataManager.Instance.SetMaxIQ(currentScore);
            }

            // 최종 점수 텍스트에 현재 점수와 최고 점수를 함께 표시 
            else
            {
                finalScoreText.text = "최종 IQ: " + currentScore + "\n최고 IQ: " + highScore;
            }
        }
        
        // 똥의 움직임과 똥 생성 멈춤
        PauseGame(true);

        // UI 설정
        if (inGameUI != null)
        {
            inGameUI.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        DataManager.Instance.Save();
    }

    /// <summary>
    /// 똥의 움직임과 똥의 생성을 멈추는 함수
    /// </summary>
    /// <param name="isPaused"></param>
    public void PauseGame(bool isPaused)
    {
        // 씬에 있는 모든 똥을 찾아서 멈춤/재개
        Poop[] allPoops = Object.FindObjectsByType<Poop>(FindObjectsSortMode.None);

        foreach (Poop poop in allPoops)
        {
            // Poop.cs의 SetPaused 함수를 호출하여 멈추거나 재개
            poop.SetPaused(isPaused);
        }

        // 똥 생성기 멈춤/재개
        if (_poopSpawner != null)
        {
            _poopSpawner.SetPaused(isPaused);
        }
    }

    /// <summary>
    /// 점수 추가
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScore();
    }

    /// <summary>
    /// 황금 똥 추가
    /// </summary>
    /// <param name="amount"></param>
    public void AddGoldenPoop(int amount)
    {
        goldenPoops += amount;
        UpdateGoldenPoop();
        // 황금 똥 개수 저장
        DataManager.Instance.SetGoldAmount(goldenPoops);
    }

    /// <summary>
    /// UI에 표시될 점수를 업데이트
    /// </summary>
    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "IQ: " + currentScore;
    }

    /// <summary>
    /// UI에 표시될 황금 똥의 개수를 업데이트
    /// </summary>
    void UpdateGoldenPoop()
    {
        if (goldenPoopText != null)
            goldenPoopText.text = "황금 똥: " + goldenPoops + "개";
    }

    /// <summary>
    /// 다시하기 버튼 누르면 호출
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 메인메뉴 버튼 누르면 호출
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}