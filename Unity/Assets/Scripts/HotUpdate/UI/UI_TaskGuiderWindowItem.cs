using UnityEngine;
using UnityEngine.UI;

public class UI_TaskGuiderWindowItem : MonoBehaviour
{
    [SerializeField] private Text infoText;
    private Vector3 targetPosition;
    private RectTransform rectTransform => (RectTransform)transform;
    private int index;
    private Vector2 halfSize;
    private Vector2 posXRange;
    private Vector2 posYRange;
    private static Vector2 playerFootUIPosition = new Vector2(0, -ClientGlobal.canvasSize.y / 2f * 0.9f);
    public void Init(Vector3 target, int index)
    {
        Vector2 canvasSize = ClientGlobal.canvasSize;
        posXRange = new Vector2(canvasSize.x / -2 + halfSize.x, canvasSize.x / 2 - halfSize.x);
        posYRange = new Vector2(canvasSize.y / -2 + halfSize.y, canvasSize.y / 2 - halfSize.y);
        halfSize = rectTransform.sizeDelta / 2f;
        this.targetPosition = target;
        UpdateIndex(index);
        infoText.text = (index + 1).ToString();
    }

    public void UpdateIndex(int index)
    {
        this.index = index;
    }

    public void UpdatePosition(Vector3 playerPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPosition);
        Vector2 canvasSize = ClientGlobal.canvasSize;
        // 屏幕坐标系中心点在左下角，UI坐标系在中间，偏移一半
        screenPos.x -= canvasSize.x / 2f;
        screenPos.y -= canvasSize.y / 2f;

        // 锁定在屏幕范围内
        // 背面 直接锁定到屏幕最下方
        if (screenPos.z <= 0) screenPos.y = posYRange.x;
        else screenPos.y = Mathf.Clamp(screenPos.y, posYRange.x, posYRange.y);
        screenPos.z = 0;
        screenPos.x = Mathf.Clamp(screenPos.x, posXRange.x, posXRange.y);

        rectTransform.anchoredPosition = screenPos;
        rectTransform.up = rectTransform.anchoredPosition - playerFootUIPosition;

        float dis = Vector3.Distance(playerPosition, targetPosition);
        infoText.text = $"{index + 1}({dis.ToString("F2")}m)";
    }

}
