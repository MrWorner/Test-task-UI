/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{

    //[RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    //[AddComponentMenu("UI/Extensions/Re-orderable list")]
    public class ReorderableList : MonoBehaviour
    {
        public LayoutGroup ContentLayout;//Child container with re-orderable items in a layout group
        public RectTransform DraggableArea;//Parent area to draw the dragged element on top of containers. Defaults to the root Canvas
        // This sets every item size (when being dragged over this list) to the current size of the first element of this list
        [Header("UI Re-orderable Events")]
        public ReorderableListHandler OnElementDropped = new ReorderableListHandler();
        public ReorderableListHandler OnElementGrabbed = new ReorderableListHandler();
        public ReorderableListHandler OnElementRemoved = new ReorderableListHandler();
        public ReorderableListHandler OnElementAdded = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedFrom = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedTo = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedFromReturned = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedToReturned = new ReorderableListHandler();
        public ReorderableListHandler OnElementDroppedWithMaxItems = new ReorderableListHandler();

        private RectTransform _content;
        private ReorderableListContent _listContent;

        public RectTransform Content
        {
            get
            {
                if (_content == null)
                {
                    _content = ContentLayout.GetComponent<RectTransform>();
                }
                return _content;
            }
        }

        /// <summary>
        /// Refresh related list content
        /// </summary>
        public void Refresh()
        {
            Destroy(_listContent);
            _listContent = ContentLayout.gameObject.AddComponent<ReorderableListContent>();
            _listContent.Init(this);
        }

        private void Start()
        {
            if (ContentLayout == null)
            {
                Debug.LogError("You need to have a child LayoutGroup content set for the list: " + name, gameObject);
                return;
            }
            if (DraggableArea == null)
            {
                DraggableArea = transform.root.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
            }
            if (!GetComponent<Graphic>())
            {
                Debug.LogError("You need to have a Graphic control (such as an Image) for the list [" + name + "] to be droppable", gameObject);
                return;
            }

            Refresh();
        }


        #region Nested type: ReorderableListEventStruct

        [Serializable]
        public struct ReorderableListEventStruct
        {
            public GameObject DroppedObject;
            public int FromIndex;
            public ReorderableList FromList;
            public bool IsAClone;
            public GameObject SourceObject;
            public int ToIndex;
            public ReorderableList ToList;

            public void Cancel()
            {
                Debug.Log("<color=green>Cancel()!</color>");
                SourceObject.GetComponent<ReorderableListElement>().isValid = false;
            }
        }

        #endregion


        #region Nested type: ReorderableListHandler

        [Serializable]
        public class ReorderableListHandler : UnityEvent<ReorderableListEventStruct>
        {
        }

        #endregion
    }
}
