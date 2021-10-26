/// Credit Ziboo, Andrew Quesenberry 
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/
/// Last Child Fix - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/70/all-re-orderable-lists-cause-a-transform

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
    public class ReorderableListElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {

        [BoxGroup("Debug"), SerializeField, ReadOnly] private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
        [BoxGroup("Debug"), SerializeField, ReadOnly] private ReorderableList _currentReorderableListRaycasted;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private int _fromIndex;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _draggingObject;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private LayoutElement _draggingObjectLE;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private Vector2 _draggingObjectOriginalSize;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _fakeElement;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private LayoutElement _fakeElementLE;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private int _displacedFromIndex;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _displacedObject;

        [BoxGroup("Debug"), SerializeField, ReadOnly] private bool _isDragging;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _rect;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private ReorderableList _reorderableList;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private CanvasGroup _canvasGroup;
        [BoxGroup("Debug"), SerializeField, ReadOnly] internal bool isValid;
        //public bool isFake = false;


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



        #region IBeginDragHandler Members

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            isValid = true;
            if (_reorderableList == null)
                return;

            _draggingObject = _rect;
            _fromIndex = _rect.GetSiblingIndex();
            _displacedFromIndex = -1;
            //Send OnElementRemoved Event
            if (_reorderableList.OnElementRemoved != null)
            {
                _reorderableList.OnElementRemoved.Invoke(new MG_ListItem
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

            //Put _dragging object into the dragging area
            _draggingObjectOriginalSize = gameObject.GetComponent<RectTransform>().rect.size;
            _draggingObjectLE = _draggingObject.GetComponent<LayoutElement>();
            _draggingObject.SetParent(_reorderableList.DraggableArea, true);
            _draggingObject.SetAsLastSibling();
            _reorderableList.Refresh();

            //Create a fake element for previewing placement
            _fakeElement = new GameObject("Fake").AddComponent<RectTransform>();
            _fakeElementLE = _fakeElement.gameObject.AddComponent<LayoutElement>();


            RefreshSizes();

            //Send OnElementGrabbed Event
            if (_reorderableList.OnElementGrabbed != null)
            {
                _reorderableList.OnElementGrabbed.Invoke(new MG_ListItem
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = false,
                    SourceObject = _draggingObject.gameObject,
                    FromList = _reorderableList,
                    FromIndex = _fromIndex,
                });

                if (!isValid)
                {
                    //Debug.Log("<color=red>DISABLED!</color>");
                    CancelDrag();
                    return;
                }
            }

            _isDragging = true;
        }

        #endregion

        #region IDragHandler Members

        public void OnDrag(PointerEventData eventData)
        {

            if (!_isDragging)
                return;
            if (!isValid)
            {
                //Debug.Log("<color=red>DISABLED!</color>");
                CancelDrag();
                return;
            }
            //Set dragging object on cursor
            var canvas = _draggingObject.GetComponentInParent<Canvas>();
            Vector3 worldPoint;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position,
                canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null, out worldPoint);
            _draggingObject.position = worldPoint;

            ReorderableList _oldReorderableListRaycasted = _currentReorderableListRaycasted;

            //Check everything under the cursor to find a ReorderableList
            EventSystem.current.RaycastAll(eventData, _raycastResults);
            for (int i = 0; i < _raycastResults.Count; i++)
            {
                _currentReorderableListRaycasted = _raycastResults[i].gameObject.GetComponent<ReorderableList>();
                if (_currentReorderableListRaycasted != null)
                {
                    break;
                }
            }

            //If nothing found or the list is not dropable, put the fake element outside
            if (
                _currentReorderableListRaycasted == null
                )
            {
                RefreshSizes();
                _fakeElement.transform.SetParent(_reorderableList.DraggableArea, false);
                // revert the displaced element when not hovering over its list
                if (_displacedObject != null)
                {
                    Debug.Log("<color=red>DISABLED!</color>");
                    //revertDisplacedElement();
                }
            }
            //Else find the best position on the list and put fake element on the right index 
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
                if ((_currentReorderableListRaycasted != _oldReorderableListRaycasted || targetIndex != _displacedFromIndex)
                    )
                {
                    Transform toDisplace = _currentReorderableListRaycasted.Content.GetChild(targetIndex);
                    if (_displacedObject != null)
                    {
                        Debug.Log("<color=red>DISABLED!</color>");
                        //revertDisplacedElement();

                    }
                    else if (_fakeElement.parent != _currentReorderableListRaycasted.Content)
                    {
                        _fakeElement.SetParent(_currentReorderableListRaycasted.Content, false);
                        Debug.Log("<color=red>DISABLED!</color>");
                        //displaceElement(targetIndex, toDisplace);
                    }
                }
                RefreshSizes();
                _fakeElement.SetSiblingIndex(targetIndex);
                _fakeElement.gameObject.SetActive(true);

            }
        }

        #endregion

        #region IEndDragHandler Members

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            if (_draggingObject != null)
            {
                //If we have a ReorderableList that is dropable
                //Put the dragged object into the content and at the right index
                var args = new MG_ListItem
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = false,
                    SourceObject = _draggingObject.gameObject,
                    FromList = _reorderableList,
                    FromIndex = _fromIndex,
                    ToList = _currentReorderableListRaycasted,
                    ToIndex = _fakeElement.GetSiblingIndex()
                };
                //Send OnelementDropped Event
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
                // Force refreshing both lists because otherwise we get inappropriate FromList in ReorderableListEventStruct 
                _reorderableList.Refresh();
                _currentReorderableListRaycasted.Refresh();

                _reorderableList.OnElementAdded.Invoke(args);
            }

            //Delete fake element
            if (_fakeElement != null)
            {
                Destroy(_fakeElement.gameObject);
                _fakeElement = null;
            }
            _canvasGroup.blocksRaycasts = true;
        }

        #endregion

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

        void CancelDrag()
        {
            _isDragging = false;

            RefreshSizes();
            _draggingObject.SetParent(_reorderableList.Content, false);
            _draggingObject.rotation = _reorderableList.Content.transform.rotation;
            _draggingObject.SetSiblingIndex(_fromIndex);


            var args = new MG_ListItem
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

            //Delete fake element
            if (_fakeElement != null)
            {
                Destroy(_fakeElement.gameObject);
                _fakeElement = null;
            }
            _canvasGroup.blocksRaycasts = true;
        }

        public void Init(ReorderableList reorderableList)
        {
            _reorderableList = reorderableList;
            _rect = GetComponent<RectTransform>();
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }
    }
}
