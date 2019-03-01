using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VibrationParam",fileName = "VibrationParam")]
public class VibrationParam : ScriptableObject
{
    [System.Serializable]
    public struct Param
    {
        [SerializeField]
        private string paramName;
        [SerializeField,Range(-10000,10000)]
        private int mPower;
        [SerializeField,Range(0f,1f)]
        private float mAddPowerTime;

        public string Name { get { return paramName; } }
        public int Power { get { return mPower; } }
        public float AddPowerTime { get { return mAddPowerTime; } }
    }

    public List<Param> mVibrationParamList;
}
