using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public static class FanStaticController
{   
    public static bool ChangePowerSwitchOn()
    {
        AppController.FindButton(63);
        return true;
    }

    public static bool ChangePowerSwitchOff()
    {
        AppController.FindButton(63);
        return false;
    }

    public static void ChangeAirFlowStatic(FanPower fanPower)
    {
        switch (fanPower)
        {
            case FanPower.START:
                AppController.FindButton(64);
                //AppController.FindButton(63);
                break;
            case FanPower.ONE:
                AppController.FindButton(63);
                break;
            case FanPower.TWO:
                AppController.FindButton(60);
                break;
            case FanPower.THREE:
                AppController.FindButton(59);
                break;
            case FanPower.END:
                //AppController.FindButton(63);
                AppController.FindButton(64);
                break;
        }
    }
}

public enum FanPower
{
    START,
    END,
    ONE,
    TWO,
    THREE
}
