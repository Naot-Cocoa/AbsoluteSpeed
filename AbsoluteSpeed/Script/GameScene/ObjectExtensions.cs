using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtensions
{
    /// <summary>
    /// 指定されたインターフェイスを実装したコンポーネントを持つオブジェクトを検索します
    /// </summary>
    public static T FindObjectOfInterface<T>(this Object obj) where T : class
    {
        foreach (var n in Object.FindObjectsOfType<Component>())
        {
            var component = n as T;
            if (component != null)
            {
                return component;
            }
        }
        return null;
    }
}


