using UnityEngine;

public class StartScript : MonoBehaviour {

	//音乐文件
	public AudioSource music;

    void Start()
    {
		//设置默认音量
		music.volume = 0.5F;
		music.Play();

        //由于从游戏进程是从StartScript开始的，为Main Thread主进程；在主进程中先初始化一些东西；
        ObstacleCombination.InitObstacleCombination();
    }

    public void OnStart()
	{
		Application.LoadLevel("sceneMain");
		//Application.LoadLevel("loading");
	}

	public void onMusic()
	{
		//点击音乐。。。
		music.volume = 0.0F;
		
	}
}
