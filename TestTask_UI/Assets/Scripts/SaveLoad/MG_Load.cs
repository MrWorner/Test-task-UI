using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class MG_Load : MonoBehaviour
{
    private static MG_Load _instance;

    #region Поля
    [PropertyOrder(-1), BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] MG_Settings _settings;
    #endregion Поля

    #region Методы UNITY
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Debug.Log("<color=red>MG_Load Awake(): найден лишний MG_Load!</color>");

        if (_settings == null) Debug.Log("<color=red>MG_Load Awake(): '_settings' не задан!</color>");
    }
    #endregion Методы UNITY

    /// <summary>
    /// Сохранить списки
    /// </summary>
    [Button]
    public void Execute()
    {
        var lists = ReorderableListContent.GetAllLists();
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
    }
}
