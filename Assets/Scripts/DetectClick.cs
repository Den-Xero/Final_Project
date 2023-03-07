using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectClick : MonoBehaviour
{
    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Determine which object was clicked on
            if (hit.collider.gameObject == gameObject)
            {
                // Do something with the clicked object
                Debug.Log("Clicked on " + gameObject.name);
                Hex par = GetComponentInParent<Hex>();
                GameManager.Main.AStar.BeginSearch(par);
                GameManager.Main.AStar.Pathway = false;
                GameManager.Main.AStar.Incomplete = false;
                GameManager.Main.AStar.Done = false;
                GameManager.Main.PlayerArcher.StartFindingPath = false;
            }
        }
    }

}
