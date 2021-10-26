/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{

    public class MG_ListUI : MonoBehaviour
    {
        #region Поля
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private LayoutGroup _contentLayout;//Child container with re-orderable items in a layout group
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private RectTransform _draggableArea;//Parent area to draw the dragged element on top of containers. Defaults to the root Canvas
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private Text _countText;
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private MG_ListUI_Content _listContent;
        [BoxGroup("Debug"), SerializeField, ReadOnly] private RectTransform _content;  
        [BoxGroup("Debug"), SerializeField, ReadOnly] private int _count;  
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
        public MG_ListHandler OnElementDropped { get; } = new MG_ListHandler();
        public MG_ListHandler OnElementGrabbed { get; } = new MG_ListHandler();
        public MG_ListHandler OnElementRemoved { get; } = new MG_ListHandler();
        public MG_ListHandler OnElementAdded { get; set; } = new MG_ListHandler();
        public MG_ListHandler OnElementDroppedWithMaxItems { get; } = new MG_ListHandler();
        #endregion Свойства (Handlers)

        #region Методы UNITY
        private void Awake()
        {
            if (_contentLayout == null) Debug.Log("<color=red>MG_MapGenerator Awake(): '_contentLayout' не прикреплен!</color>");
            if (_draggableArea == null) Debug.Log("<color=red>MG_MapGenerator Awake(): '_draggableArea' не прикреплен!</color>");
            if (_countText == null) Debug.Log("<color=red>MG_MapGenerator Awake(): '_countText' не прикреплен!</color>");
            if (_listContent == null) Debug.Log("<color=red>MG_MapGenerator Awake(): '_listContent' не прикреплен!</color>");

            //OnElementAdded.AddListener(CountUp);
            //OnElementRemoved.AddListener(CountDown);
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

            _listContent.Init(this);;
            Refresh();
        }
        #endregion Методы UNITY


        #region Публичные методы
        /// <summary>
        /// Refresh related list content
        /// </summary>
        public void Refresh()
        {
            //_listContent = ContentLayout.gameObject.GetComponent<MG_ListUI_Content>();
            _listContent.Refresh();
            _count = _listContent.CountItems();
            SetCountText(_count);
        }

        public void Reset()
        {
            SetCountText(0);
        }
        #endregion Публичные методы

        #region Личные методы
        private void SetCountText(int count)
        {
            _countText.text = "КОЛ-ВО ЭЛЕМЕНТОВ: " + count;
        }
        #endregion Личные Личные

    }
}
