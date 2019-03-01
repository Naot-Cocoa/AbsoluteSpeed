/// <summary>
/// ギアのステート
/// </summary>
public enum GearState
{
    NONE = -1,

    REVERSE,
    NEUTRAL,
    FIRST,
    SECOND,
    THIRD,
    FOURTH,
    FIFTH,
    SIXTH,

    MAX
}

public enum WheelState
{
    NONE = -1,

    GRIP,
    SLICK,
    DRIFT,

    MAX

}

//使用するトランスミッションのState
public enum TransMissionState
{
    AT,
    MT,
}
//ゲームプレイ中の状態State
public enum InGameState
{
    READY,
    PLAY,
    PAUSE,
    RESULT
}

public enum SceneState
{
    TITLE,
    MENU,
    GAME,
    PLAY_END,
    REPLAY
}



public enum Transmission
{
    NONE = -1,

    AT,
    MT,

    MAX
}

/// <summary>
/// 表示するペースノートの色
/// </summary>
public enum NavigationColor
{
    NORMAL,
    HARD
}

/// <summary>
/// 表示するペースノート反転・非反転
/// </summary>
public enum NavigationRotation
{
    NORMAL,
    REVERSE
}