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
}
