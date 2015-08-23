using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 障碍物组合类
/// </summary>
public class ObstacleCombination : UnityEngine.Object
{
    /// <summary>
    /// 组合所对应的难度级别
    /// </summary>
    public DifficultyLevel Difficulty = DifficultyLevel.Low;

    /// <summary>
    /// 各障碍物的类型；其长度需与ObstacleDistances一致；
    /// 注意，中心最左的障碍物需置于该集合的第0位置，中心最右的需于最尾。
    /// </summary>
    public List<ObstacleType> ObstacleTypes = null;

    /// <summary>
    /// 各障碍物相对于Container的x距离；其长度需与ObstacleTypes一致；
    /// 注意，此处距离是初始距离，实际距离=初始始距离*因子；
    /// 因子与难度和速度有关：在低难度、人正常速度时等于1，随着难度和速度的增加，因子增加。
    /// 注意，中心最左的障碍物需置于该集合的第0位置，中心最右的需于最尾。
    /// </summary>
    public List<float> ObstacleDistances = null;

    /// <summary>
    /// 组合中障碍物个数
    /// </summary>
    public int Count { get { return this.ObstacleTypes == null ? 0 : this.ObstacleTypes.Count; } }

    /// <summary>
    /// 组合所占宽度，为从第0个到最后一个之间的x距离
    /// </summary>
    public float Width
    {
        get
        {
            return this.ObstacleDistances == null || this.ObstacleDistances.Count == 0 ?
                0f : Mathf.Abs(this.ObstacleDistances[this.ObstacleDistances.Count - 1] - this.ObstacleDistances[0]) * distanceFactor;
        }
    }

    /// <summary>
    /// 组合的位置,也即Container的位置
    /// </summary>
    public Vector3 Position {
        get
        {
            if(this.Container == null)
            {
                throw new System.Exception("Container为空. 必须先初始化Container");
            }
            return this.Container.transform.position;
        }
    }
    

    /// <summary>
    /// 组合中所有障碍物的容器GameObject, 将障碍物 List《GGameObject》 Obstacles 作为其子GameObject，可以统一控制各障碍物的位置
    /// </summary>
    private GameObject Container = null;// new GameObject();

    public static Dictionary<ObstacleType, Sprite> ObstacleAvailableSprites = new Dictionary<ObstacleType, Sprite>();

    private static Dictionary<ObstacleType, GameObject> ObstaclesDictionary = new Dictionary<global::ObstacleType, GameObject>();

    private static Dictionary<ObstacleType, float> ObstaclesYPosition = new Dictionary<ObstacleType, float>();

    private float distanceFactor = 1;

    /// <summary>
    /// 静态构造函数，
    /// 在此函数中初始化一些全局使用的变量，如生成所有类型的障碍物对应的Sprite、GameObject等
    /// </summary>
    static ObstacleCombination()
    {
        InitObstacleCombination();
    }

    public static void InitObstacleCombination()
    {
        ObstacleAvailableSprites = new Dictionary<ObstacleType, Sprite>();
        ObstaclesYPosition = new Dictionary<ObstacleType, float>();

        //生成障碍物Sprite集合和Y值集合；由于对于每一个障碍物，这两者都是固定的，所以可以提前生前并作为静态变量供全局使用
        foreach (ObstacleType obstType in System.Enum.GetValues(typeof(ObstacleType)).Cast<ObstacleType>().ToList())
        {
            //Sprite集合
            //运行时加载各个障碍物图作为Sprite
            Sprite sp = Resources.Load<Sprite>(obstType.ToString());
            ObstacleAvailableSprites.Add(obstType, sp);

            //Y值集合
            ObstaclesYPosition.Add(obstType, CalObstaclesY(obstType));
        }
    }

    /// <summary>
    /// 根据障碍物类型计算其在场景中的Y值
    /// </summary>
    /// <param name="obstType"></param>
    /// <returns></returns>
    private static float CalObstaclesY(ObstacleType obstType)
    {
        float yObstacle = 0f;

        switch (obstType)
        {
            case ObstacleType.B1:
                yObstacle = Settings.GetYOnFloor(Settings.HeightB1);
                break;
            case ObstacleType.B2:
                yObstacle = Settings.GetYOnFloor(Settings.HeightB2);
                break;
            case ObstacleType.B3:
                yObstacle = Settings.GetYOnFloor(Settings.HeightB3);
                break;
            case ObstacleType.B4:
                yObstacle = Settings.GetYOnFloor(Settings.HeightB4);
                break;

            case ObstacleType.T1:
                yObstacle = Settings.GetYUnderCeil(Settings.HeightT1);
                break;
            case ObstacleType.T2:
                yObstacle = Settings.GetYUnderCeil(Settings.HeightT2);
                break;
            case ObstacleType.T3:
                yObstacle = Settings.GetYUnderCeil(Settings.HeightT3);
                break;
            case ObstacleType.T4:
                yObstacle = Settings.GetYUnderCeil(Settings.HeightT4);
                break;

            default:
                throw new System.Exception("Invaild ObstacleType.");
        }

        return yObstacle;
    }


    public ObstacleCombination(DifficultyLevel difficulty, List<ObstacleType> types, List<float> distances)
    {
        if (types == null || distances == null || types.Count != distances.Count)
        {
            throw new System.Exception("参数 types, distances 不可为null，且长度需相等");
        }

        this.Difficulty = difficulty;
        this.ObstacleTypes = types;
        this.ObstacleDistances = distances;
    }

    private GameObject GenObstacleObject(ObstacleType type)
    {
        GameObject newObstacle = new GameObject();
        newObstacle.name = Settings.Text_Obstacle;

        SpriteRenderer spriteRenderer = newObstacle.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "obstacle";
        int indexSprite = (int)type;// Random.Range(0, obstacleAvailableSprites.Count);
        Debug.Log(string.Format("indexSprite: {0}  ", indexSprite));
        spriteRenderer.sprite = Sprite.Instantiate(ObstacleAvailableSprites[type]); //ObstacleAvailableSprites[type];



        BoxCollider2D boxCollider2D = newObstacle.AddComponent<BoxCollider2D>();

        return newObstacle;
    }



    /// <summary>
    /// 生成障碍物组合中的List《GameObject》 Obstacles
    /// </summary>
    /// <param name="distanceFactor">与速度相关的因子：速度越大时，需要把障碍物之间的距离相应并大；实际距离=每个障碍物的设置距离*因子</param>
    public void GenObstacles(float distanceFactor)
    {
        if (this.Count == 0 ||
            this.ObstacleTypes == null || this.ObstacleTypes.Count != this.Count ||
            this.ObstacleDistances == null || this.ObstacleDistances.Count != this.Count)
        {
            throw new System.Exception("参数未完全初始化。");
        }

        Container = new GameObject();
        this.distanceFactor = distanceFactor;

        for (int i = 0; i < Count; i++)
        {
            ObstacleType type = ObstacleTypes[i];

            GameObject obstacle = GenObstacleObject(type);

            obstacle.transform.parent = Container.transform;

            //注意最终距离还需要乘以一下与速度相关的因子：速度越大时，需要把障碍物之间的距离相应并大
            float x = ObstacleDistances[i] * distanceFactor;

            obstacle.transform.position = new Vector3(x, ObstaclesYPosition[type], 0);
        }
    }

    /// <summary>
    /// 设置组合的位置，也即Container的位置；
    /// 由于组合里的所有障碍物都是Container的子GameObject，各障碍物的Transform是相对于Container的，因此只需要设置Container的位置以及各障碍物的相对Postion，即确定了各障碍物的位置
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector3 position)
    {
        this.Container.transform.position = position;
    }

    /// <summary>
    /// 设置组合的x值，也即Container的x；其y值和z默认为0；
    /// 由于组合里的所有障碍物都是Container的子GameObject，各障碍物的Transform是相对于Container的，因此只需要设置Container的位置以及各障碍物的相对Postion，即确定了各障碍物的位置
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(float xPosition)
    {
        this.SetPosition(new Vector3(xPosition, 0, 0));
    }

    public Vector3 GetPosition()
    {
        return this.Position;
    }

    /// <summary>
    /// 销毁障碍物
    /// </summary>
    public void Destory()
    {
        Object.Destroy(this.Container);
    }
}