using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
        RestoreEquippedItems();
    }

    private void RestoreEquippedItems()
    {
        if (DataManager.Instance == null) return;

        int headId = DataManager.Instance.GetEquippedItemId(CostumeType.Head);
        int bodyId = DataManager.Instance.GetEquippedItemId(CostumeType.Body);
        int legsId = DataManager.Instance.GetEquippedItemId(CostumeType.Legs);

        EquipCostume(GetCostumeByID(headId, _headCostumeDataList));
        EquipCostume(GetCostumeByID(bodyId, _bodyCostumeDataList));
        EquipCostume(GetCostumeByID(legsId, _legsCostumeDataList));
    }

    private CostumeSO GetCostumeByID(int id, CostumeSO[] list)
    {
        // 배열에서 ID가 일치하는 첫 번째 요소 반환 (없으면 null)
        return list.FirstOrDefault(x => x.costumeID == id);
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
            // UI를 생성하기 전, DataManager에 저장된 ID인지 확인하여 SO 상태 업데이트
            if (DataManager.Instance != null)
            {
                costumeData.isUnlocked = DataManager.Instance.HasItem(costumeData.costumeID);
            }

            GameObject itemObj = Instantiate(_costumeItemPrefab, _costumeItemParent);

            if (itemObj.TryGetComponent<CostumeItem>(out var costumeItem))
            {
                // costumeItem 내부에서는 costumeData.isUnlocked를 보고 자물쇠 아이콘등 설정
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
            // 미보유 -> 구매 시도
            if (!DataManager.Instance.CanBuy(costumeData.price))
            {
                // 나중에 경고메시지 같은거 추가하면 좋을듯
                return;
            }

            // 1. 데이터 매니저에 획득 정보 저장
            DataManager.Instance.AcquireItem(costumeData.costumeID);
            DataManager.Instance.Save(); // 중요한 정보니 즉시 저장 추천

            // 2. SO 상태도 갱신 (UI 즉각 반영을 위해)
            costumeData.UnlockCostume();

            // 3. UI 새로고침 (잠금 아이콘 해제된 상태로 다시 그리기)
            LoadCostumeItems(_currentCostumeType);
        }
    }

    void EquipCostume(CostumeSO costumeData)
    {
        if (costumeData == null) return;

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
        DataManager.Instance.SetEquippedItem(costumeData.costumeType, costumeData.costumeID);
        DataManager.Instance.Save();
    }

    public void ResetAllData()
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
