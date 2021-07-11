using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	public void play()
    {
        SceneManager.LoadScene("Level01", LoadSceneMode.Single);
    }

    public void quit()
    {
        Application.Quit();
    }
}
