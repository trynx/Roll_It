using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCameraController : MonoBehaviour {

    public GameObject player;

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        if(player != null) transform.position = player.transform.position;
    }
}
