using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCameraController : MonoBehaviour {

    /// <summary>
    /// 人，在此中引用，主要是方便调用
    /// </summary>
    private GameObject runner = null;

    /// <summary>
    /// 狗，在此中引用，主要是方便调用
    /// </summary>
    private GameObject dog = null;


    private List<GameObject> backgrounds = new List<GameObject>();
	private List<GameObject> obstacles = new List<GameObject>();

    private List<ObstacleCombination> obstacleCombinations = new List<ObstacleCombination>(); 

	/// <summary>
	/// 障碍物可用的Sprite列表，当生成障碍物时，从此列表中选取Sprite。
	/// </summary>
	public List<Sprite> obstacleAvailableSprites;

	/// <summary>
	/// 摄像机可视范围为的高度，也即场景高度
	/// </summary>
	private float heightCam = 0;	
	/// <summary>
	/// 摄像机可视范围为的宽度，也即场景宽度
	/// </summary>
	private float widthCam = 0;

	/// <summary>
	/// 背景图片的宽度
	/// </summary>
	private float widthBackground = 0;

	/// <summary>
	/// 当前场景中障碍物的个数
	/// </summary>
	private int numObstacles = 0;

	/// <summary>
	/// 障碍物的y值
	/// </summary>
	public float yObstaclesBottom = -1.5f;


    /// <summary>
    /// 判断人追上狗的距离
    /// </summary>
    public float distanceCathUp = 0.2f;

    /// <summary>
    /// 狗离障碍物多远时需要起跳。
    /// 其它狗的起跳速度和高度应结合障碍物的高度和宽度，但目前暂时简单处理
    /// </summary>
    public float distanceDogToJump = 2.0f;

    /// <summary>
    /// 最新出现的障碍物组合
    /// </summary>
    private ObstacleCombination lastCombination = null;

    /// <summary>
    /// 相邻障碍物组合的距离，距离在此基础上会有一个小的随机浮动
    /// </summary>
    public float distanceEachCombination = 8;

    /// <summary>
    /// 低难度障碍物组合出现的概率
    /// </summary>
    public float probabilityLow = 0.5f;
    /// <summary>
    /// 中难度障碍物组合出现的概率
    /// </summary>
    public float probabilityMedium = 0.0f;
    /// <summary>
    /// 高难度障碍物组合出现的概率
    /// </summary>
    public float probabilityHigh = 0.5f;



    void Start () {

        heightCam = Settings.HeightCamera;// 2.0f * Camera.main.orthographicSize;
        widthCam = Settings.WidthCamera;// heightCam * Camera.main.aspect;
		//float backgroundWidth = 
		Debug.Log(string.Format("camWidth: {0} ; camHeight: {1} ", widthCam, heightCam));

        runner = GameObject.Find("runner");
        dog = GameObject.Find("dog");

        InitBackgrounds();
        
        InitObstacles();
    }

    /// <summary>
    /// 之前原为Update()
    /// Update()是每帧画面更新时调用；FixedUdate()是固定时间调用，其时间通过Unity中Edit--Project Setting--Time修改
    /// </summary>
    void FixedUpdate() //Update() 
	{		
		UpdateBackgrounds();
        
        UpdateObstacles();

        CheckDogToJump();

        CheckCatchUp();

        RemoveObstacleCombinationOutside();
    }
    

    void InitObstacles()
    {
        ObstacleCombination newComb = GenNewCombination(DifficultyLevel.Low, new Vector3(Settings.WidthCamera, 0, 0), 2);        
    }

    /// <summary>
    /// 初始化背景。
    /// </summary>
    void InitBackgrounds()
	{
		GameObject backMiddle = GameObject.Find("background") as GameObject;
		Bounds backBounds = (backMiddle.transform.GetComponent<SpriteRenderer>()).bounds;
		widthBackground = backBounds.size.x;
		
		
		GameObject backLeft = (GameObject)Instantiate(backMiddle);
		GameObject backRight = (GameObject)Instantiate(backMiddle);
		backLeft.transform.position = new Vector3(-widthBackground, 0, 0);		
		backRight.transform.position = new Vector3(widthBackground, 0, 0);
		
		//背景从左到右排序，方便更新背景时调整位置。
		backgrounds.Add(backLeft);
		backgrounds.Add(backMiddle);
		backgrounds.Add(backRight);
		
		Debug.Log(string.Format("widthBackground: {0}  ",  widthBackground));		
	}
        
	
	/// <summary>
	/// 更新背影。主要动态改变墙的位置。
	/// </summary>
	void UpdateBackgrounds()
	{
		//简化，由于视角一直往右边移动，所以只判断最左边的墙是否超出可见范围。
		int i=0;
		{
			Vector3 backPos = backgrounds[i].transform.position;
			if(IsBackgroundLeftOutside_X(backgrounds[i]))
			{
				backgrounds[i].transform.position = new Vector3(backPos.x + widthBackground * backgrounds.Count, backPos.y, backPos.z);
				
				//一直保持最左边的墙在backgrounds列表中的最前面
				GameObject tempBack = backgrounds[i];
				backgrounds.RemoveAt(i);
				backgrounds.Add(tempBack);
			}
		}
	}
	
	/// <summary>
	/// 判断墙是否在超出屏幕左侧，只判断x方向上的。
	/// </summary>
	/// <returns><c>true</c> if this instance is point in view_ x the specified pos; otherwise, <c>false</c>.</returns>
	/// <param name="pos">Position.</param>
	bool IsBackgroundLeftOutside_X(GameObject background)
	{
		Vector3 pos = background.transform.position;
		Vector3 posCam = Camera.main.transform.position;
		
		if(pos.x + widthBackground/2.0f < posCam.x - widthCam / 2.0f ) //|| pos.x - widthBackground/2.0f > posCam.x + widthCam /2.0f
		{
			return true;
		}
		return false;
	}
    

    /// <summary>
    /// 更新障碍物
    /// </summary>
    void UpdateObstacles()
    {
        if (IsReadyToGenNewCombination())
        {
            DifficultyLevel diff = GenDifficultyLevel();
            float x = Camera.main.transform.position.x + Settings.WidthCameraHalf + Random.value * 3;
            float distanceFactor = 3;

            ObstacleCombination newComb = GenNewCombination(diff, new Vector3(x, 0, 0), distanceFactor);
        }
    }

    /// <summary>
    /// 根据低、中、高难度的概率，生成难度级别
    /// </summary>
    /// <returns></returns>
    DifficultyLevel GenDifficultyLevel()
    {
        float rand = Random.value;
        Debug.Log(rand);
        if (rand < probabilityLow)
        {
            return DifficultyLevel.Low;
        }
        else if (rand < probabilityLow + probabilityMedium)
        {
            return DifficultyLevel.Medium;
        }
        else
        {
            return DifficultyLevel.High;
        }
    }


    bool IsReadyToGenNewCombination()
    {
        float xLastComb = lastCombination.GetPosition().x;
        float xCamera = Camera.main.transform.position.x;

        if (xCamera - xLastComb - lastCombination.Width + Settings.WidthCameraHalf > distanceEachCombination)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 创建新的障碍物组合；
    /// 根据各参数从Settings.ObstacleCombinations中设置的已有组合中随机选取一个组合以生成障碍物。
    /// </summary>
    /// <param name="difficulty">游戏难度</param>
    /// <param name="position">组合的位置，组合中障碍物的位置相对于组合的位置</param>
    /// <param name="distanceFactor">各障碍物的距离因子</param>
    /// <returns></returns>
    ObstacleCombination GenNewCombination(DifficultyLevel difficulty, Vector3 position, float distanceFactor)
    {
        int indexCombination_Low = Random.Range(0, Settings.ObstacleCombinations[difficulty].Count);
        ObstacleCombination newComb = Settings.ObstacleCombinations[difficulty][indexCombination_Low];
        newComb.GenObstacles(distanceFactor);
        newComb.SetPosition(position);

        lastCombination = newComb;
        obstacleCombinations.Add(newComb);

        return newComb;
    }

    /// <summary>
    /// 移除掉超出屏幕右边的障碍物
    /// </summary>
    void RemoveObstacleCombinationOutside()
    {
        int countCombs = obstacleCombinations.Count;

        for(int i= 0; i < countCombs; i++)
        {
            ObstacleCombination comb = obstacleCombinations[i];

            Vector3 posComb = comb.Position;
            Vector3 posCam = Camera.main.transform.position;

            //考虑到障碍物有一定宽度，暂且在最后加多一项-10
            if (posComb.x + comb.Width < posCam.x - Settings.WidthCameraHalf - 10)
            {
                Debug.Log("comb.Width: " + comb.Width);
                Debug.Log("destroy one ObstacleCombination Outside");
                obstacleCombinations.RemoveAt(i);// (comb);
                comb.Destory();
                comb = null;

                i--;
                countCombs--;
            }
        }
        
    }

    
    /// <summary>
    /// 判断狗是否需要起跳以躲避障碍
    /// </summary>
    /// <returns></returns>
    bool CheckDogToJump()
    {
        Vector3 posDog = dog.transform.position;

        foreach (GameObject obstacle in obstacles)
        {
            Vector3 posObst = obstacle.transform.position;

            if (posObst.x > posDog.x && posObst.x - posDog.x < distanceDogToJump)
            {
                dog.SendMessage("ReadyToJump");

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 判断是否人追上狗
    /// </summary>
    void CheckCatchUp()
    {
        Vector3 posRunner = runner.transform.position;
        Vector3 posDog = dog.transform.position;

        //Debug.Log(string.Format("posRunner: {0},{1} ", posRunner.x, posRunner.y, posRunner.z));

        float distance = posDog.x - posRunner.x;
        if(distance < distanceCathUp)
        {
            Debug.Log(string.Format("Caught Up. Distance: {0}", distance));
            OnCaughtUp();
        }
    }

    /// <summary>
    /// 当人追上狗后的处理接口
    /// </summary>
    void OnCaughtUp()
    {
        State.IsCaughtUp = true;
                
        //do sth here...

    }



}
























