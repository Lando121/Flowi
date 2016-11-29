using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector3 mousePos = Input.GetTouch(0).position;
            mousePos.z = 10f;
            this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        GameObject.Find("MainCamera").GetComponent<GameLoop>().increaseHitPercentage(25f);
    }
}
