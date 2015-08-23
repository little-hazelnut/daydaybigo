using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerController : MonoBehaviour {


	public Sprite[] sprites;
	public float framesPerSecond = 15;
	private SpriteRenderer spriteRenderer;
	Animator anim;//ackhan:1
	/// <summary>
	/// runner在X方向上的初始速度
	/// </summary>
	public float speedInitX
    {
        get { return Settings.SpeedNormal; }
    }

    /// <summary>
    /// runner跳起时的向上初始速度
    /// </summary>
    public float speedJumpUp = 20f;
	/// <summary>
	/// runner的重力加速度
	/// </summary>
	public float gravityAcc = -150f;

	/// <summary>
	/// runner的当前Y速度
	/// </summary>
	private float speedY = 0f;
	/// <summary>
	/// runner的当前X方向速度
	/// </summary>
	private float speedX = 0f;

	/// <summary>
	/// runner是否处于跳起状态
	/// </summary>
	private bool isJumpping = false;

    /// <summary>
    /// 每一次起跳时人的速度的增加量
    /// </summary>
    public float speedUpEachJump = 0.2f;

    /// <summary>
    /// runner在地面上的时候的y值,
    /// </summary>
    public static float yOnGround = Settings.YRunnerOnFloor;// -1.5f;

    /// <summary>
    /// 手机的重力感应所产生的加速度因子，与Input.acceleration.x的乘积构成重力感应所产生的X方向加速度。
    /// </summary>
    public float mobileAccFactor = 5f;

    private RunnerState runnerState { get { return State.RunnerState; } }

    private GameObject mainCamera = null;

    // Use this for initialization
    //void Start () {		
    void Awake()
    {
        anim = GetComponent<Animator> ();//ackhan:2
		spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;

        speedX = Settings.SpeedInit_Runner; //   

        mainCamera = GameObject.Find("Main Camera");
	}

    /// <summary>
    /// 之前原为Update()
    /// Update()是每帧画面更新时调用；FixedUdate()是固定时间调用，其时间通过Unity中Edit--Project Setting--Time修改
    /// </summary>
	void FixedUpdate() //Update() 
    {
		anim.SetFloat ("Vspeed", GetComponent<Rigidbody2D> ().velocity.y);//ackhan:5
		//检测是否有点击事件，并作相应处理；目前主要是处理起跳
		if(Input.GetButtonDown("Fire1"))
		{
			anim.SetBool("Ground", false);//ackhan:3
            ReadyToJump();
		}
		anim.SetBool("Ground", true);//ackhan:4
		//更新sprite
		int index = (int)(Time.timeSinceLevelLoad * framesPerSecond);
		index = index % sprites.Length;
		spriteRenderer.sprite = sprites[index];

        // 根据重力感应的增加X方向速度
        UpdateSpeedX();

		//更新位置
		UpdatePosition();
	}

    void UpdateSpeedX()
    {
        switch (runnerState)
        {
            case RunnerState.SpeedingUp:
                speedX += Time.fixedDeltaTime * Settings.AccSpeedUp_Runner + Input.acceleration.x * mobileAccFactor;

                //加速完成，开始进入搜狗状态
                if (speedX >= Settings.SpeedNormal)
                {
                    speedX = Settings.SpeedNormal;
                    //触发Main Camera中的OnEndedSpeedUp()
                    mainCamera.SendMessage("OnEndedSpeedUp");
                }
                break;

            case RunnerState.SearchingDog:
            case RunnerState.Chasing:
                speedX = speedInitX + Input.acceleration.x * mobileAccFactor;

                break;
            case RunnerState.CaughtUp:
                speedX = 0;
                break;
            default:
                throw new System.Exception("状态不对，很不对");
        }
    }

    void UpdatePosition_SpeedingUp()
    {
    }
        

	/// <summary>
	/// 起跳处理
	/// </summary>
	void ReadyToJump()
	{
        //如果已经是跳起状态，不作处理；
        //如果要连跳的话，注释此处
        //if(isJumpping)
        //{
        //	return;
        //}
        speedY = speedJumpUp;
		isJumpping = true;
	}

	/// <summary>
	/// 更新位置
	/// </summary>
	void UpdatePosition()
	{
        if(runnerState == RunnerState.CaughtUp)
        {
            //若处于追到狗的状态，暂时先停止跑，待交互处理
            //加分----------
            GameObject GO_canvas = GameObject.Find("Canvas");
            GO_canvas.SendMessage("updateScore", null);
            //----------
            return;
        }

		float x = CalculateX();
		float y = yOnGround;

		//如果正处于跳起状态,则需要重新计算y坐标
		if(isJumpping)
		{
			anim.SetBool("Ground", false);//ackhan:6
			y = CalculateY();

			if(y <= yOnGround)
			{
				y=yOnGround;
				speedY = 0f;
				isJumpping=false;
			}
          
		}

		transform.position = new Vector3(x, y, 0f);
    }

	/// <summary>
	/// 计算runner的X坐标
	/// </summary>
	float CalculateX()
	{
		return transform.position.x + speedX * Time.deltaTime;
	}

	/// <summary>
	/// 计算runner的Y坐标
	/// </summary>
	float CalculateY()
	{
		speedY = speedY + gravityAcc * Time.deltaTime;
		return transform.position.y + speedY * Time.deltaTime;
	}


    /// <summary>
    /// 当人发生碰撞时的处理函数；
    /// 注意是OnCollisionEnter2D， 不是OnCollisionEnter；同时，参数是Collision2D，不是Collision
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(string.Format("on Runner Collider Collision ... count ") + collision.collider.tag + " " + collision.collider.name);

        //当碰撞时，暂时先是处理为速度恢复为初始速度；
        //待添加上重力感应功能后再另行处理
        //speedX = speedInitX;

        if (collision.collider.name == Settings.Text_Obstacle)
        {
            OnCollisionToObstacle();
        }

    }

    /// <summary>
    /// 当碰到障碍物时的处理接口，如减时间
    /// </summary>
    void OnCollisionToObstacle()
    {

    }

    void bigoingStart(){
		anim.SetBool("Bigo", true);
		Invoke("bigoingEnd", 1.5f);//等待3s后结束bigo
	}
	
	void bigoingEnd()
	{
		anim.SetBool("Bigo", false);
		//Application.LoadLevelAsync ("sceneMain");
		//Application.LoadLevel("sceneMain");//重新开始还是有问题


		////
		speedX = 0;
		mainCamera.SendMessage ("OnEndedInteraction");
	}


}
