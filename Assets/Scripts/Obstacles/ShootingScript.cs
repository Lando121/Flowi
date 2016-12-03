using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.name == "PlayerIcon")
        {
            GameObject.Find("MainCamera").GetComponent<GameLoop>().increaseHitPercentage(10f);
        }
    }
}
