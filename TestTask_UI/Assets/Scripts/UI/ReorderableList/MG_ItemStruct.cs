using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace UnityEngine.UI.Extensions
{
    [Serializable]
    public struct MG_ItemStruct
    {
        public GameObject DroppedObject;
        public int FromIndex;
        public ReorderableList FromList;
        public bool IsAClone;
        public GameObject SourceObject;
        public int ToIndex;
        public ReorderableList ToList;

    }
}
