using UnityEngine;
using System.Collections;

/// <summary>
/// 状态类；
/// 在此中定义相关变量，表示游戏进程的某些状态，如处于第几关，抓到几只狗，是否追到狗等；
/// 添加此类主要是方便统一控制
/// </summary>
public class State {

    /// <summary>
    /// 是否正在游戏；IsPlaying表示点击“开始游戏”后到“游戏结束”期间。
    /// 待定是否需要此变量
    /// </summary>
    public static bool IsPlaying = false;

    /// <summary>
    /// 是否处于抓到狗状态；若是，则进行人狗互动等
    /// </summary>
    public static bool IsCaughtUp = false;

    /// <summary>
    /// 抓到的狗的数量
    /// </summary>
    public static int NumDogsCaught = 0;

    public static int updateNumDogsCaught() {
        return ++NumDogsCaught;
    }

    public static int getNumDogsCaught()
    {
        return NumDogsCaught;
    }
    

    public static RunnerState RunnerState;
}



/// <summary>
/// 人的状态枚举，对应不同状态，人和狗有
/// </summary>
public enum RunnerState
{
    /// <summary>
    /// 正在追赶
    /// </summary>
    Chasing,
    /// <summary>
    /// 抓到狗
    /// </summary>
    CaughtUp,
    /// <summary>
    /// 抓到狗后再起速
    /// </summary>          
    SpeedingUp,
    /// <summary>
    /// 加速完后正常速度，还没有遇到狗；在遇到狗后，狗会开始跑，人会返回Chasing状态
    /// </summary>
    SearchingDog,
}

