using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class touchLocationScript : MonoBehaviour {

    float x;
    float y;
    float marginal = 50;
    Vector3 speed = new Vector3( 0, 1, 0 );
    public Text infoText;
   

	// Use this for initialization
	void Start () {
        x = gameObject.transform.position.x;
        y = gameObject.transform.position.y;
       
    }
	
	// Update is called once per frame
	void Update () {
       
        Vector3 tmpPos = Camera.main.WorldToScreenPoint(transform.position);
        infoText.text = tmpPos.ToString();

        if (Input.touchCount > 0)
        {
            if (tmpPos.y > Screen.height - marginal || tmpPos.y < marginal)
            {
                speed = -speed;
                gameObject.transform.Translate(speed * Time.deltaTime);
            } else
            {
                gameObject.transform.Translate(speed * Time.deltaTime);
            }
            

            //Debug.Log(topRight);

          
        }
    }
}
