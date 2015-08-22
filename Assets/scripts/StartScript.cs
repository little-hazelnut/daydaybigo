using UnityEngine;

public class StartScript : MonoBehaviour {

    void Start()
    {
        //由于从游戏进程是从StartScript开始的，为Main Thread主进程；在主进程中先初始化一些东西；
        ObstacleCombination.InitObstacleCombination();
    }

    public void OnStart()
	{
		Application.LoadLevel("sceneMain");
	}
}
