using UnityEngine;

public class SafeAreaManager : MonoBehaviour
{
    private RectTransform _rectTransform; // 스크립트가 제어할 UI 패널의 RectTransform을 저장할 변수
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0); // 가장 마지막으로 확인했던 safeArea 값을 저장하는 변수

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // 저장해둔 값과 달라졌을 때만 ApplySafeArea()를 다시 호출
        if (Screen.safeArea != _lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    /// <summary>
    /// UI를 터치 영역 안으로 조절
    /// </summary>
    private void ApplySafeArea()
    {
        // SafeArea값 저장
        Rect safeArea = Screen.safeArea;

        if (_rectTransform == null) return;

        // SafeArea 시작점 (좌측 하단 픽셀)
        Vector2 anchorMin = safeArea.position;

        // SafeArea 끝점 (우측 상단 픽셀)
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // 픽셀 좌표를 화면 전체 크기로 나눠서 비율로 변환
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // 패널의 좌측, 우측 하단 앵커를 방금 계산한 비율 값으로 설정
        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;

        // 현재 픽셀 값을 lastSafeArea에 저장
        _lastSafeArea = safeArea;
    }
}