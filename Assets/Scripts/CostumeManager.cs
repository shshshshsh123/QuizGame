using UnityEngine;
using UnityEngine.UI;

public class CostumeManager : MonoBehaviour
{
    public static CostumeManager Instance;

    [Header("# Costume Manager")]
    [SerializeField, Tooltip("코스튬 아이템 프리팹")] GameObject _costumeItemPrefab;
    [SerializeField, Tooltip("코스튬 아이템들이 배치될 부모 오브젝트")] Transform _costumeItemParent;

    [Header("# Costume Data")]
    [SerializeField, Tooltip("머리 코스튬 데이터 리스트")] CostumeSO[] _headCostumeDataList;
    [SerializeField, Tooltip("상의 코스튬 데이터 리스트")] CostumeSO[] _bodyCostumeDataList;
    [SerializeField, Tooltip("하의 코스튬 데이터 리스트")] CostumeSO[] _legsCostumeDataList;

    [Header("# Player Image")]
    [SerializeField, Tooltip("플레이어 머리 이미지")] Image _playerHeadImage;
    [SerializeField, Tooltip("플레이어 몸통 이미지")] Image _playerBodyImage;
    [SerializeField, Tooltip("플레이어 다리 이미지")] Image _playerLegsImage;

    private CostumeType _currentCostumeType;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        _currentCostumeType = CostumeType.Head;
        LoadCostumeItems(_currentCostumeType);
    }

    public void LoadCostumeItems(CostumeType costumeType)
    {
        _currentCostumeType = costumeType;

        // 기존 아이템 삭제 (버그 방지)
        foreach (Transform child in _costumeItemParent)
        {
            Destroy(child.gameObject);
        }

        CostumeSO[] targetList = costumeType switch
        {
            CostumeType.Head => _headCostumeDataList,
            CostumeType.Body => _bodyCostumeDataList,
            CostumeType.Legs => _legsCostumeDataList,
            _ => null // 예외 처리
        };

        if (targetList == null)
        {
            // Debug.LogWarning($"Costume type '{costumeType}'에 해당하는 리스트가 설정되지 않았습니다.");
            return;
        }

        foreach (var costumeData in targetList)
        {
            GameObject itemObj = Instantiate(_costumeItemPrefab, _costumeItemParent);

            if (itemObj.TryGetComponent<CostumeItem>(out var costumeItem))
            {
                // 아이템 정보 설정 (콜백 함수 포함)
                costumeItem.SetCostumeInfo(costumeData, OnCostumeItemClicked);
            }
        }
    }

    void OnCostumeItemClicked(CostumeSO costumeData)
    {
        if (costumeData.isUnlocked) // 보유중인 코스튬이면???
        {
            EquipCostume(costumeData);
        }
        else
        {
            // 코스튬 구매 로직
            // 여기서는 단순히 잠금 해제만 시도 (TODO: 골드(황금똥) 충분히 보유중인지 + 소모 로직추가)
            costumeData.UnlockCostume();
            LoadCostumeItems(_currentCostumeType); // UI 업데이트
        }
    }

    void EquipCostume(CostumeSO costumeData)
    {
        switch (costumeData.costumeType)
        {
            case CostumeType.Head:
                _playerHeadImage.sprite= costumeData.costumeSprite;
                break;
            case CostumeType.Body:
                _playerBodyImage.sprite = costumeData.costumeSprite;
                break;
            case CostumeType.Legs:
                _playerLegsImage.sprite= costumeData.costumeSprite;
                break;
        }
    }

    public void TestFunction()
    {
        foreach (var costumeData in _headCostumeDataList)
        {
            costumeData.isUnlocked = false;
        }
        foreach (var costumeData in _bodyCostumeDataList)
        {
            costumeData.isUnlocked = false;
        }
        foreach (var costumeData in _legsCostumeDataList)
        {
            costumeData.isUnlocked = false;
        }
        _headCostumeDataList[0].UnlockCostume();
        _bodyCostumeDataList[0].UnlockCostume();
        _legsCostumeDataList[0].UnlockCostume();
        _playerHeadImage.sprite = _headCostumeDataList[0].costumeSprite;
        _playerBodyImage.sprite = _bodyCostumeDataList[0].costumeSprite;
        _playerLegsImage.sprite = _legsCostumeDataList[0].costumeSprite;

        LoadCostumeItems(_currentCostumeType);
    }
}
