using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.game;
using System;

public class DeadMenuScript : MonoBehaviour {
	GameObject restartButton;
	GameObject menuButton;
	GameObject scoreText;

    public GameObject submitText;

    public InputField inputNameField;

	// Use this for initialization
	// Update is called once per frame
	public void init(){
		/*restartButton = GameObject.Find ("RestartButton");
		menuButton = GameObject.Find ("MenuButton");*/
		scoreText = GameObject.Find ("DeadScoreText");
        /*
		this.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width * 0.45f, Screen.width * 0.45f);
		menuButton.GetComponent<RectTransform>().sizeDelta = new Vector2 (Screen.width * 0.1f, Screen.width * 0.05f);
		restartButton.GetComponent<RectTransform>().sizeDelta = new Vector2 (Screen.width * 0.1f, Screen.width * 0.1f);
		menuButton.transform.localPosition = new Vector3 (Screen.width / 12f, 0, -1f);
        submitText.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.15f, Screen.width * 0.15f);
		restartButton.transform.localPosition = new Vector3 (-(Screen.width / 12f), 0, -1f);
		scoreText.transform.localPosition = new Vector3(0f, Screen.height/17f, -1f);*/

    }

    public void uploadToDatabase() {
        Debug.Log("Uploading");
        App42API.Initialize("2694dfa93450b0de104aec48c0576578eb6885987b9f229510314c2e36ebe3ee", "60b3d72b40907bb63f5880ede93f868e6a978270d9d11b7fd406c54fe3a515e1");
        string userName = inputNameField.text;
        if (userName == "") {
            userName = "Anonymous";
        }

        ScoreBoardService scoreBoardService = App42API.BuildScoreBoardService();

        string gameName = "Flowi";
        float gameScore = Camera.main.GetComponent<GameLoop>().score;
        scoreBoardService.SaveUserScore(gameName, userName, gameScore, new UnityCallBack());
        GameObject.Find("GameManager").GetComponent<GameSceneManager>().cancelSubmit();
    }

    public class UnityCallBack : App42CallBack {
        public void OnSuccess(object response) {
            Game game = (Game)response;
            App42Log.Console("gameName is " + game.GetName());
            for (int i = 0; i < game.GetScoreList().Count; i++) {
                App42Log.Console("userName is : " + game.GetScoreList()[i].GetUserName());
                App42Log.Console("score is : " + game.GetScoreList()[i].GetValue());
                App42Log.Console("scoreId is : " + game.GetScoreList()[i].GetScoreId());
            }
        }
        public void OnException(Exception e) {
            App42Log.Console("Exception : " + e);
        }
    }


		
	public void destroy (){
		Destroy(this.gameObject);
	}

	public void setScore (float score){
		scoreText.GetComponent<Text> ().text = "Score: " + score.ToString ("0.0");
	}



}
