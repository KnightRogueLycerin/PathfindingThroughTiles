using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {
    public void quit()
    {
        Debug.Log("Exiting the application");
        Application.Quit();
        // If run in editor above wont work
        // UnityEditor.EditorApplication.isPlaying = false;
    }
    public void openLevel(string name)
    {
        Debug.Log("Loading " + name + " level");
        SceneManager.LoadScene(name);
    }
    public void restart()
    {
        Debug.Log("Restarting");
        openLevel(SceneManager.GetActiveScene().name);
    }
}
