using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //bigo numbers
    //public static int score;
    Text score_text;
    Text time_text;
    
    void Awake()
    {
        Instance = this;
    }

    void Start() {
        foreach (Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("Score") == 0)
            {
                score_text = t.GetComponent<Text>();
            }
            if (t.name.CompareTo("Time") == 0)
            {
                time_text = t.GetComponent<Text>();
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void updateScore() {
        score_text.text = "Bigo!:   <color=yellow>" + State.updateNumDogsCaught() + "</color>";
        //string.Format("Bigo!:   <color=yellow>{0}</color>",score);
    }

    void updateTime() {

    }
}




