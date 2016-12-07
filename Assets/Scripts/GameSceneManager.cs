using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour {

   public GameObject submitScreen;

   public void changeToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void showSubmitScreen() {
        submitScreen.SetActive(true);
    }

    public void hideSubmitScreen() {
        submitScreen.SetActive(false);
    }

}
