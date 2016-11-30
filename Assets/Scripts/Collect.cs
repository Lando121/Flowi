using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider)
    {
            if(collider.name == "PlayerIcon")
        {
            Camera.main.GetComponent<GameLoop>().addScore(100);
            Destroy(gameObject);
        }   
           
        
    }
}
