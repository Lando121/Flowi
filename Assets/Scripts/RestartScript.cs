using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject btn = GameObject.Find("Restart_button");
        Button restart = btn.GetComponent<Button>();
        restart.onClick.AddListener(changeToScene);
	}

    // Update is called once per frame
    public void changeToScene()
    {
        Debug.Log("EOWEO?WPEOWPEOW");
        SceneManager.LoadScene("main_menu");
    }


}
