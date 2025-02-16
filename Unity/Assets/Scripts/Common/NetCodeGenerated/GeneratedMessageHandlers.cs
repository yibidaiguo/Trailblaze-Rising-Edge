using Unity.Netcode;

public partial class NetMessageManager
{
    //这个函数会在Init的时候调用
    partial void OnInit()
    {
         messagingManager.OnUnnamedMessage += ReceiveMessage;
    }
    partial void ReceiveMessage(ulong clientId, FastBufferReader reader)
    {
        reader.ReadValueSafe(out MessageType messageType);
        switch (messageType)
        {
            case MessageType.C_S_Register:
                reader.ReadValueSafe(out C_S_Register c_s_register);
                TriggerMessageCallback(MessageType.C_S_Register, clientId, c_s_register);
                break;
            case MessageType.S_C_Register:
                reader.ReadValueSafe(out S_C_Register s_c_register);
                TriggerMessageCallback(MessageType.S_C_Register, clientId, s_c_register);
                break;
            case MessageType.AccountInfo:
                reader.ReadValueSafe(out AccountInfo accountinfo);
                TriggerMessageCallback(MessageType.AccountInfo, clientId, accountinfo);
                break;
            case MessageType.C_S_Login:
                reader.ReadValueSafe(out C_S_Login c_s_login);
                TriggerMessageCallback(MessageType.C_S_Login, clientId, c_s_login);
                break;
            case MessageType.S_C_Login:
                reader.ReadValueSafe(out S_C_Login s_c_login);
                TriggerMessageCallback(MessageType.S_C_Login, clientId, s_c_login);
                break;
            case MessageType.C_S_EnterGame:
                reader.ReadValueSafe(out C_S_EnterGame c_s_entergame);
                TriggerMessageCallback(MessageType.C_S_EnterGame, clientId, c_s_entergame);
                break;
            case MessageType.C_S_Disonnect:
                reader.ReadValueSafe(out C_S_Disonnect c_s_disonnect);
                TriggerMessageCallback(MessageType.C_S_Disonnect, clientId, c_s_disonnect);
                break;
            case MessageType.S_C_Disonnect:
                reader.ReadValueSafe(out S_C_Disonnect s_c_disonnect);
                TriggerMessageCallback(MessageType.S_C_Disonnect, clientId, s_c_disonnect);
                break;
            case MessageType.C_S_ChatMessage:
                reader.ReadValueSafe(out C_S_ChatMessage c_s_chatmessage);
                TriggerMessageCallback(MessageType.C_S_ChatMessage, clientId, c_s_chatmessage);
                break;
            case MessageType.S_C_ChatMessage:
                reader.ReadValueSafe(out S_C_ChatMessage s_c_chatmessage);
                TriggerMessageCallback(MessageType.S_C_ChatMessage, clientId, s_c_chatmessage);
                break;
            case MessageType.C_S_GetBagData:
                reader.ReadValueSafe(out C_S_GetBagData c_s_getbagdata);
                TriggerMessageCallback(MessageType.C_S_GetBagData, clientId, c_s_getbagdata);
                break;
            case MessageType.S_C_GetBagData:
                reader.ReadValueSafe(out S_C_GetBagData s_c_getbagdata);
                TriggerMessageCallback(MessageType.S_C_GetBagData, clientId, s_c_getbagdata);
                break;
            case MessageType.C_S_BagUseItem:
                reader.ReadValueSafe(out C_S_BagUseItem c_s_baguseitem);
                TriggerMessageCallback(MessageType.C_S_BagUseItem, clientId, c_s_baguseitem);
                break;
            case MessageType.S_C_BagUpdateItem:
                reader.ReadValueSafe(out S_C_BagUpdateItem s_c_bagupdateitem);
                TriggerMessageCallback(MessageType.S_C_BagUpdateItem, clientId, s_c_bagupdateitem);
                break;
            case MessageType.C_S_BagSwapItem:
                reader.ReadValueSafe(out C_S_BagSwapItem c_s_bagswapitem);
                TriggerMessageCallback(MessageType.C_S_BagSwapItem, clientId, c_s_bagswapitem);
                break;
            case MessageType.S_C_ShortcutBarUpdateItem:
                reader.ReadValueSafe(out S_C_ShortcutBarUpdateItem s_c_shortcutbarupdateitem);
                TriggerMessageCallback(MessageType.S_C_ShortcutBarUpdateItem, clientId, s_c_shortcutbarupdateitem);
                break;
            case MessageType.C_S_ShortcutBarSetItem:
                reader.ReadValueSafe(out C_S_ShortcutBarSetItem c_s_shortcutbarsetitem);
                TriggerMessageCallback(MessageType.C_S_ShortcutBarSetItem, clientId, c_s_shortcutbarsetitem);
                break;
            case MessageType.C_S_ShortcutBarSwapItem:
                reader.ReadValueSafe(out C_S_ShortcutBarSwapItem c_s_shortcutbarswapitem);
                TriggerMessageCallback(MessageType.C_S_ShortcutBarSwapItem, clientId, c_s_shortcutbarswapitem);
                break;
            case MessageType.C_S_ShopBuyItem:
                reader.ReadValueSafe(out C_S_ShopBuyItem c_s_shopbuyitem);
                TriggerMessageCallback(MessageType.C_S_ShopBuyItem, clientId, c_s_shopbuyitem);
                break;
            case MessageType.S_C_UpdateCoinCount:
                reader.ReadValueSafe(out S_C_UpdateCoinCount s_c_updatecoincount);
                TriggerMessageCallback(MessageType.S_C_UpdateCoinCount, clientId, s_c_updatecoincount);
                break;
            case MessageType.C_S_BagSellItem:
                reader.ReadValueSafe(out C_S_BagSellItem c_s_bagsellitem);
                TriggerMessageCallback(MessageType.C_S_BagSellItem, clientId, c_s_bagsellitem);
                break;
            case MessageType.C_S_CraftItem:
                reader.ReadValueSafe(out C_S_CraftItem c_s_craftitem);
                TriggerMessageCallback(MessageType.C_S_CraftItem, clientId, c_s_craftitem);
                break;
            case MessageType.C_S_GetTaskDatas:
                reader.ReadValueSafe(out C_S_GetTaskDatas c_s_gettaskdatas);
                TriggerMessageCallback(MessageType.C_S_GetTaskDatas, clientId, c_s_gettaskdatas);
                break;
            case MessageType.S_C_GetTaskDatas:
                reader.ReadValueSafe(out S_C_GetTaskDatas s_c_gettaskdatas);
                TriggerMessageCallback(MessageType.S_C_GetTaskDatas, clientId, s_c_gettaskdatas);
                break;
            case MessageType.C_S_CompleteDialogTask:
                reader.ReadValueSafe(out C_S_CompleteDialogTask c_s_completedialogtask);
                TriggerMessageCallback(MessageType.C_S_CompleteDialogTask, clientId, c_s_completedialogtask);
                break;
            case MessageType.S_C_CompleteTask:
                reader.ReadValueSafe(out S_C_CompleteTask s_c_completetask);
                TriggerMessageCallback(MessageType.S_C_CompleteTask, clientId, s_c_completetask);
                break;
            case MessageType.S_C_AddTask:
                reader.ReadValueSafe(out S_C_AddTask s_c_addtask);
                TriggerMessageCallback(MessageType.S_C_AddTask, clientId, s_c_addtask);
                break;
            case MessageType.S_C_UpdateTask:
                reader.ReadValueSafe(out S_C_UpdateTask s_c_updatetask);
                TriggerMessageCallback(MessageType.S_C_UpdateTask, clientId, s_c_updatetask);
                break;
            case MessageType.C_S_AddTask:
                reader.ReadValueSafe(out C_S_AddTask c_s_addtask);
                TriggerMessageCallback(MessageType.C_S_AddTask, clientId, c_s_addtask);
                break;
        }
    }
}
