using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterStage : MonoBehaviour {

    public LoadTrigger loadTrigger;
    

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if(player != null)
        {
            // Reset all the effects on the player
            player.CleanElements();
            // Set the how many pick ups are
            player.numPickUps = FindObjectsOfType<Rotator>().Length; 
        }

        // Set up the entrance wall
        FindObjectOfType<RemoveEntranceWall>().gameObject.GetComponent<Renderer>().enabled = true;
        FindObjectOfType<RemoveEntranceWall>().gameObject.GetComponent<BoxCollider>().enabled = true;
        FindObjectOfType<RemoveEntranceWall>().gameObject.GetComponent<BoxCollider>().isTrigger = false;


        Destroy(this);
       

    }

}
