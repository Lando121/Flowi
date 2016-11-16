using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

    public float monsterSpeed = 2.0f;
    private float monsterStartx;
    private float monsterStopx;

    // Use this for initialization
    void Start() {


    }

    public void initiateMonster(Vector3 position, float scale, float start, float stop) {
        transform.localScale *= scale;
        transform.position = position;
        monsterStartx = start;
        monsterStopx = stop;
    }

    // Update is called once per frame
    void Update() {
        if (transform.position.x >= monsterStopx && monsterSpeed > 0) {
            monsterSpeed = -monsterSpeed;

        }
        else if (transform.position.x <= monsterStartx && monsterSpeed < 0) {
            monsterSpeed = -monsterSpeed;
        }
        transform.Translate(Vector2.right * monsterSpeed * Time.deltaTime, Space.World);
    }

}
