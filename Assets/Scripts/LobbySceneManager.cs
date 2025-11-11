using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    public static LobbySceneManager Instance;

    [Header("# Panels")]
    [SerializeField, Tooltip("메인 패널")] GameObject _mainPanel;
    [SerializeField, Tooltip("옷장 패널")] GameObject _costumePanel;
    [SerializeField, Tooltip("출석 패널")] GameObject _dailyPanel;

    [Header("# Main Panel")]
    [SerializeField, Tooltip("최고점수")] TMP_Text _highScoreText;
    [SerializeField, Tooltip("로고")] Image _logoImage;
    [SerializeField, Tooltip("시작 버튼")] Button _startButton;
    [SerializeField, Tooltip("옷장 버튼")] Button _costumeButton;
    //[SerializeField, Tooltip("출석 버튼")] Button _dailyButton;

    [Header("# Costume Panel")]
    [SerializeField, Tooltip("닫기 버튼")] Button _costumeCloseButton;
    [SerializeField, Tooltip("황금똥")] TMP_Text _goldAmountText;
    [SerializeField, Tooltip("이름 설정")] Button _nameSettingButton;
    [SerializeField, Tooltip("플레이어 이미지(머리)")] Image _playerHeadImage;
    [SerializeField, Tooltip("플레이어 이미지(상의)")] Image _playerBodyImage;
    [SerializeField, Tooltip("플레이어 이미지(하의)")] Image _playerLegImage;
    [SerializeField, Tooltip("카테고리 선택")] Button[] _categoryButtons; // 0: 머리, 1: 상의, 2: 하의
    [SerializeField, Tooltip("아이템 슬롯")] GameObject[] _itemSlots;
    Color _selectedColor = new Color(0.8f, 0.8f, 0.8f);
    Color _defaultColor = Color.white;

    [Header("# Daily Panel")]
    [SerializeField, Tooltip("닫기 버튼")] Button _dailyCloseButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 메인 패널 버튼 리스너 등록
        _startButton.onClick.AddListener(StartGame);
        _costumeButton.onClick.AddListener(OpenCostumePanel);
        //_dailyButton.onClick.AddListener(OpenDailyPanel);

        // 옷장 패널 버튼 리스너 등록
        _costumeCloseButton.onClick.AddListener(CloseCostumePanel);
        for (int i = 0; i < _categoryButtons.Length; i++)
        {
            int index = i;
            _categoryButtons[i].onClick.AddListener(() => SelectCategory(index));
        }

        // 출석 패널 버튼 리스너 등록
        _dailyCloseButton.onClick.AddListener(CloseDailyPanel);

        // 초기화
        _mainPanel.SetActive(true);
        _costumePanel.SetActive(false);
        _dailyPanel.SetActive(false);
        SelectCategory(0); // 기본 카테고리 선택: 머리
    }

    /*-----메인 패널-----*/
    void StartGame()
    {
        // TODO: 씬 이동
    }

    void OpenCostumePanel()
    {
        _costumePanel.SetActive(true);
        _mainPanel.SetActive(false);
    }

    void OpenDailyPanel()
    {
        _dailyPanel.SetActive(true);
        _mainPanel.SetActive(false);
    }

    /*-----옷장 패널-----*/
    void CloseCostumePanel()
    {
        _costumePanel.SetActive(false);
        _mainPanel.SetActive(true);
    }

    void SelectCategory(int category)   // 0: 머리, 1: 상의, 2: 하의
    {
        // 모든 카테고리 버튼 색상 초기화
        _categoryButtons[0].GetComponent<Image>().color = _defaultColor;
        _categoryButtons[1].GetComponent<Image>().color = _defaultColor;
        _categoryButtons[2].GetComponent<Image>().color = _defaultColor;

        switch (category)
        {
            case 0:
                // 머리 아이템 슬롯 활성화
                _categoryButtons[0].GetComponent<Image>().color = _selectedColor;
                CostumeManager.Instance.LoadCostumeItems(CostumeType.Head);
                break;
            case 1:
                // 상의 아이템 슬롯 활성화
                _categoryButtons[1].GetComponent<Image>().color = _selectedColor;
                CostumeManager.Instance.LoadCostumeItems(CostumeType.Body);
                break;
            case 2:
                // 하의 아이템 슬롯 활성화
                _categoryButtons[2].GetComponent<Image>().color = _selectedColor;
                CostumeManager.Instance.LoadCostumeItems(CostumeType.Legs);
                break;
        }
    }

    /*-----출석 패널-----*/
    void CloseDailyPanel()
    {
        _dailyPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }
}
