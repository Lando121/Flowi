using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.game;
using System;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour {

    public int numberOfRankings = 10;
    public GameObject rankingText;
    private string[] entries;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateRankingTexts(string[] entries) {
        for (int i = 0; i < entries.Length; i++) {
            GameObject newText = Instantiate(rankingText);
            newText.transform.parent = transform;
            Vector2 canvasSize = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;
            newText.GetComponent<RectTransform>().localPosition = new Vector2(0,canvasSize.y/2 - canvasSize.y/12*(i+1));
            newText.GetComponent<Text>().text = entries[i];
        }
    }

    public void fetchFromDatabase() {
        entries = new string[numberOfRankings];
        string gameName = "Flowi";
        int max = numberOfRankings;
        App42Log.SetDebug(true);        //Print output in your editor console  
        App42API.Initialize("2694dfa93450b0de104aec48c0576578eb6885987b9f229510314c2e36ebe3ee", "60b3d72b40907bb63f5880ede93f868e6a978270d9d11b7fd406c54fe3a515e1");
        ScoreBoardService scoreBoardService = App42API.BuildScoreBoardService();
        scoreBoardService.GetTopNRankings(gameName, max, new UnityCallBack());
    }

    public class UnityCallBack : App42CallBack {
        public void OnSuccess(object response) {
            Game game = (Game)response;
            App42Log.Console("gameName is " + game.GetName());
            string[] entries = new string[game.GetScoreList().Count];
            for (int i = 0; i < game.GetScoreList().Count; i++) {
                entries[i] = (i + 1) + ".   " + game.GetScoreList()[i].GetUserName() + " - " + game.GetScoreList()[i].GetValue();
                App42Log.Console("userName is : " + game.GetScoreList()[i].GetUserName());
                App42Log.Console("score is : " + game.GetScoreList()[i].GetValue());
                App42Log.Console("scoreId is : " + game.GetScoreList()[i].GetScoreId());
            }
            GameObject.Find("Leaderboard").GetComponent<LeaderboardScript>().updateRankingTexts(entries);
        }

        public void OnException(Exception e) {
            App42Log.Console("Exception : " + e);
        }
    }
}

