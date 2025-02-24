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
        reader.ReadValueSafe(out NetMessageType messageType);
        switch (messageType)
        {
            case NetMessageType.C_S_Register:
                reader.ReadValueSafe(out C_S_Register c_s_register);
                TriggerMessageCallback(NetMessageType.C_S_Register, clientId, c_s_register);
                break;
            case NetMessageType.S_C_Register:
                reader.ReadValueSafe(out S_C_Register s_c_register);
                TriggerMessageCallback(NetMessageType.S_C_Register, clientId, s_c_register);
                break;
            case NetMessageType.AccountInfo:
                reader.ReadValueSafe(out AccountInfo accountinfo);
                TriggerMessageCallback(NetMessageType.AccountInfo, clientId, accountinfo);
                break;
            case NetMessageType.C_S_Login:
                reader.ReadValueSafe(out C_S_Login c_s_login);
                TriggerMessageCallback(NetMessageType.C_S_Login, clientId, c_s_login);
                break;
            case NetMessageType.S_C_Login:
                reader.ReadValueSafe(out S_C_Login s_c_login);
                TriggerMessageCallback(NetMessageType.S_C_Login, clientId, s_c_login);
                break;
            case NetMessageType.C_S_EnterGame:
                reader.ReadValueSafe(out C_S_EnterGame c_s_entergame);
                TriggerMessageCallback(NetMessageType.C_S_EnterGame, clientId, c_s_entergame);
                break;
            case NetMessageType.C_S_Disonnect:
                reader.ReadValueSafe(out C_S_Disonnect c_s_disonnect);
                TriggerMessageCallback(NetMessageType.C_S_Disonnect, clientId, c_s_disonnect);
                break;
            case NetMessageType.S_C_Disonnect:
                reader.ReadValueSafe(out S_C_Disonnect s_c_disonnect);
                TriggerMessageCallback(NetMessageType.S_C_Disonnect, clientId, s_c_disonnect);
                break;
            case NetMessageType.C_S_ChatMessage:
                reader.ReadValueSafe(out C_S_ChatMessage c_s_chatmessage);
                TriggerMessageCallback(NetMessageType.C_S_ChatMessage, clientId, c_s_chatmessage);
                break;
            case NetMessageType.S_C_ChatMessage:
                reader.ReadValueSafe(out S_C_ChatMessage s_c_chatmessage);
                TriggerMessageCallback(NetMessageType.S_C_ChatMessage, clientId, s_c_chatmessage);
                break;
            case NetMessageType.C_S_GetBagData:
                reader.ReadValueSafe(out C_S_GetBagData c_s_getbagdata);
                TriggerMessageCallback(NetMessageType.C_S_GetBagData, clientId, c_s_getbagdata);
                break;
            case NetMessageType.S_C_GetBagData:
                reader.ReadValueSafe(out S_C_GetBagData s_c_getbagdata);
                TriggerMessageCallback(NetMessageType.S_C_GetBagData, clientId, s_c_getbagdata);
                break;
            case NetMessageType.C_S_BagUseItem:
                reader.ReadValueSafe(out C_S_BagUseItem c_s_baguseitem);
                TriggerMessageCallback(NetMessageType.C_S_BagUseItem, clientId, c_s_baguseitem);
                break;
            case NetMessageType.S_C_BagUpdateItem:
                reader.ReadValueSafe(out S_C_BagUpdateItem s_c_bagupdateitem);
                TriggerMessageCallback(NetMessageType.S_C_BagUpdateItem, clientId, s_c_bagupdateitem);
                break;
            case NetMessageType.C_S_BagSwapItem:
                reader.ReadValueSafe(out C_S_BagSwapItem c_s_bagswapitem);
                TriggerMessageCallback(NetMessageType.C_S_BagSwapItem, clientId, c_s_bagswapitem);
                break;
            case NetMessageType.S_C_ShortcutBarUpdateItem:
                reader.ReadValueSafe(out S_C_ShortcutBarUpdateItem s_c_shortcutbarupdateitem);
                TriggerMessageCallback(NetMessageType.S_C_ShortcutBarUpdateItem, clientId, s_c_shortcutbarupdateitem);
                break;
            case NetMessageType.C_S_ShortcutBarSetItem:
                reader.ReadValueSafe(out C_S_ShortcutBarSetItem c_s_shortcutbarsetitem);
                TriggerMessageCallback(NetMessageType.C_S_ShortcutBarSetItem, clientId, c_s_shortcutbarsetitem);
                break;
            case NetMessageType.C_S_ShortcutBarSwapItem:
                reader.ReadValueSafe(out C_S_ShortcutBarSwapItem c_s_shortcutbarswapitem);
                TriggerMessageCallback(NetMessageType.C_S_ShortcutBarSwapItem, clientId, c_s_shortcutbarswapitem);
                break;
            case NetMessageType.C_S_ShopBuyItem:
                reader.ReadValueSafe(out C_S_ShopBuyItem c_s_shopbuyitem);
                TriggerMessageCallback(NetMessageType.C_S_ShopBuyItem, clientId, c_s_shopbuyitem);
                break;
            case NetMessageType.S_C_UpdateCoinCount:
                reader.ReadValueSafe(out S_C_UpdateCoinCount s_c_updatecoincount);
                TriggerMessageCallback(NetMessageType.S_C_UpdateCoinCount, clientId, s_c_updatecoincount);
                break;
            case NetMessageType.C_S_BagSellItem:
                reader.ReadValueSafe(out C_S_BagSellItem c_s_bagsellitem);
                TriggerMessageCallback(NetMessageType.C_S_BagSellItem, clientId, c_s_bagsellitem);
                break;
            case NetMessageType.C_S_CraftItem:
                reader.ReadValueSafe(out C_S_CraftItem c_s_craftitem);
                TriggerMessageCallback(NetMessageType.C_S_CraftItem, clientId, c_s_craftitem);
                break;
            case NetMessageType.C_S_GetTaskDatas:
                reader.ReadValueSafe(out C_S_GetTaskDatas c_s_gettaskdatas);
                TriggerMessageCallback(NetMessageType.C_S_GetTaskDatas, clientId, c_s_gettaskdatas);
                break;
            case NetMessageType.S_C_GetTaskDatas:
                reader.ReadValueSafe(out S_C_GetTaskDatas s_c_gettaskdatas);
                TriggerMessageCallback(NetMessageType.S_C_GetTaskDatas, clientId, s_c_gettaskdatas);
                break;
            case NetMessageType.C_S_CompleteDialogTask:
                reader.ReadValueSafe(out C_S_CompleteDialogTask c_s_completedialogtask);
                TriggerMessageCallback(NetMessageType.C_S_CompleteDialogTask, clientId, c_s_completedialogtask);
                break;
            case NetMessageType.S_C_CompleteTask:
                reader.ReadValueSafe(out S_C_CompleteTask s_c_completetask);
                TriggerMessageCallback(NetMessageType.S_C_CompleteTask, clientId, s_c_completetask);
                break;
            case NetMessageType.S_C_AddTask:
                reader.ReadValueSafe(out S_C_AddTask s_c_addtask);
                TriggerMessageCallback(NetMessageType.S_C_AddTask, clientId, s_c_addtask);
                break;
            case NetMessageType.S_C_UpdateTask:
                reader.ReadValueSafe(out S_C_UpdateTask s_c_updatetask);
                TriggerMessageCallback(NetMessageType.S_C_UpdateTask, clientId, s_c_updatetask);
                break;
            case NetMessageType.C_S_AddTask:
                reader.ReadValueSafe(out C_S_AddTask c_s_addtask);
                TriggerMessageCallback(NetMessageType.C_S_AddTask, clientId, c_s_addtask);
                break;
            case NetMessageType.C_S_ChatToAI:
                reader.ReadValueSafe(out C_S_ChatToAI c_s_chattoai);
                TriggerMessageCallback(NetMessageType.C_S_ChatToAI, clientId, c_s_chattoai);
                break;
            case NetMessageType.S_C_AIAnswer:
                reader.ReadValueSafe(out S_C_AIAnswer s_c_aianswer);
                TriggerMessageCallback(NetMessageType.S_C_AIAnswer, clientId, s_c_aianswer);
                break;
        }
    }
}
