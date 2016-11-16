using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0) {
            Vector3 mousePos = Input.GetTouch(0).position;
            mousePos.z = 10f;
            this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
}
