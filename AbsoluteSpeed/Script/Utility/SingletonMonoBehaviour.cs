using UnityEngine;
using System;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour{

    private static T mInstance;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                Type t = typeof(T);

                mInstance = (T)FindObjectOfType(t);
                if (mInstance == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }
            }

            return mInstance;
        }
    }

    virtual protected void Awake()
    {
        //既に存在する場合は破棄
        if (this != Instance)
        {
            Destroy(this);
            Debug.LogError(
                typeof(T) +
                " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
                " アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
            return;
        }
    }
}
