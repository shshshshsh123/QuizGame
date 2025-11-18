using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            _equippedItemDict[CostumeType.Head] = 1001;
            _equippedItemDict[CostumeType.Body] = 2001;
            _equippedItemDict[CostumeType.Legs] = 3001;
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private HashSet<int> _acquiredItemSet = new HashSet<int>();
    private Dictionary<CostumeType, int> _equippedItemDict = new Dictionary<CostumeType, int>();
    private string SavePath => Path.Combine(Application.persistentDataPath, "saveFile.json");

    // 재화및 최고점수
    private int _maxIQ; // 최고점수
    public int MaxIQ => _maxIQ;
    public void SetMaxIQ(int iq) { _maxIQ = iq; }
    private int _goldAmount;    // 황금똥
    public int GoldAmount => _goldAmount;
    public void SetGoldAmount(int amount) { _goldAmount = amount; }
    public bool CanBuy(int price) { return _goldAmount >= price; }

    // 이름
    public string playerName;

    private void OnApplicationQuit() => Save();

    public void AcquireItem(int itemId)
    {
        if (!_acquiredItemSet.Contains(itemId))
        {
            _acquiredItemSet.Add(itemId);
            Save();
        }
    }

    public bool HasItem(int itemId) => _acquiredItemSet.Contains(itemId);

    // 장착 정보 저장 (메모리 상)
    public void SetEquippedItem(CostumeType type, int itemId)
    {
        if (_equippedItemDict.ContainsKey(type))
        {
            _equippedItemDict[type] = itemId;
            Debug.Log($"{type} 부위에 아이템 {itemId} 장착 설정됨");
        }
    }

    public int GetEquippedItemId(CostumeType type)
    {
        if (_equippedItemDict.TryGetValue(type, out int id)) return id;
        return 0;
    }

    // 저장하기
    public void Save()
    {
        GameSaveData data = new GameSaveData();
        // HashSet을 List로 변환하여 저장
        data.acquiredItemIds = new List<int>(_acquiredItemSet);

        data.equippedHeadId = _equippedItemDict[CostumeType.Head];
        data.equippedBodyId = _equippedItemDict[CostumeType.Body];
        data.equippedLegsId = _equippedItemDict[CostumeType.Legs];

        data.goldAmount = _goldAmount;
        data.maxIQ = _maxIQ;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("저장 완료: " + SavePath);
    }

    // 불러오기
    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("세이브 파일이 없습니다. 새로운 데이터를 시작합니다."); 
            _acquiredItemSet.Clear();
            // 기본 아이템 획득 처리
            _acquiredItemSet.Add(1001);
            _acquiredItemSet.Add(2001);
            _acquiredItemSet.Add(3001);
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            _acquiredItemSet = new HashSet<int>(data.acquiredItemIds);
            _equippedItemDict[CostumeType.Head] = data.equippedHeadId;
            _equippedItemDict[CostumeType.Body] = data.equippedBodyId;
            _equippedItemDict[CostumeType.Legs] = data.equippedLegsId;

            _goldAmount = data.goldAmount;
            _maxIQ = data.maxIQ;

            Debug.Log($"로드 완료: {_acquiredItemSet.Count}개의 아이템 로드됨");
        }
        catch (Exception e)
        {
            Debug.LogError($"로드 실패: {e.Message}");
        }
    }
}

[Serializable]
public class GameSaveData
{
    // JSON 직렬화를 위해 List로 저장 (저장용)
    public List<int> acquiredItemIds = new List<int>(); 
    
    public int equippedHeadId = 0;
    public int equippedBodyId = 0;
    public int equippedLegsId = 0;

    public int goldAmount = 0;
    public int maxIQ = 0;
}