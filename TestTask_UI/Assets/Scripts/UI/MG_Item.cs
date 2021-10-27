//За основу класса лежит UNITY 3D UI EXTENSIONS: https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/release/

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestsTask_UI
{
    public class MG_Item : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region Поля
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private Text _text;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
        [BoxGroup("Debug"), SerializeField, ReadOnly] private MG_ListUI _currentReorderableListRaycasted;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private int _fromIndex;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _draggingObject;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private LayoutElement _draggingObjectLE;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private Vector2 _draggingObjectOriginalSize;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _fakeElement;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private LayoutElement _fakeElementLE;

        //[BoxGroup("Debug"), SerializeField, ReadOnly] private int _displacedFromIndex;
        //[BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _displacedObject;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private bool _isDragging;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _rect;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private MG_ListUI _reorderableList;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private CanvasGroup _canvasGroup;
        [BoxGroup("Debug"), SerializeField, ReadOnly] internal bool isValid;
        #endregion Поля

        #region Свойства
        public Text TextLabel { get => _text; }
        #endregion Свойства

        #region Методы UNITY

        void Awake()
        {
            if (_text == null && !IsFake()) Debug.Log("<color=red>MG_Item Awake(): '_text' не прикреплен!</color>");
        }

        #endregion Методы UNITY

        #region Публичные методы

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="reorderableList"></param>
        public void Init(MG_ListUI reorderableList)
        {
            _reorderableList = reorderableList;
            _rect = GetComponent<RectTransform>();
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        /// <summary>
        /// Является ли текущий объект фейком
        /// </summary>
        /// <returns>фейк</returns>
        public bool IsFake()
        {
            if (name.Equals("Fake"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Получить текст
        /// </summary>
        /// <returns>текст</returns>
        public string GetText()
        {
            return _text.text;
        }
        #endregion Публичные методы

        #region Методы интерфейса "IBeginDragHandler"

        /// <summary>
        /// При начале перетаскивания
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            isValid = true;
            if (_reorderableList == null)
                return;

            _draggingObject = _rect;
            _fromIndex = _rect.GetSiblingIndex();

            if (_reorderableList.OnElementRemoved != null)
            {
                _reorderableList.OnElementRemoved.Invoke(new MG_ItemStruct
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = false,
                    SourceObject = _draggingObject.gameObject,
                    FromList = _reorderableList,
                    FromIndex = _fromIndex,
                });
            }
            if (isValid == false)
            {
                _draggingObject = null;
                return;
            }

            _draggingObjectOriginalSize = gameObject.GetComponent<RectTransform>().rect.size;
            _draggingObjectLE = _draggingObject.GetComponent<LayoutElement>();
            _draggingObject.SetParent(_reorderableList.DraggableArea, true);
            _draggingObject.SetAsLastSibling();
            _reorderableList.Refresh();

            _fakeElement = new GameObject("Fake").AddComponent<RectTransform>();
            _fakeElementLE = _fakeElement.gameObject.AddComponent<LayoutElement>();

            RefreshSizes();

            if (_reorderableList.OnElementGrabbed != null)
            {
                _reorderableList.OnElementGrabbed.Invoke(new MG_ItemStruct
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = false,
                    SourceObject = _draggingObject.gameObject,
                    FromList = _reorderableList,
                    FromIndex = _fromIndex,
                });

                if (!isValid)
                {
                    CancelDrag();
                    return;
                }
            }

            _isDragging = true;
        }

        #endregion

        #region Методы интерфейса "IDragHandler"

        /// <summary>
        /// При перетаскивания
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {

            if (!_isDragging)
                return;
            if (!isValid)
            {
                CancelDrag();
                return;
            }

            var canvas = _draggingObject.GetComponentInParent<Canvas>();
            Vector3 worldPoint;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position,
                canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null, out worldPoint);
            _draggingObject.position = worldPoint;

            MG_ListUI _oldReorderableListRaycasted = _currentReorderableListRaycasted;

            EventSystem.current.RaycastAll(eventData, _raycastResults);
            for (int i = 0; i < _raycastResults.Count; i++)
            {
                _currentReorderableListRaycasted = _raycastResults[i].gameObject.GetComponent<MG_ListUI>();
                if (_currentReorderableListRaycasted != null)
                {
                    break;
                }
            }

            if (_currentReorderableListRaycasted == null)
            {
                RefreshSizes();
                _fakeElement.transform.SetParent(_reorderableList.DraggableArea, false);
            }
            else
            {
                if (_fakeElement.parent != _currentReorderableListRaycasted.Content)
                {
                    _fakeElement.SetParent(_currentReorderableListRaycasted.Content, false);
                }

                float minDistance = float.PositiveInfinity;
                int targetIndex = 0;
                float dist = 0;
                for (int j = 0; j < _currentReorderableListRaycasted.Content.childCount; j++)
                {
                    var c = _currentReorderableListRaycasted.Content.GetChild(j).GetComponent<RectTransform>();

                    if (_currentReorderableListRaycasted.ContentLayout is VerticalLayoutGroup)
                        dist = Mathf.Abs(c.position.y - worldPoint.y);
                    else if (_currentReorderableListRaycasted.ContentLayout is HorizontalLayoutGroup)
                        dist = Mathf.Abs(c.position.x - worldPoint.x);
                    else if (_currentReorderableListRaycasted.ContentLayout is GridLayoutGroup)
                        dist = (Mathf.Abs(c.position.x - worldPoint.x) + Mathf.Abs(c.position.y - worldPoint.y));

                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        targetIndex = j;
                    }
                }

                RefreshSizes();
                _fakeElement.SetSiblingIndex(targetIndex);
                _fakeElement.gameObject.SetActive(true);

            }
        }

        #endregion

        #region Методы интерфейса "IEndDragHandler"

        /// <summary>
        /// Конец перетаскивания
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            if (_draggingObject != null)
            {
                var args = new MG_ItemStruct
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = false,
                    SourceObject = _draggingObject.gameObject,
                    FromList = _reorderableList,
                    FromIndex = _fromIndex,
                    ToList = _currentReorderableListRaycasted,
                    ToIndex = _fakeElement.GetSiblingIndex()
                };

                if (_reorderableList && _reorderableList.OnElementDropped != null)
                {
                    _reorderableList.OnElementDropped.Invoke(args);
                }
                if (!isValid)
                {
                    CancelDrag();
                    return;
                }
                RefreshSizes();
                _draggingObject.SetParent(_currentReorderableListRaycasted.Content, false);
                _draggingObject.rotation = _currentReorderableListRaycasted.transform.rotation;
                _draggingObject.SetSiblingIndex(_fakeElement.GetSiblingIndex());

                _reorderableList.Refresh();
                _currentReorderableListRaycasted.Refresh();

                _reorderableList.OnElementAdded.Invoke(args);
            }

            if (_fakeElement != null)
            {
                Destroy(_fakeElement.gameObject);
                _fakeElement = null;
            }
            _canvasGroup.blocksRaycasts = true;
        }

        #endregion

        #region Личные методы

        /// <summary>
        /// Обновить размер
        /// </summary>
        private void RefreshSizes()
        {
            Vector2 size = _draggingObjectOriginalSize;

            if (_currentReorderableListRaycasted != null
                && _currentReorderableListRaycasted.Content.childCount > 0
                )
            {
                var firstChild = _currentReorderableListRaycasted.Content.GetChild(0);
                if (firstChild != null)
                {
                    size = firstChild.GetComponent<RectTransform>().rect.size;
                }
            }

            _draggingObject.sizeDelta = size;
            _fakeElementLE.preferredHeight = _draggingObjectLE.preferredHeight = size.y;
            _fakeElementLE.preferredWidth = _draggingObjectLE.preferredWidth = size.x;
            _fakeElement.GetComponent<RectTransform>().sizeDelta = size;

        }

        /// <summary>
        /// Отменить перетаскивание
        /// </summary>
        private void CancelDrag()
        {
            _isDragging = false;

            RefreshSizes();
            _draggingObject.SetParent(_reorderableList.Content, false);
            _draggingObject.rotation = _reorderableList.Content.transform.rotation;
            _draggingObject.SetSiblingIndex(_fromIndex);


            var args = new MG_ItemStruct
            {
                DroppedObject = _draggingObject.gameObject,
                IsAClone = false,
                SourceObject = _draggingObject.gameObject,
                FromList = _reorderableList,
                FromIndex = _fromIndex,
                ToList = _reorderableList,
                ToIndex = _fromIndex
            };

            _reorderableList.Refresh();

            _reorderableList.OnElementAdded.Invoke(args);

            if (_fakeElement != null)
            {
                Destroy(_fakeElement.gameObject);
                _fakeElement = null;
            }
            _canvasGroup.blocksRaycasts = true;
        }
        #endregion Личные Личные
    }
}
