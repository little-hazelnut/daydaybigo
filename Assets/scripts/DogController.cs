using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DogController : MonoBehaviour {
		
	public Sprite[] sprites;
	public float framesPerSecond= 15;
	private SpriteRenderer spriteRenderer;
	
	/// <summary>
	/// runner在X方向上的初始速度
	/// </summary>
	public float speedInitX = 5;
	
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
    /// runner在地面上的时候的y值,
    /// </summary>
    private static float yOnGround = Settings.YDogOnFloor;//-2.0f;
    
	void Start () {		
		spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;

		speedX = speedInitX;

	}

    /// <summary>
    /// 之前原为Update()
    /// Update()是每帧画面更新时调用；FixedUdate()是固定时间调用，其时间通过Unity中Edit--Project Setting--Time修改
    /// </summary>
    void FixedUpdate() //Update()
    {		
		//更新sprite
		int index = (int)(Time.timeSinceLevelLoad * framesPerSecond);
		index = index % sprites.Length;
		spriteRenderer.sprite = sprites[index];
		
		//更新位置
		UpdatePosition();
	}

	
	/// <summary>
	/// 更新位置
	/// </summary>
	void UpdatePosition()
    {
        if (State.IsCaughtUp)
        {
            //若处于追到狗的状态，暂时先停止跑，待交互处理
            return;
        }

        float x = CalculateX();
		float y = yOnGround;
		
		//如果正处于跳起状态,则需要重新计算y坐标
		if(isJumpping)
		{
			y = CalculateY();
			
			if(y <= yOnGround)
			{
				y=yOnGround;
				speedY = 0f;
				isJumpping=false;
			}
		}
		
		transform.position = new Vector3(x, y, 0f);

        //让摄像机在水平方向
        if (!Settings.IsCameraFollowRunner)
        {
            Camera.main.transform.position = new Vector3(
            transform.position.x - 2.0f,
            Camera.main.transform.position.y,
            Camera.main.transform.position.z);
        }
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
    /// 起跳处理
    /// </summary>
    void ReadyToJump()
    {
        //如果已经是跳起状态，不作处理
        if (isJumpping)
        {
            return;
        }

        speedY = speedJumpUp;
        isJumpping = true;
    }

    


}






