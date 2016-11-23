﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameLoop : MonoBehaviour {
    public static bool playing = false;
    public GameObject lineObject;
    private LineGenerator line;
    public float multiplier = 0;
    public float multiplierTime = 0;
    private float tmpTime = 0;
    public Text multiplierText;
    public Text hitPercentageText;
    private float hitPercentage = 0;
    private float timeOutsideLine = 0;
    // Use this for initialization
    void Start() {

        multiplierText.text = "Multiplier: ";
        hitPercentageText.text = "";
        line = lineObject.GetComponent<LineGenerator>();
    }

    // Update is called once per frame
    void Update() {
        Vector2 mousePos = Input.mousePosition;
        float distanceToLine;
        distanceToLine = line.distanceToLine(mousePos);
        //multiplierText.text = Input.GetTouch(0).position.ToString();
        //multiplierText.text = "Multiplier: " + multiplier.ToString("0.0");
        hitPercentageText.text = "MissPercentage: " + hitPercentage.ToString("0.00");
        if (Input.touchCount == 0) {
            pauseGame();
        }

        if (playing) {

            distanceToLine = line.distanceToLine(Input.GetTouch(0).position);
            if (distanceToLine < line.lineThickness / 2) {
                multiplierTime += Time.deltaTime;
                timeOutsideLine = 0;
                if ((int)Mathf.Floor(multiplierTime) != tmpTime) {
                    tmpTime++;
                    multiplier += 0.5f;
                }
            }
            else {
                hitPercentage += Time.deltaTime;
                timeOutsideLine += Time.deltaTime;
                if (timeOutsideLine > 0.2) {
                    multiplier -= 0.2f;
                    if (multiplier < 0) {
                        multiplier = 0;
                    }
                    timeOutsideLine = 0;
                }

                tmpTime = 0;
                multiplierTime = 0;

            }


        }
        else {
            if (Input.touchCount == 1) {
                resumeGame();
            }
        }


    }
    public void pauseGame() {
        playing = false;
    }

    public void resumeGame() {
        playing = true;
    }
}
