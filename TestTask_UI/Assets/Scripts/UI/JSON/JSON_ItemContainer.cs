using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestsTask_UI
{
    [Serializable]
    public class JSON_ItemContainer
    {
        private static JSON_ItemContainer _instance;

        #region Поля
        public List<JSON_Item> _items = new List<JSON_Item>();
        #endregion Поля

        #region Публичные методы Static

        /// <summary>
        /// Получить элементы листов
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<JSON_Item> GetJSON_Items()
        {
            return _instance._items;
        }

        #endregion Публичные методы Static

        #region Публичные методы

        /// <summary>
        /// Конструктор
        /// </summary>
        public JSON_ItemContainer()
        {
            _instance = this;
        }

        /// <summary>
        /// Обработать элементы листа
        /// </summary>
        /// <param name="items"></param>
        /// <param name="listID"></param>
        public void Process(IReadOnlyList<MG_Item> items, int listID)
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

        /// <summary>
        /// Сгенерировать JSON ITEM
        /// </summary>
        /// <param name="item"></param>
        /// <param name="listID"></param>
        private void GenerateJSON_Item(MG_Item item, int listID)
        {
            JSON_Item json_item = new JSON_Item();
            json_item.listID = listID;
            json_item.text = item.GetText();

            _items.Add(json_item);
        }

        #endregion Личные Личные
    }
}
