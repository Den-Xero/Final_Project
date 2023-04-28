using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonComands : MonoBehaviour
{
    public void MoveToScene(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
