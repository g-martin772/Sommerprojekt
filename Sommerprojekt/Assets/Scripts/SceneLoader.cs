using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public void QuitGame()
	{
		Application.Quit();
		Debug.Log("GameQuit");
	}

	public void showSettings()
	{

	}

	public void Scene1()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(1);
	}

	public void MainMenu()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(0);
	}
}
