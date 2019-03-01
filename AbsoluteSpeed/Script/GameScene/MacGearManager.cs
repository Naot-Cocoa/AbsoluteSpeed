using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MacGearManager : GearManager
{
    public MacGearManager(VehicleCore core) : base(core) { }

    protected override GearState MT()
    {
        if (Input.GetKeyDown(KeyCode.RightCommand))
        {
            mCurrentGearIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftCommand))
        {
            mCurrentGearIndex--;
        }
        mCurrentGearIndex = Mathf.Clamp(mCurrentGearIndex, 0, (int)GearState.MAX - 1);

        return (GearState)Enum.ToObject(typeof(GearState), mCurrentGearIndex);
    }
}
