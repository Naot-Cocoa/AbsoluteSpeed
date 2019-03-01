using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Rename : MonoBehaviour
{
    private string mNewName = "Object";
    private bool mAddNumber = true;
    private bool mZeroFill = true;

#if UNITY_EDITOR
    [CustomEditor(typeof(Rename))]
    public class RenameEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Rename rename = target as Rename;

            rename.mNewName = EditorGUILayout.TextField("NewName", rename.mNewName);
            if (rename.mAddNumber = EditorGUILayout.Toggle("AddNumber", rename.mAddNumber)) rename.mZeroFill = EditorGUILayout.Toggle("ZeroFill?(from 01)", rename.mZeroFill);
            if (GUILayout.Button("Change Name")) rename.RenameStart();
        }
    }
#endif

    private void RenameStart()
    {
        if (mAddNumber)
        {
            string addZero = "0";

            if (mZeroFill)
            {
                int count = transform.childCount;

                for (int i = 0; i < count; i++)
                {
                    count /= 10;
                    if (count < 1) break;
                    addZero += "0";
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).name = mNewName + i.ToString(addZero);
            }
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).name = mNewName;
        }
    }
}
