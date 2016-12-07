﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;


public class GameLoop : MonoBehaviour {
    public static bool playing = false;
    private bool resumingGame = false;
    public GameObject lineObject;
    private LineGenerator line;
    public float multiplier = 0;
    public float multiplierTime = 0;
    private float tmpLineTime = 0;
    public Text multiplierText;
    public Text scoreText;
    public float score;
    public Text countDownText;
    private float hitPercentage = 0;
    private float timeOutsideLine = 0;
    private Touch lastKnownTouch;
    private int resumeCounter = 3;
    private float tmpCounterTime = 0;
    private int hpDecreaseMultiplier = 15;
    private TutScript notificationScript;
    private GameObject deadMenu;
    private bool gameHasStarted = false;
    private bool gameOver = false;

    private float distanceToLine;
    // Use this for initialization
    void Start() {
        Physics2D.gravity = Vector2.zero;
        multiplierText.text = "Multiplier: ";
		deadMenu = GameObject.Find ("DeadMenu");
		deadMenu.GetComponent<DeadMenuScript>().init();
		deadMenu.SetActive(false);
        line = lineObject.GetComponent<LineGenerator>();
        notificationScript = GameObject.Find("NotificationText").GetComponent<TutScript>();
        notificationScript.displayText("Place your finger on the circle!", 60);
        lastKnownTouch.position = new Vector2(Screen.width / 2, Screen.height / 4);
        GameObject.Find("BackgroundMusic").GetComponent<BackgroundMusicPlayer>().playMusic();
    }

    // Update is called once per frame
    void Update() {

        scoreText.text = "Score: " + score.ToString("0.0");
        if (hitPercentage > 100) {
            // GameObject.Find("GameManager").GetComponent<GameSceneManager>().changeToScene("main_menu");
            //GameObject.Find("NotifcationText").GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.8f, Screen.width * 0.8f);

            // notificationScript.enable();
            // notificationScript.displayText("You Lost you worthless piece of shit, but you score is " + score.ToString("0.0"), 50);
            GameObject.Find("HP_bar").GetComponent<PositionOfHpBar>().updateHPLine(hitPercentage);
            gameOver = true;
            playing = false;
			deadMenu.SetActive(true);
			deadMenu.GetComponent<DeadMenuScript> ().setScore (score);
          //  GameObject.Find("DeadMenu").GetComponent<DeadMenuScript>().init();

        }

        if (Input.touchCount == 0) {
            resumingGame = false;
            countDownText.enabled = false;
            pauseGame();

        }
        if (playing) {

            score += Time.deltaTime;
            hittingLine();
            GameObject.Find("HP_bar").GetComponent<PositionOfHpBar>().updateHPLine(hitPercentage);
            lastKnownTouch = Input.GetTouch(0);
        }
        else if (!gameOver) {


            if (Input.touchCount == 1 && !resumingGame) {
                if ((Input.GetTouch(0).position - lastKnownTouch.position).magnitude < 200) {
                    notificationScript.disable(); //Disable   tutorial text
                    resumingGame = true;
                    tmpCounterTime = Time.realtimeSinceStartup;
                    countDownText.enabled = true;
                    resumeGame();
                }

            }
            else {
                if (Input.touchCount == 1) {
                    if ((Input.GetTouch(0).position - lastKnownTouch.position).magnitude > 200) {
                        resumingGame = false;
                        countDownText.enabled = false;
                    }
                    resumeGame();
                }
            }
        }


    }

    public void increaseHitPercentage(float value) {
        hitPercentage += value;
    }

    public void addScore(float value) {
        score += value * 1 + multiplier;
    }

    private void hittingLine() {
        //multiplierText.text = Input.GetTouch(0).position.ToString();
        multiplierText.text = "Multiplier: " + multiplier.ToString("0.0");


        distanceToLine = line.distanceToLine(Input.GetTouch(0).position);
        if (distanceToLine <= 0) {
            multiplierTime += Time.deltaTime;
            timeOutsideLine = 0;
            if ((int)Mathf.Floor(multiplierTime) != tmpLineTime) {
                tmpLineTime++;
                multiplier += 0.5f;
            }
        }
        else {
            hitPercentage += Time.deltaTime * hpDecreaseMultiplier;
            timeOutsideLine += Time.deltaTime;
            if (timeOutsideLine > 0.2) {
                multiplier -= 0.2f;
                if (multiplier < 0) {
                    multiplier = 0;
                }
                timeOutsideLine = 0;
            }

            tmpLineTime = 0;
            multiplierTime = 0;
        }
    }

    public void pauseGame() {
        playing = false;

    }

    public void setStartingPoint(Vector2 pos) {
        lastKnownTouch.position = pos;
    }

    public void resumeGame() {
        if (Time.realtimeSinceStartup - tmpCounterTime > resumeCounter) {
            resumingGame = false;
            playing = true;
            countDownText.enabled = false;
        }
        Vector2 distToResumePoint = Input.GetTouch(0).position - lastKnownTouch.position;
        //countDownText.text = distToResumePoint.magnitude.ToString();
        countDownText.text = (resumeCounter - Mathf.FloorToInt(Time.realtimeSinceStartup - tmpCounterTime)).ToString();


    }
}