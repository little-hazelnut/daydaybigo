using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameoverScript : MonoBehaviour {

	public static GameoverScript Instance;

	Button b_restart;
	Button b_toFirstPage;

	Text playerScore_text;

	// Use this for initialization
	void Start () {
	
		//UnityAction<BaseEventData> reStartButdownAciton =
		//	new UnityAction<BaseEventData> (OnButRestart);
		//UnityAction<BaseEventData> toFirPgeButdownAciton =
		//	new UnityAction<BaseEventData> (OnButToFirPge);


		foreach (Transform t in this.GetComponentInChildren<Transform>())
		{
			if(t.name.CompareTo("ReStart")==0) 	// find button restart
			{
				b_restart = t.GetComponent<Button>();
				b_restart.onClick.AddListener(
					delegate(){
						this.OnButRestart();
					}
				);
			}else if(t.name.CompareTo("ToFirstPage")==0)	//find button toFirstPage
			{
				b_toFirstPage = t.GetComponent<Button>();
				b_toFirstPage.onClick.AddListener(
					delegate {
						this.OnButToFirPge();
					}
				);
			}
			else if(t.name.CompareTo("PlayerScore")==0)
			{
				playerScore_text = t.GetComponent<Text>();
				setPlayerScore(playerScore_text);
			}else if(t.name.CompareTo("BestScore")==0)
			{
				//playerScore_text = t.GetComponent<Text>();
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Awake()
	{
		Instance = this;
	}

	void OnButRestart()
	{
		Application.LoadLevel ("sceneMain");
	}

	void OnButToFirPge()
	{
		Application.LoadLevel ("sceneStart");
	}

	void setPlayerScore(Text t){
		playerScore_text.text = State.getNumDogsCaught()+"";
	}

	void setMaxScore(Text t){
		//max score
	}
	
}
