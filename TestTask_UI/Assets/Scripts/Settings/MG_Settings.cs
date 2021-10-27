/////////////////////////////////////////////////////////////////////////////////
//
//	Описание:					
//			    
/////////////////////////////////////////////////////////////////////////////////

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TestsTask_UI
{
    public class MG_Settings : MonoBehaviour
    {
        private static MG_Settings _instance;

        #region Поля
        [BoxGroup("Путь к файлу сохранения"), Required(InfoMessageType.Error), SerializeField] string _path;
        [BoxGroup("Путь к файлу сохранения"), Required(InfoMessageType.Error), SerializeField] string _fileName = "SavedSession.json";
        #endregion Поля

        #region Свойства
        //public string Path { get => _path; }
        //public string FileName { get => _fileName; }
        #endregion Свойства

        #region Методы UNITY
        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Debug.Log("<color=red>MG_Settings Awake(): найден лишний MG_Settings!</color>");

            if (_path.Length < 1) GeneratePath();
            if (_fileName.Length < 1) GenerateFileName();
        }
        #endregion Методы UNITY

        #region Публичные методы
        public string GetFilePath()
        {
            return Path.Combine(_path, _fileName);
        }
        #endregion Публичные методы

        #region Личные методы
        [Button]
        private void GeneratePath()
        {
            _path = Application.dataPath;
        }

        [Button]
        private void GenerateFileName()
        {
            _fileName = "SavedSession.json";
        }
        #endregion Личные Личные
    }
}
