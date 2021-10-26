using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

[Serializable]
public class JSON_ItemContainer
{


    #region Поля
    public List<JSON_Item> _items = new List<JSON_Item>();
    #endregion Поля

    #region Публичные методы
    public JSON_ItemContainer(IReadOnlyList<MG_Item> items, int listID)
    {
        if (items.Any())
        {
            foreach (var item in items)
            {
                GenerateJSON_Item(item, listID);
            }
        }
    }
    #endregion Публичные методы

    #region Личные методы
    private void GenerateJSON_Item(MG_Item item, int listID)
    {
        JSON_Item json_item = new JSON_Item();
        json_item.listID = listID;
        json_item.text = item.GetText();

        _items.Add(json_item);
    }
    #endregion Личные Личные

}
