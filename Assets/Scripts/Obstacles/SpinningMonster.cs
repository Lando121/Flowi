using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningMonster : MonoBehaviour {
    
    private float angleRotation;


    public float rotationSpeed;

	// Use this for initialization
	void Start () {
        transform.Rotate(0, 0, Random.Range(0, 360));
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GameLoop.playing) {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
	}
    
    public void setRotationRadius(float radius) {
        transform.GetChild(0).localPosition = new Vector3(radius,0,0);
    }

}
