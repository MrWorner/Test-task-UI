/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{

    public class ReorderableList : MonoBehaviour
    {
        #region Поля
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private LayoutGroup _contentLayout;//Child container with re-orderable items in a layout group
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private RectTransform _draggableArea;//Parent area to draw the dragged element on top of containers. Defaults to the root Canvas
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private Text _countText;
        [BoxGroup("Тест"), SerializeField, ReadOnly] private RectTransform _content;
        [BoxGroup("Тест"), SerializeField, ReadOnly] private ReorderableListContent _listContent;
        [BoxGroup("Тест"), SerializeField, ReadOnly] private int _count;
        #endregion Поля

        #region Свойства
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

        public LayoutGroup ContentLayout { get => _contentLayout; set => _contentLayout = value; }//Child container with re-orderable items in a layout group
        public RectTransform DraggableArea { get => _draggableArea; set => _draggableArea = value; }//Parent area to draw the dragged element on top of containers. Defaults to the root Canvas
        #endregion Свойства

        #region Свойства (Handlers)
        public ReorderableListHandler OnElementDropped { get; } = new ReorderableListHandler();
        public ReorderableListHandler OnElementGrabbed { get; } = new ReorderableListHandler();
        public ReorderableListHandler OnElementRemoved { get; } = new ReorderableListHandler();
        public ReorderableListHandler OnElementAdded { get; } = new ReorderableListHandler();
        public ReorderableListHandler OnElementDroppedWithMaxItems { get; } = new ReorderableListHandler();
        #endregion Свойства (Handlers)

        #region Методы UNITY
        private void Awake()
        {
            if (_contentLayout == null) Debug.Log("<color=red>MG_MapGenerator Awake(): '_contentLayout' не прикреплен!</color>");
            if (_draggableArea == null) Debug.Log("<color=red>MG_MapGenerator Awake(): '_draggableArea' не прикреплен!</color>");
            if (_countText == null) Debug.Log("<color=red>MG_MapGenerator Awake(): '_countText' не прикреплен!</color>");
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
        #endregion Методы UNITY


        #region Публичные методы
        /// <summary>
        /// Refresh related list content
        /// </summary>
        public void Refresh()
        {
            Destroy(_listContent);
            _listContent = ContentLayout.gameObject.AddComponent<ReorderableListContent>();
            _listContent.Init(this);
        }
        #endregion Публичные методы

        #region Личные методы

        #endregion Личные Личные














        #region Nested type: ReorderableListHandler

        [Serializable]
        public class ReorderableListHandler : UnityEvent<MG_ListItem>
        {
        }
        #endregion




    }
}
