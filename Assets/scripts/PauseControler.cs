using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseControler : MonoBehaviour {

	public GameObject ingameMenu;//菜单

	//-----------数据显示1.定义 start-------
	public Text Time_value;//时间显示
	public Text  Dogs_value;//狗数显示
	public int dogs = 0; 
	//-----------数据显示1 end-------

	public void onPause()
	{
		//-----------数据显示2.赋值 start-------
		dogs++;
		//Dogs_value.text = dogs.ToString();
		//Time_value.text = dogs+"秒";
		//-----------数据显示2 end-------
		
		Time.timeScale = 0;
		ingameMenu.SetActive (true);
	}
	public void OnResume()
	{
		Time.timeScale = 1f;
		ingameMenu.SetActive (false);
	}
	
	public void OnRestart()
	{
		//Application.LoadLevel("sceneMain");

		Application.LoadLevel("loading");
		Time.timeScale = 1f;//乱改，原因不明
		//---------Application.LoadLevelAsync("sceneMain");
		//Application.LoadLevelAdditive ("sceneMain");
	}

}
