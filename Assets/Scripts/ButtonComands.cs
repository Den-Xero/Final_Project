using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonComands : MonoBehaviour
{
    public void MoveToScene(int SceneIndex)
    {
        //When the button has been pressed it will move to the scene index that is specified
        SceneManager.LoadScene(SceneIndex);
    }

    public void Exit()
    {
        //Closes the game on the button click.
        Application.Quit();
    }
}
