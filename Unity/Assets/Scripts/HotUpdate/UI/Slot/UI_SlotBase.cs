using JKFrame;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_SlotBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected static UI_SlotBase enteredSlot;   // 目前鼠标进入的格子

    [SerializeField] protected Image frameIamge;
    [SerializeField] protected Sprite normalFrame;
    [SerializeField] protected Sprite selectedFrame;
    [SerializeField] protected Text keyCodeText; // 快捷窗口中时显示快捷键的
    public int dataIndex { get; private set; }
    protected Action<int> onUseAction;
    protected Action<PointerEventData.InputButton, int> onClickAction;
    protected Action<UI_SlotBase, UI_SlotBase> onDragToNewSlotAction;   // 从A拖拽到B
    public IItemWindow ownerWindow { get; private set; }
    public virtual void Init(IItemWindow ownerWindow, ItemDataBase data, ItemConfigBase config, int dataIndex,
        Action<int> onUseAction = null, Action<UI_SlotBase, UI_SlotBase> onDragToNewSlotAction = null, Action<PointerEventData.InputButton, int> onClickAction = null)
    {
        this.ownerWindow = ownerWindow;
        this.dataIndex = dataIndex;
        this.onUseAction = onUseAction;
        this.onClickAction = onClickAction;
        this.onDragToNewSlotAction = onDragToNewSlotAction;
        SetFrameColor(Color.white);
        if (keyCodeText != null) keyCodeText.gameObject.SetActive(false);
        OnPointerExit(null);
        OnInit();
    }

    public virtual void OnInit()
    {
    }
    public virtual void SetShortcutKeyCode(int num) // 键盘数字
    {
        keyCodeText.gameObject.SetActive(true);
        keyCodeText.text = num.ToString();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        frameIamge.sprite = selectedFrame;
        enteredSlot = this;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        frameIamge.sprite = normalFrame;
        enteredSlot = null;
    }

    public virtual void Destroy()
    {
        this.GameObjectPushPool();
        if (enteredSlot == this) enteredSlot = null;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onClickAction?.Invoke(eventData.button, dataIndex);

        // 鼠标右键意味着使用物品
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Use();
        }
    }
    public void Use()
    {
        onUseAction?.Invoke(dataIndex);
    }
    public void SetFrameColor(Color color)
    {
        frameIamge.color = color;
    }
    public virtual void SetCount(string countString, Color color)
    {
    }


}
public abstract class UI_SlotBase<D, C> : UI_SlotBase, IBeginDragHandler, IDragHandler, IEndDragHandler where D : ItemDataBase where C : ItemConfigBase
{
    [SerializeField] protected Image iconImage;
    protected D itemData;
    protected C itemConfig;

    public override void Init(IItemWindow ownerWindow, ItemDataBase data, ItemConfigBase config, int dataIndex, Action<int> onUseAction = null, Action<UI_SlotBase, UI_SlotBase> onDragToNewSlotAction = null, Action<PointerEventData.InputButton, int> onClickAction = null)
    {
        this.itemData = (D)data;
        this.itemConfig = (C)config;
        base.Init(ownerWindow, data, config, dataIndex, onUseAction, onDragToNewSlotAction, onClickAction);
    }

    public override void OnInit()
    {
        iconImage.sprite = itemConfig.icon;
    }
    public override void Destroy()
    {
        if (itemConfig != null && enteredSlot == this)
        {
            UISystem.Close<UI_ItemInfoPopupWindow>();
        }
        itemData = null;
        itemConfig = null;
        base.Destroy();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (itemConfig != null)
        {
            UISystem.Show<UI_ItemInfoPopupWindow>().Show(transform.position, ((RectTransform)transform).sizeDelta.y / 2, itemConfig);
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (itemConfig != null && enteredSlot == this)
        {
            UISystem.Close<UI_ItemInfoPopupWindow>();
        }
        base.OnPointerExit(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onDragToNewSlotAction == null) return;
        iconImage.transform.SetParent(UISystem.DragLayer);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onDragToNewSlotAction == null) return;
        iconImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onDragToNewSlotAction == null) return;
        iconImage.transform.SetParent(transform);
        iconImage.transform.SetAsFirstSibling();
        iconImage.transform.localPosition = Vector3.zero;
        // 对方格子是有意义的并且不是自己
        if (enteredSlot != null && enteredSlot != this)
        {
            onDragToNewSlotAction?.Invoke(this, enteredSlot);
        }
    }
}