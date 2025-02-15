using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponSlot : UI_SlotBase<WeaponData, WeaponConfig>
{
    [SerializeField] private GameObject usedImage;
    private bool usedState = false;
    public override void OnInit()
    {
        base.OnInit();
        SetUseState(false);
    }

    public void SetUseState(bool used)
    {
        this.usedState = used;
        usedImage.SetActive(used);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!usedState) // 当前武器已经处于使用状态，没必要发送网络请求切换武器
        {
            base.OnPointerClick(eventData);
        }
    }
}
