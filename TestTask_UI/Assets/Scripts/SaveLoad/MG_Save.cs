using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// Сохранить карту
    /// </summary>
    [Button]
    public void Execute()
    {
        string dataPath = _settings.GetFilePath();

        //Vector2Int mapSize = _map.Size;
        //IReadOnlyList<MG_HexCell> cells = _map.GetCells();
        //JSON_MapContainer _container = new JSON_MapContainer(cells, mapSize);//Обрабатываем информацию и получаем контейнер      

        //string json = JsonUtility.ToJson(_container, false);
        //StreamWriter sw = File.CreateText(dataPath);
        //sw.Close();
        //File.WriteAllText(dataPath, json);

        //if (_openFolder) Process.Start(@Application.dataPath);
        ////countSavedTiles = _container.GetSize();//получить кол-во сохраненных cells
        ////Debug.Log("MG_SaveMap Save(): SAVED JSON tiles! Total tile count = " + countSavedTiles);
    }

}
