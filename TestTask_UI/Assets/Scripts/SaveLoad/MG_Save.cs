using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TestsTask_UI
{
    public class MG_Save : MonoBehaviour
    {

        private static MG_Save _instance;

        #region Поля
        [PropertyOrder(-1), BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] MG_Settings _settings;
        [BoxGroup("Настройки"), Required(InfoMessageType.Error), SerializeField] bool _openFolder = true;
        #endregion Поля

        #region Методы UNITY
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Debug.Log("<color=red>MG_Save Awake(): найден лишний MG_Save!</color>");

            if (_settings == null) Debug.Log("<color=red>MG_Save Awake(): '_settings' не задан!</color>");
        }
        #endregion Методы UNITY


        #region Публичные методы

        /// <summary>
        /// Сохранить
        /// </summary>
        [Button]
        public void Execute()
        {
            JSON_ItemContainer _container = new JSON_ItemContainer();
            var lists = MG_ListUI_Content.GetAllLists();

            if (lists.Any())
            {
                foreach (var list in lists)
                {
                    int id = list.Id;
                    List<MG_Item> listItems = list.GetItems();
                    if (listItems.Any())
                    {
                        _container.Process(listItems, id);
                    }
                }
            }

            string json = JsonUtility.ToJson(_container, false);
            string filePath = _settings.GetFilePath();
            StreamWriter sw = File.CreateText(filePath);
            sw.Close();
            File.WriteAllText(filePath, json);
            if (_openFolder) Process.Start(filePath);
        }

        #endregion Публичные методы


    }
}
