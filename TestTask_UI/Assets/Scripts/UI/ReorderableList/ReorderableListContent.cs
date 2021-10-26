/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
    public class ReorderableListContent : MonoBehaviour
    {
        [BoxGroup("Debug"), SerializeField, ReadOnly] private List<Transform> _cachedChildren;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private List<MG_Item> _cachedListElement;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private MG_Item _listElement;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private ReorderableList _extList;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _rect;

        private void OnEnable()
        {
            if (_rect) StartCoroutine(RefreshChildren());
        }


        public void OnTransformChildrenChanged()
        {
            if (this.isActiveAndEnabled) StartCoroutine(RefreshChildren());
        }

        public void Init(ReorderableList extList)
        {
            _extList = extList;
            _rect = GetComponent<RectTransform>();
            _cachedChildren = new List<Transform>();
            _cachedListElement = new List<MG_Item>();

            StartCoroutine(RefreshChildren());
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

        private IEnumerator RefreshChildren()
        {
            //Handle new children
            for (int i = 0; i < _rect.childCount; i++)
            {
                if (_cachedChildren.Contains(_rect.GetChild(i)))
                    continue;

                //Get or Create MG_Item
                _listElement = _rect.GetChild(i).gameObject.GetComponent<MG_Item>() ??
                       _rect.GetChild(i).gameObject.AddComponent<MG_Item>();
                _listElement.Init(_extList);

                _cachedChildren.Add(_rect.GetChild(i));
                _cachedListElement.Add(_listElement);
            }

            //HACK a little hack, if I don't wait one frame I don't have the right deleted children
            yield return 0;

            //Remove deleted child
            for (int i = _cachedChildren.Count - 1; i >= 0; i--)
            {
                if (_cachedChildren[i] == null)
                {
                    _cachedChildren.RemoveAt(i);
                    _cachedListElement.RemoveAt(i);
                }
            }
        }
    }
}