using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadMenuScript : MonoBehaviour {
	GameObject restartButton;
	GameObject menuButton;
	GameObject scoreText; 
	// Use this for initialization
	// Update is called once per frame
	public void init(){
		restartButton = GameObject.Find ("RestartButton");
		menuButton = GameObject.Find ("MenuButton");
		scoreText = GameObject.Find ("ScoreText");
		this.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width * 0.45f, Screen.width * 0.45f);
		menuButton.GetComponent<RectTransform>().sizeDelta = new Vector2 (Screen.width * 0.1f, Screen.width * 0.05f);
		restartButton.GetComponent<RectTransform>().sizeDelta = new Vector2 (Screen.width * 0.1f, Screen.width * 0.1f);
		menuButton.transform.localPosition = new Vector3 (Screen.width / 12f, 0, -1f);
		restartButton.transform.localPosition = new Vector3 (-(Screen.width / 12f), 0, -1f);
		//scoreText.transform.localPosition = new Vector3(0f, Screen.height/17f, -1f);
	}
		
	public void destroy (){
		Destroy(this.gameObject);
	}

	public void setScore (float score){
		scoreText.GetComponent<Text> ().text = "Score: " + score.ToString ("0.0");
	}
}
