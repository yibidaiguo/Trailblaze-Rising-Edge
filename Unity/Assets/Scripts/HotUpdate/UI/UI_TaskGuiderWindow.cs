using JKFrame;
using System.Collections.Generic;
using UnityEngine;

public class UI_TaskGuiderWindow : UI_WindowBase
{
    [SerializeField] private GameObject itemTemplate;
    private List<UI_TaskGuiderWindowItem> itemList = new List<UI_TaskGuiderWindowItem>();
    public override void Init()
    {
        itemTemplate.gameObject.SetActive(false);
    }


    private Vector3 lastCameraPos;
    private void Update()
    {
        if (PlayerManager.Instance.localPlayer == null) return;
        if (Camera.main.transform.position == lastCameraPos) return;
        lastCameraPos = Camera.main.transform.position;
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].UpdatePosition(PlayerManager.Instance.localPlayer.transform.position);
        }
    }

    public void AddItem(Vector3 targetPos)
    {
        UI_TaskGuiderWindowItem item = CreateItem();
        item.Init(targetPos, itemList.Count);
        itemList.Add(item);
    }

    public void RemoveItem(int index)
    {
        UI_TaskGuiderWindowItem item = itemList[index];
        itemList.RemoveAt(index);
        GameObject.Destroy(item.gameObject);
        UpdateAllItemIndex(index);
    }

    private void UpdateAllItemIndex(int startIndex)
    {
        for (int i = startIndex; i < itemList.Count; i++)
        {
            itemList[i].UpdateIndex(i);
        }
    }

    public void UpdateItem(Vector3 targetPos, int index)
    {
        itemList[index].Init(targetPos, index);
    }

    private UI_TaskGuiderWindowItem CreateItem()
    {
        UI_TaskGuiderWindowItem item = GameObject.Instantiate(itemTemplate, transform).GetComponent<UI_TaskGuiderWindowItem>();
        item.gameObject.SetActive(true);
        return item;
    }
}
