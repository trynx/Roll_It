using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTrigger : MonoBehaviour {

    public string loadName;
    public string unloadName;
    public bool isEntrance;

    private GameObject entrance;
    

    private void OnTriggerExit(Collider other)
    {
        if(loadName != "")
        {
            GameManager.Instance.Load(loadName);
        }

        if (unloadName != "")
        {
            StartCoroutine(UnloadScene());
        }
        
        if(isEntrance) RemoveWall();
    }

    IEnumerator UnloadScene()
    {
        yield return new WaitForSeconds(.1f);
        GameManager.Instance.Unload(unloadName);
    }

    void RemoveWall()
    {
        entrance = GameObject.FindGameObjectWithTag("Entrance");
        if(entrance != null)
        {
            entrance.GetComponent<Renderer>().enabled = false;
            entrance.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
