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
    public Text hitPercentageText;
    public Text countDownText;
    private float hitPercentage = 0;
    private float timeOutsideLine = 0;
    private Touch lastKnownTouch;
    private int resumeCounter = 3;
    private float tmpCounterTime = 0;

    private float distanceToLine;
    // Use this for initialization
    void Start () {

        multiplierText.text = "Multiplier: ";
        hitPercentageText.text = "";
        line = lineObject.GetComponent<LineGenerator>();
    }
	
	// Update is called once per frame
	void Update () {
      
        if (Input.touchCount == 0)
        {
            resumingGame = false;
            countDownText.enabled = false;
            pauseGame();
           
        }      
        if (playing)
        {
            hittingLine();
            lastKnownTouch = Input.GetTouch(0);
        }
        else {
           if(Input.touchCount == 1 && !resumingGame)
            {
                resumingGame = true;
                tmpCounterTime = Time.realtimeSinceStartup;
                countDownText.enabled = true;
                resumeGame();
            } else
            {
                if(Input.touchCount == 1)
                {
                    resumeGame();
                }
            }
        }
      

    }

    private void hittingLine()
    {
        //multiplierText.text = Input.GetTouch(0).position.ToString();
        multiplierText.text = "Multiplier: " + multiplier.ToString("0.0");
        hitPercentageText.text = "MissPercentage: " + hitPercentage.ToString("0.00");

        distanceToLine = line.distanceToLine(Input.GetTouch(0).position);
        if (distanceToLine < line.lineThickness / 2)
        {
            multiplierTime += Time.deltaTime;
            timeOutsideLine = 0;
            if ((int)Mathf.Floor(multiplierTime) != tmpLineTime)
            {
                tmpLineTime++;
                multiplier += 0.5f;
            }
        }
        else
        {
            hitPercentage += Time.deltaTime;
            timeOutsideLine += Time.deltaTime;
            if (timeOutsideLine > 0.2)
            {
                multiplier -= 0.2f;
                if (multiplier < 0)
                {
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

    public void resumeGame()
    {
        if (Time.realtimeSinceStartup - tmpCounterTime > resumeCounter)
        {
            resumingGame = false;
            playing = true;
            countDownText.enabled = false;
        }
        Vector2 distToResumePoint = Input.GetTouch(0).position - lastKnownTouch.position;
        //countDownText.text = distToResumePoint.magnitude.ToString();
        countDownText.text = (resumeCounter - Mathf.FloorToInt(Time.realtimeSinceStartup - tmpCounterTime)).ToString();


    }
}