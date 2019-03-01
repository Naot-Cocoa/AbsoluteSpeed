using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearManager
{

    protected int mCurrentGearIndex = 0;
   // [SerializeField]
    private Transmission mTransmission;// = Transmission.NONE;
    [SerializeField, Tooltip("シフトレバー使用フラグ")]
    private bool mShiftLever = false;
    private VehicleCore mVehicleCore;
    private GearParam mGearParam;


    public Transmission GetTransmission { get {return mTransmission; } }
    public bool IsMT { get { return (mTransmission == Transmission.MT); } }
    public GearManager(VehicleCore core)
    {
        Initialize();
        mVehicleCore = core;
        mGearParam = Resources.Load(ConstString.Path.GEAR_PARAM) as GearParam;
    }

    /// <summary>AT,MT変換に使用</summary>
    /// <param name="transmission"></param>
    public void ConvertTo(Transmission transmission) { mTransmission = transmission;}

    public GearState GetGear()
    {
        switch (GetTransmission)
        {
            case Transmission.AT: { return AT(); }
            case Transmission.MT: { return MT(); }
            default: { return GearState.NONE; }
        }
    }


    public virtual void OnEnable() { Initialize(); }

    private void Initialize()
    {
        mCurrentGearIndex = (int)GearState.FIRST;
        mShiftLever = false;
    }

    protected virtual GearState MT()
    {
        if (mShiftLever)
        {

            //reverseから
            if (Input.GetButton(ConstString.Input.REVERSE)) { mCurrentGearIndex = 0; }
            else if (Input.GetButton(ConstString.Input.FIRST)) { mCurrentGearIndex = 2; }
            else if (Input.GetButton(ConstString.Input.SECOND)) { mCurrentGearIndex = 3; }
            else if (Input.GetButton(ConstString.Input.THIRD)) { mCurrentGearIndex = 4; }
            else if (Input.GetButton(ConstString.Input.FOURTH)) { mCurrentGearIndex = 5; }
            else if (Input.GetButton(ConstString.Input.FIFTH)) { mCurrentGearIndex = 6; }
            else if (Input.GetButton(ConstString.Input.SIXTH)) { mCurrentGearIndex = 7; }
            //neutral
            else { mCurrentGearIndex = 1; }
        }
        else
        {
            if (Input.GetButtonDown(ConstString.Input.UPPER_GEAR)) { mCurrentGearIndex++; }
            else if (Input.GetButtonDown(ConstString.Input.DOWNER_GEAR)) { mCurrentGearIndex--; }
        }

        mCurrentGearIndex = Mathf.Clamp(mCurrentGearIndex, 0, (int)GearState.MAX - 1);

        return (GearState)Enum.ToObject(typeof(GearState), mCurrentGearIndex);
    }

    private GearState AT()
    {

        //reverseから
        float speed = mVehicleCore.EngineSpeed;
        if (speed >= mGearParam.GetGearData(GearState.SIXTH).NeedSpeed) { mCurrentGearIndex = 7; }
        else if (speed >= mGearParam.GetGearData(GearState.FIFTH).NeedSpeed) { mCurrentGearIndex = 6; }
        else if (speed >= mGearParam.GetGearData(GearState.FOURTH).NeedSpeed) { mCurrentGearIndex = 5; }
        else if (speed >= mGearParam.GetGearData(GearState.THIRD).NeedSpeed) { mCurrentGearIndex = 4; }
        else if (speed >= mGearParam.GetGearData(GearState.SECOND).NeedSpeed) { mCurrentGearIndex = 3; }
        else if (speed >= mGearParam.GetGearData(GearState.FIRST).NeedSpeed) { mCurrentGearIndex = 2; }
        else { mCurrentGearIndex = 0; }//minusにならない
        mCurrentGearIndex = Mathf.Clamp(mCurrentGearIndex, 0, (int)GearState.MAX - 1);
        return (GearState)Enum.ToObject(typeof(GearState), mCurrentGearIndex);
    }
}
