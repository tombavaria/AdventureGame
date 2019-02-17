using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneScript : MonoBehaviour
{
    public GameObject guiObject;
    public string levelToLoad;

    void Start ()
    {
        guiObject.SetActive(false);
    }

    void OnTriggerStay (Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            guiObject.SetActive(true); 
            if (guiObject.activeInHierarchy == true && Input.GetButtonDown("Submit"))
            {
                Application.LoadLevel(levelToLoad);
            }
        }
    }
    void OnTriggerExit()
    {
        guiObject.SetActive(false);
    }
}
