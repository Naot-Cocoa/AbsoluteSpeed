using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstString
{
    public static class Path
    {
        public const string GEAR_PARAM = "ScriptableObjects/GearParam";
        public const string RAY_CONFIG = "ScriptableObjects/RayConfig";
        public const string WHEEL_PARAM = "ScriptableObjects/WheelParam";
        public const string VEHICLE_SETTINGS = "ScriptableObjects/VehicleSettings";
        public const string PLAYER = "/Main/GameSceneManager/Stage(Clone)/Player";
        public const string PASE_NOTE_NORMAL = "/Main/GameSceneManager/Stage(Clone)/Player/GameSceneUI/PaseNoteInstantPoint/PaseNote_Normal";
        public const string PASE_NOTE_HARD = "/Main/GameSceneManager/Stage(Clone)/Player/GameSceneUI/PaseNoteInstantPoint/PaseNote_Hard";
    }

    public static class Input
    {
        public const string ACCEL = "Accel";
        public const string BRAKE = "Brake";
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
        public const string REVERSE = "Reverse";
        public const string FIRST = "First";
        public const string SECOND = "Second";
        public const string THIRD = "Third";
        public const string FOURTH = "Fourth";
        public const string FIFTH = "Fifth";
        public const string SIXTH = "Sixth";
        public const string UPPER_GEAR = "UpperGear";
        public const string DOWNER_GEAR = "DownerGear";
        public const string SUBMIT = "Submit";
        public const string C_ACCEL = "ControllerAccel";
        public const string C_HORIZONTAL = "ControllerHorizontal";
    }

    public static class Tag
    {
        public const string AI = "Ai";
    }

    public static class Name
    {
        public const string PLAYER = "Player";
        public const string TITLE_MESSAGE = "MessageImage";
        public const string PASE_NOTE = "PaseNote";
        public const string PASE_NOTE_GRASS = "PaseNoteGrass";
    }

    public static class Setting
    {
        public const string PASE_NOTE_INSTANCE_SETTING = "PaseNoteInstanceSetting";
    }

    public static class PaseNoteUI
    {
        public const string ARROW_RIGHT = "ArrowRight";
        public const string ARROW_RIGHT_TURN = "ArrowRightTurn";
        public const string ARROW_RIGHT_WAVE = "ArrowRightWave";
        public const string EXCLAMATION = "Exclamation";
    }
}
