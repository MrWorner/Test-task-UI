//За основу класса лежит UNITY 3D UI EXTENSIONS: https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/release/

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestsTask_UI
{
    public class MG_ListUI_Content : MonoBehaviour
    {
        #region Поля Static
        private static HashSet<int> _all_IDs = new HashSet<int>();
        private static HashSet<MG_ListUI_Content> _all_lists = new HashSet<MG_ListUI_Content>();
        #endregion Поля Static

        #region Поля
        [BoxGroup("ID"), SerializeField] private int _id = -1;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private List<Transform> _cachedChildren;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private List<MG_Item> _cachedListElement;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private MG_Item _item;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private MG_ListUI _main;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _rect;
        #endregion Поля

        #region Свойства
        public int Id { get => _id; }
        public MG_ListUI Main { get => _main; }
        #endregion Свойства

        #region Методы UNITY
        void Awake()
        {
            CheckID(this);
        }
        private void OnEnable()
        {
            if (_rect) StartCoroutine(RefreshChildren());
        }

        public void OnTransformChildrenChanged()
        {
            if (this.isActiveAndEnabled) StartCoroutine(RefreshChildren());
        }
        #endregion Методы UNITY

        #region Публичные методы Stati

        /// <summary>
        /// Получить все листы
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<MG_ListUI_Content> GetAllLists()
        {
            return _all_lists;
        }

        #endregion Публичные методы Static

        #region Публичные методы

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="extList"></param>
        public void Init(MG_ListUI extList)
        {
            _main = extList;
            _rect = GetComponent<RectTransform>();
            Refresh();
        }

        /// <summary>
        /// Обновить
        /// </summary>
        public void Refresh()
        {
            _cachedChildren = new List<Transform>();
            _cachedListElement = new List<MG_Item>();
            StartCoroutine(RefreshChildren());
        }

        /// <summary>
        /// Получить список элементов  
        /// </summary>
        /// <returns></returns>
        public List<MG_Item> GetItems()
        {
            List<MG_Item> collection = new List<MG_Item>();

            if (_cachedListElement.Any())
            {
                foreach (MG_Item item in _cachedListElement)
                {
                    if (item == null)
                        continue;

                    if (item.IsFake())
                        continue;

                    collection.Add(item);
                }
            }
            return collection;
        }

        /// <summary>
        /// Получить кол-во элементов в листе
        /// </summary>
        /// <returns>кол-во элементов в листе</returns>
        [Button]
        public int CountItems()
        {
            if (_cachedListElement.Any())
            {
                int result = 0;
                foreach (MG_Item item in _cachedListElement)
                {
                    if (item == null)
                        continue;

                    if (item.IsFake())
                        continue;

                    result++;
                }
                return result;
            }
            else
            {
                return 0;
            }
        }

        [Button]
        public void RemoveAllItems()
        {
            if (_cachedListElement.Any())
            {
                foreach (var item in _cachedListElement)
                {
                    if (item != null)
                        Destroy(item.gameObject);
                }
            }
            _cachedChildren.Clear();
            _cachedListElement.Clear();
            _main.ResetText();

        }
        #endregion Публичные методы

        #region Личные методы

        /// <summary>
        /// Проверить ID
        /// </summary>
        /// <param name="list"></param>
        private static void CheckID(MG_ListUI_Content list)
        {
            int id = list._id;

            if (_all_IDs.Contains(id) == false)
            {
                _all_IDs.Add(id);
                _all_lists.Add(list);
            }
            else
            {
                Debug.Log("<color=red>MG_ListUI_Content CheckID(): найден дубликат ID!</color> id= " + id);
            }
        }

        /// <summary>
        /// Обновить child
        /// </summary>
        /// <returns></returns>
        private IEnumerator RefreshChildren()
        {
            for (int i = 0; i < _rect.childCount; i++)
            {
                if (_cachedChildren.Contains(_rect.GetChild(i)))
                    continue;

                _item = _rect.GetChild(i).gameObject.GetComponent<MG_Item>() ??
                       _rect.GetChild(i).gameObject.AddComponent<MG_Item>();
                _item.Init(_main);

                _cachedChildren.Add(_rect.GetChild(i));
                _cachedListElement.Add(_item);
            }

            yield return 0;//Фикс

            for (int i = _cachedChildren.Count - 1; i >= 0; i--)
            {
                if (_cachedChildren[i] == null)
                {
                    _cachedChildren.RemoveAt(i);
                    _cachedListElement.RemoveAt(i);
                }
            }
        }
        #endregion Личные Личные
    }
}