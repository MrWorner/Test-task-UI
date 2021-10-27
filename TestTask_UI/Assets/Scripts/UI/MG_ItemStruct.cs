using System;
using UnityEngine;


namespace TestsTask_UI
{
    [Serializable]
    public struct MG_ItemStruct
    {
        public GameObject DroppedObject;
        public int FromIndex;
        public MG_ListUI FromList;
        public bool IsAClone;
        public GameObject SourceObject;
        public int ToIndex;
        public MG_ListUI ToList;

    }
}
