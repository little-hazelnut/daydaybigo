using UnityEngine;
using System.Collections;

public class button_start : MonoBehaviour {

	public GameObject ingameMenu;//菜单
	public int a = 0; 


	/*
	void OnMouseDown() {
		Debug.Log ("mouseDown");
		ingameMenu.SetActive (false);
	}
	*/


	public void onScore()
	{
		a++;
		if(a%2 ==0)	ingameMenu.SetActive (false);
		else ingameMenu.SetActive (true);
		
	}


}
