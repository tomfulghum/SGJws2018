using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(1);      //load Game Scene with index 1
        Debug.Log("Restart");
    }

}
