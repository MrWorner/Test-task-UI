//За основу класса лежит UNITY 3D UI EXTENSIONS: https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/release/

using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TestsTask_UI
{

    public class MG_ListUI : MonoBehaviour
    {
        #region Поля
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private LayoutGroup _contentLayout;
        [BoxGroup("ТРЕБОВАНИЯ"), Required(InfoMessageType.Error), SerializeField] private RectTransform _draggableArea;
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

        public LayoutGroup ContentLayout { get => _contentLayout; set => _contentLayout = value; }
        public RectTransform DraggableArea { get => _draggableArea; set => _draggableArea = value; }
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
            DraggableArea = transform.root.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
        }

        private void Start()
        {
            _listContent.Init(this);;
            Refresh();
        }
        #endregion Методы UNITY

        #region Публичные методы
        
        /// <summary>
        /// Обновить
        /// </summary>
        public void Refresh()
        {
            //_listContent = ContentLayout.gameObject.GetComponent<MG_ListUI_Content>();
            _listContent.Refresh();
            _count = _listContent.CountItems();
            SetCountText(_count);
        }

        /// <summary>
        /// Сбросить текст КОЛ-ВО ЭЛЕМЕНТОВ
        /// </summary>
        public void ResetText()
        {
            SetCountText(0);
        }

        /// <summary>
        /// Установить КОЛ-ВО ЭЛЕМЕНТОВ
        /// </summary>
        /// <param name="count"></param>
        public void SetCountText(int count)
        {
            _countText.text = "КОЛ-ВО ЭЛЕМЕНТОВ: " + count;
        }
        #endregion Публичные методы
    }
}
