﻿public class ConsumableData : StackableItemDataBase
{
    public override ItemDataBase Copy()
    {
        return new ConsumableData { id = id, count = count };
    }

    public override ItemType GetItemType()
    {
        return ItemType.Consumable;
    }

}
