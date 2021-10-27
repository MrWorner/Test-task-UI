using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestsTask_UI
{
    public class MG_ItemBuilder : MonoBehaviour
    {
        private static MG_ItemBuilder _instance;

        #region Поля
        [PropertyOrder(-1), BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] GameObject _prefab;
        #endregion Поля

        #region Свойства
        #endregion Свойства

        #region Методы UNITY
        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Debug.Log("<color=red>MG_ItemBuilder Awake(): найден лишний MG_ItemBuilder!</color>");

            if (_prefab == null) Debug.Log("<color=red>MG_ItemBuilder Awake(): '_prefab' не прикреплен!</color>");
        }
        #endregion Методы UNITY

        #region Публичные методы
        public GameObject Build()
        {
            GameObject newItem = Instantiate(_prefab, new Vector3(0, 0, 0), Quaternion.identity);
            return newItem;
        }
        #endregion Публичные методы

        #region Личные методы

        #endregion Личные Личные
    }
}
