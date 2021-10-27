//Данный класс взят из UNITY 3D UI EXTENSIONS: https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/release/

using UnityEngine;

namespace TestsTask_UI
{
    public static class ExtentionMethods
    {
        public static T GetOrAddComponent<T>(this GameObject child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.AddComponent<T>();
            }
            return result;
        }
    }
}
