using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostumeItem : MonoBehaviour
{
    [Header("# Costume Item Info")]
    [SerializeField, Tooltip("버튼(본인 오브젝트)")] Button _costumeButton;
    [SerializeField, Tooltip("이미지")] Image _costumeIconImage;
    [SerializeField, Tooltip("가격 텍스트")] TMP_Text _priceText;    // 보유중일 경우에는 "보유중" 으로 표시
    [SerializeField, Tooltip("잠금 아이콘")] GameObject _lockIconObject;

    private CostumeSO _costumeData;
    private Action<CostumeSO> _onClickCallback;

    /// <summary>
    /// 외부에서 코스튬 정보를 설정하는 함수
    /// </summary>
    /// <param name="costumeData">아이템이 표시할 SO 데이터</param>
    /// <param name="onClickCallback">클릭시 실행될 함수</param>
    public void SetCostumeInfo(CostumeSO costumeData, Action<CostumeSO> onClickCallback)
    {
        _costumeData = costumeData;
        _onClickCallback = onClickCallback;

        // 아이콘 설정
        _costumeIconImage.sprite = costumeData.costumeSprite;

        // 잠금 상태에 따른 UI 업데이트
        if (costumeData.isUnlocked)
        {
            _priceText.text = "보유중";
            _priceText.color = Color.black;
            _lockIconObject.SetActive(false);
            _costumeIconImage.gameObject.SetActive(true);
        }
        else
        {
            _priceText.text = "1000 G"; // 예시 가격
            _priceText.color = Color.yellow;
            _lockIconObject.SetActive(true);
            _costumeIconImage.gameObject.SetActive(false);
        }

        // 버튼 리스너 삭제및 추가
        _costumeButton.onClick.RemoveAllListeners();
        _costumeButton.onClick.AddListener(OnItemClicked);
    }

    /// <summary>
    /// 아이템이 클릭되었을 때 호출되는 함수
    /// </summary>
    void OnItemClicked()
    {
        _onClickCallback?.Invoke(_costumeData);
    }
}