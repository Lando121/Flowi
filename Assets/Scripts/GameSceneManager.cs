using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameSceneManager : MonoBehaviour {

    public GameObject submitScreen;
    public GameObject submitButton;
    public GameObject leaderboard;


   public void changeToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void showSubmitScreen() {
        submitScreen.SetActive(true);
    }

    public void cancelSubmit() {
        submitButton.SetActive(false);
        hideSubmitScreen();
    }

    public void hideSubmitScreen() {
        submitScreen.SetActive(false);
    }

    public void showLeaderboard() {
        leaderboard.SetActive(true);
        leaderboard.GetComponent<LeaderboardScript>().fetchFromDatabase();
    }

    public void hideLeaderboard() {
        leaderboard.SetActive(false);
    }

}
