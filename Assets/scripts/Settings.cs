using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 游戏相关设置类，如物体高度等；
/// 设置为静态类，供全局使用；
/// 由于无法将此类中的变量映射到Unity编辑中以便边调试边修改参数值，因此，此类中的变量最好是比较稳定、不需要经常变更的变量,如人、狗、障碍物的高度等参数设定；
/// 对于需要经常调整、尝试的变量，可以设置在各object的脚本中，并使用public修饰，以便可以映射到unity编辑器中，如人、狗的初始速度，加减速等参数。
/// </summary>
public static class Settings {

    /// <summary>
    /// 此函数会在程序第一次使用Settings类时调用
    /// </summary>
    static Settings()
    {
        InitObstacleCombinations();     
    }
    

    private static float OrthoSizeCamera = Camera.main.orthographicSize;

    /// <summary>
    /// 摄像机是否跟随人；若否，则跟随狗
    /// </summary>
    public static bool IsCameraFollowRunner = true;

    /// <summary>
    /// 人和狗在正常追跑时的速度
    /// </summary>
    public static float SpeedNormal = 8f;

    /// <summary>
    /// 加速时每次增加的速度量
    /// </summary>
    public static float SpeedAddition = 1f;

    
    public static float AccSpeedUp_Runner = 5f;

    public static float AccSpeedUp_Dog = 5f;

    public static float SpeedInit_Dog = 3f;

    public static float SpeedInit_Runner = 0f;



    public static float DistanceFromRightToMeetingDog = 3;

    public static string Text_Obstacle = "obstacle";

    #region 各对象高度、宽度等设置, 及相关函数

    /// <summary>
    /// 摄像头画面的高度，也即整个游戏画面的世界高度
    /// </summary>
    public static float HeightCamera
    {
        get
        {
            //return Camera.main.orthographicSize * 2;
            return OrthoSizeCamera * 2;
        }
    }

    /// <summary>
    /// 摄像头画面的高度的一半
    /// </summary>
    public static float HeightCameraHalf
    {
        get
        {
            return HeightCamera / 2;
        }
    }

    /// <summary>
    /// 摄像头画面的宽度，也即整个游戏画面的世界宽度
    /// </summary>
    public static float WidthCamera
    {
        get
        {
            return HeightCamera * Camera.main.aspect;
        }
    }

    /// <summary>
    /// 摄像头画面的宽度的一半
    /// </summary>
    public static float WidthCameraHalf
    {
        get
        {
            return WidthCamera / 2;
        }
    }

    /// <summary>
    /// 地面的Y值；
    /// 也是人和狗的底部Y值，结合人和狗的高度可以算出人和狗在地面上时的中心Y值
    /// </summary>
    public static float YFloor { get { return -HeightCamera / 2 + HeightCamera * 1 / 8; } }

    /// <summary>
    /// 计分栏底部的Y值
    /// </summary>
    public static float YScoringBar { get { return HeightCamera * 7 / 8 - HeightCamera / 2; } }

    /// <summary>
    /// 人的高度
    /// </summary>
    public static float HeightRunner { get { return HeightCamera * 1 / 6; } }

    /// <summary>
    /// 狗的高度
    /// </summary>
    public static float HeightDog { get { return HeightCamera * 1 / 9; } }

    /// <summary>
    /// 人每跳高度
    /// </summary>
    public static float HeightRunnerJump { get { return HeightCamera * 7 / 36; } }

    /// <summary>
    /// 地面障碍物B1的高度
    /// </summary>
    public static float HeightB1 { get { return HeightCamera * 1 / 6; } }
    public static float HeightB2 { get { return HeightCamera * 1 / 3; } }
    public static float HeightB3 { get { return HeightCamera * 1 / 2; } }
    public static float HeightB4 { get { return HeightCamera * 1 / 12; } }

    public static float HeightT1 { get { return HeightCamera * 1 / 6; } }
    public static float HeightT2 { get { return HeightCamera * 1 / 3; } }
    public static float HeightT3 { get { return HeightCamera * 1 / 2; } }
    public static float HeightT4 { get { return HeightCamera * 1 / 12; } }


    public static float WidthB1 { get { return HeightCamera * 1 / 18; } }
    public static float WidthB2 { get { return HeightCamera * 1 / 18; } }
    public static float WidthB3 { get { return HeightCamera * 1 / 18; } }
    public static float WidthB4 { get { return HeightCamera * 1 / 2; } }

    public static float WidthT1 { get { return HeightCamera * 1 / 18; } }
    public static float WidthT2 { get { return HeightCamera * 1 / 18; } }
    public static float WidthT3 { get { return HeightCamera * 1 / 18; } }
    public static float WidthT4 { get { return HeightCamera * 1 / 2; } }

    /// <summary>
    /// 人在地面时的Y值
    /// </summary>
    //public static float YRunnerOnFloor { get { return GetYOnFloor(HeightRunner); } }
    public static float YRunnerOnFloor { get { return YFloor + HeightRunner / 2; } }

    /// <summary>
    /// 狗在地面时的Y值
    /// </summary>
    //public static float YDogOnFloor { get { return GetYOnFloor(HeightDog); } }
    public static float YDogOnFloor { get { return YFloor + HeightDog / 2; } }

    /// <summary>
    /// 根据物体的高度计算其在地面上的物体的Y值
    /// </summary>
    /// <param name="heightObject">物体的高度</param>
    /// <returns></returns>
    public static float GetYOnFloor(float heightObject)
    {
        return YFloor + heightObject / 2;
    }

    /// <summary>
    /// 根据物体的高度计算其在计分栏（天花板）下的物体的Y值
    /// </summary>
    /// <param name="heightObject"></param>
    /// <returns></returns>
    public static float GetYUnderCeil(float heightObject)
    {
        return YScoringBar - heightObject / 2;
    }


    #endregion


    #region 初始化障碍物组合
    /// <summary>
    /// 游戏所可能出现的障碍物组合；
    /// 在InitObstacleCombinations()函数中设置各组合
    /// </summary>
    public static Dictionary<DifficultyLevel, List<ObstacleCombination>> ObstacleCombinations = new Dictionary<DifficultyLevel, List<ObstacleCombination>>();

    /// <summary>
    /// 初始化游戏中的障碍物组合；
    /// </summary>
    private static void InitObstacleCombinations()
    {
        #region 初级难度的障碍物组合
        List<ObstacleCombination> combs_Low = new List<ObstacleCombination>()
        {
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.B1 },
                new List<float>() { 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.B2 },
                new List<float>() { 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.B3 },
                new List<float>() { 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.B4 },
                new List<float>() { 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.T1 },
                new List<float>() { 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.T2 },
                new List<float>() { 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.T3 },
                new List<float>() { 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Low,
                new List<ObstacleType>() { ObstacleType.T4 },
                new List<float>() { 0 }
            ),
        };
        #endregion


        #region 中级难度的障碍物组合
        List<ObstacleCombination> combs_Medium = new List<ObstacleCombination>()
        {
            new ObstacleCombination(
                DifficultyLevel.Medium,
                new List<ObstacleType>() { ObstacleType.B1, ObstacleType.T3 },
                new List<float>() { 0, 1 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Medium,
                new List<ObstacleType>() { ObstacleType.B2, ObstacleType.T2 },
                new List<float>() { 0, 1 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Medium,
                new List<ObstacleType>() { ObstacleType.B3, ObstacleType.T1 },
                new List<float>() { 0, 1 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Medium,
                new List<ObstacleType>() { ObstacleType.B2, ObstacleType.T4 },
                new List<float>() { 0, 0 }
            ),
            new ObstacleCombination(
                DifficultyLevel.Medium,
                new List<ObstacleType>() { ObstacleType.B4, ObstacleType.T2 },
                new List<float>() { 0, 0 }
           )
        };


        #endregion


        #region 中级难度的障碍物组合
        List<ObstacleCombination> combs_High = new List<ObstacleCombination>()
        {
            //// 高级-1 /////
            // 第一行
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.B2,
                    ObstacleType.B2,
                    ObstacleType.B1,
                    ObstacleType.T4
                },
                new List<float>() {
                    0,
                    1,
                    2,
                    3,
                    1.5f
                }
            ),
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B2,
                    ObstacleType.T2,
                    ObstacleType.B2
                },
                new List<float>() {
                    0,
                    1,
                    2
                }
            ),
            // 第二行
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B2,
                    ObstacleType.T2,
                    ObstacleType.B1,
                    ObstacleType.T3,
                },
                new List<float>() {
                    0,
                    1,
                    2,
                    3
                }
            ),
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B3,
                    ObstacleType.T1,
                    ObstacleType.B1,
                    ObstacleType.T3,
                },
                new List<float>() {
                    0,
                    1,
                    2,
                    3
                }
            ),
            // 第三行， （第三行第一个与第二行第二个重复了）
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.T3,
                    ObstacleType.B2,
                    ObstacleType.T2,
                },
                new List<float>() {
                    0,
                    1,
                    2.5f,
                    3.5f
                }
            ),

            //// 高级-2 /////
            // 第一行
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.T3,
                    ObstacleType.B3,
                    ObstacleType.T1,
                },
                new List<float>() {
                    0,
                    1,
                    3,
                    4
                }
            ),
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B2,
                    ObstacleType.T2,
                    ObstacleType.B3,
                    ObstacleType.T1,
                },
                new List<float>() {
                    0,
                    1,
                    3,
                    4
                }
            ),
            // 第二行
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.B1,
                    ObstacleType.T3,
                },
                new List<float>() {
                    0,
                    3,
                    4
                }
            ),
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.B2,
                    ObstacleType.T2,
                },
                new List<float>() {
                    0,
                    3,
                    4
                }
            ),
            //第三行
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.B3,
                    ObstacleType.T1,
                },
                new List<float>() {
                    0,
                    3,
                    4
                }
            ),
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.B2,
                    ObstacleType.T4,
                },
                new List<float>() {
                    0,
                    3,
                    3
                }
            ),

            //// 高级-3 /////
            // 第一行            
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.B4,
                    ObstacleType.T2,
                },
                new List<float>() {
                    0,
                    3,
                    3
                }
            ),
            new ObstacleCombination(
                DifficultyLevel.High,
                new List<ObstacleType>() {
                    ObstacleType.B1,
                    ObstacleType.B1,
                    ObstacleType.B2,
                    ObstacleType.B2,
                    ObstacleType.B1,
                    ObstacleType.T4,
                },
                new List<float>() {
                    0,
                    3,
                    4,
                    5,
                    6,
                    4.5f
                }
            ),


        };

        #endregion

        ObstacleCombinations.Add(DifficultyLevel.Low, combs_Low);
        ObstacleCombinations.Add(DifficultyLevel.Medium, combs_Medium);
        ObstacleCombinations.Add(DifficultyLevel.High, combs_High);
    }



    #endregion

}

