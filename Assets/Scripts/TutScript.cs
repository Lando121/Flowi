using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutScript : MonoBehaviour {
	private string textToDispaly;
	public Text notifactionText;
	// Use this for initialization
	// Update is called once per frame
	void Update () {
		
	}

	public void displayText(string text, int fontSize){
		notifactionText = this.GetComponent<Text> ();
		textToDispaly = text;
		notifactionText.fontSize = fontSize;
		notifactionText.text = text;
		
	}
	public void disable(){
		notifactionText.enabled = false;
	}

    public void enable()
    {
        notifactionText.enabled = true;
    }
}