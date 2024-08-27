using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static event Action OnItemBought;
    public static event Action OnItemSold;

    public static void ItemBought()
    {
        OnItemBought?.Invoke();
    }

    public static void ItemSold()
    {
        OnItemSold?.Invoke();
    }
}