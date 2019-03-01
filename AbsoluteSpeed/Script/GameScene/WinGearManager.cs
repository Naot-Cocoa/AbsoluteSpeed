using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WinGearManager : GearManager
{
    public WinGearManager(VehicleCore core) : base(core) { }

    protected override GearState MT()
    {
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            mCurrentGearIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            mCurrentGearIndex--;
        }
        mCurrentGearIndex = Mathf.Clamp(mCurrentGearIndex, 0, (int)GearState.MAX - 1);

        return (GearState)Enum.ToObject(typeof(GearState), mCurrentGearIndex);
    }
}