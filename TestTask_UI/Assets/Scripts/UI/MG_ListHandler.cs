//За основу класса лежит UNITY 3D UI EXTENSIONS: https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/release/

using System;
using UnityEngine.Events;

namespace TestsTask_UI
{
    [Serializable]
    public class MG_ListHandler : UnityEvent<MG_ItemStruct>
    {
    }
}
