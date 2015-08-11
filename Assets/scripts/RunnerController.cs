using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerController : MonoBehaviour {


	public Sprite[] sprites;
	public float framesPerSecond= 60;
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
	private static float yOnGround = -1.5f;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;

		speedX = speedInitX;

	}
	
	// Update is called once per frame
	void Update () 
	{
		//检测是否有点击事件，并作相应处理；目前主要是处理起跳
		if(Input.GetButton("Fire1"))
		{
			ReadyToJump();
		}

		//更新sprite
		int index = (int)(Time.timeSinceLevelLoad * framesPerSecond);
		index = index % sprites.Length;
		spriteRenderer.sprite = sprites[index];

		//更新位置
		UpdatePosition();

	}

			  


	/// <summary>
	/// 起跳处理
	/// </summary>
	void ReadyToJump()
	{
		//如果已经是跳起状态，不作处理
		if(isJumpping)
		{
			return;
		}
		
		speedY = speedJumpUp;
		isJumpping = true;
	}

	/// <summary>
	/// 更新位置
	/// </summary>
	void UpdatePosition()
	{
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

				speedX += 0.5f;
			}
		}

		transform.position = new Vector3(x, y, 0f);

		//让摄像机在水平方向
//		Camera.main.transform.position = new Vector3(
//			transform.position.x, 
//			Camera.main.transform.position.y, 
//			Camera.main.transform.position.z);
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


	




}
