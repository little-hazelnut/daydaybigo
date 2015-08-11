using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCameraController : MonoBehaviour {
	
	private List<GameObject> backgrounds = new List<GameObject>();
	private List<GameObject> obstacles = new List<GameObject>();
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

	// Use this for initialization
	void Start () {
		
		heightCam = 2.0f * Camera.main.orthographicSize;
		widthCam = heightCam * Camera.main.aspect;
		//float backgroundWidth = 
		Debug.Log(string.Format("camWidth: {0} ; camHeight: {1} ", widthCam, heightCam));
		
		InitBackgrounds();
		InitObstacles();
	}
	
	// Update is called once per frame
	void Update () 
	{		
		UpdateBackgrounds();
		UpdateObstacles();
	}


	/// <summary>
	/// 启动时即初始化背景。
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
	/// 启动时即初始化障碍物。
	/// </summary>
	void InitObstacles()
	{
		float xObstacle = 2.0f;
		GameObject newObstacle = GenNewObstacle(xObstacle);
	}

	/// <summary>
	/// 计算障碍物的y值
	/// </summary>
	/// <returns>The obstacle y.</returns>
	/// <param name="obstacle">Obstacle.</param>
	float CalObstacleY(SpriteRenderer obstacle)
	{
		float obstacleHeight = obstacle.bounds.size.y;
		return yObstaclesBottom + obstacleHeight /2.0f;
	}

	/// <summary>
	/// 生成新的障碍物，并根据x值放置
	/// </summary>
	/// <returns>The new obstacle.</returns>
	/// <param name="xObstacle">X obstacle.</param>
	GameObject GenNewObstacle(float xObstacle)
	{
		GameObject newObstacle = new GameObject();

		SpriteRenderer spriteRenderer = newObstacle.AddComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerName = "obstacle";
		int indexSprite = Random.Range(0, obstacleAvailableSprites.Count);		
		Debug.Log(string.Format("indexSprite: {0}  ",  indexSprite));
		spriteRenderer.sprite = obstacleAvailableSprites[indexSprite];

		BoxCollider2D boxCollider2D = newObstacle.AddComponent<BoxCollider2D>();



		float yObstacle = CalObstacleY(spriteRenderer);
		newObstacle.transform.position = new Vector3(xObstacle, yObstacle, 0);

		//
		obstacles.Add(newObstacle);

		return newObstacle;
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
	/// 更新障碍物。
	/// 注意，内部实现待完善
	/// </summary>
	void UpdateObstacles()
	{
		//简化，由于视角一直往右边移动，所以只判断最左边的墙是否超出可见范围。
		int i=0;
		{
			Vector3 obstPos = obstacles[i].transform.position;
			if(IsObstacleLeftOutside_X(obstacles[i]))
			{
				float rand = Random.Range(1f, 3f);
				
				Debug.Log(string.Format("rand: {0}  ",  rand));
				float xNew = Camera.main.transform.position.x + widthCam / 2.0f * (rand  + 1.0f ); 

				obstacles[i].transform.position = new Vector3(xNew, obstPos.y, obstPos.z);	


				//注意，此处暂时简单更改spriteRender，但不确定会不会自己更新box collider 2d的bounds。待更细处理
				int indexSprite = Random.Range(0, obstacleAvailableSprites.Count);		
				Debug.Log(string.Format("indexSprite: {0}  ",  indexSprite));
				obstacles[i].GetComponent<SpriteRenderer>().sprite = obstacleAvailableSprites[indexSprite];


			}
		}
	}

	bool IsObstacleLeftOutside_X(GameObject obstacle)
	{
		Vector3 pos = obstacle.transform.position;
		Vector3 posCam = Camera.main.transform.position;
		float widthObstacle = obstacle.GetComponent<SpriteRenderer>().bounds.size.y;
		
		if(pos.x + widthObstacle/2.0f < posCam.x - widthCam / 2.0f )
		{
			return true;
		}
		return false;
	}
}
























