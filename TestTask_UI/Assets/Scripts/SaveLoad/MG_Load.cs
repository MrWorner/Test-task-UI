using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TestsTask_UI
{
    public class MG_Load : MonoBehaviour
    {
        private static MG_Load _instance;

        #region Поля
        [PropertyOrder(-1), BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] MG_Settings _settings;
        [PropertyOrder(-1), BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] MG_ItemBuilder _itemBuilder;
        #endregion Поля

        #region Методы UNITY
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Debug.Log("<color=red>MG_Load Awake(): найден лишний MG_Load!</color>");

            if (_settings == null) Debug.Log("<color=red>MG_Load Awake(): '_settings' не задан!</color>");
            if (_itemBuilder == null) Debug.Log("<color=red>MG_Load Awake(): '_itemBuilder' не задан!</color>");
        }
        #endregion Методы UNITY

        /// <summary>
        /// Сохранить списки
        /// </summary>
        [Button]
        public void Execute()
        {
            var lists = MG_ListUI_Content.GetAllLists();
            if (lists.Any())
            {
                foreach (var list in lists)
                {
                    list.RemoveAllItems();
                }
            }

            string filePath = _settings.GetFilePath();
            string json = File.ReadAllText(filePath);
            JSON_ItemContainer сontainer = JsonUtility.FromJson<JSON_ItemContainer>(json);
            List<JSON_Item> jsonList = сontainer._items;

            if (jsonList.Any())
            {
                if (lists.Any())
                {
                    foreach (MG_ListUI_Content list in lists)
                    {
                        int count = 0;
                        foreach (JSON_Item jsonItem in jsonList)
                        {
                            int id = jsonItem.listID;

                            if (list.Id == id)
                            {
                                count++;
                                GameObject newItemObj = _itemBuilder.Build();
                                MG_Item item = newItemObj.GetComponent<MG_Item>();
                                item.TextLabel.text = jsonItem.text;
                                newItemObj.transform.SetParent(list.gameObject.transform);
                            }
                        }

                        list.Main.SetCountText(count);
                    }
                }

            }
        }
    }
}
