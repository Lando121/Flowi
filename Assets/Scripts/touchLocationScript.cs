using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class touchLocationScript : MonoBehaviour {

    float x;
    float y;
    float marginal = 50;
    Vector3 speed = new Vector3( 0, 1, 0 );
    public Text healthText;
    float health = 10f;
    Vector3 squarePos;
    float time;
    int tmpTime;
    bool playing = true;

    public GameObject lineObject;
    private LineGenerator line;

	// Use this for initialization
	void Start () {
        line = lineObject.GetComponent<LineGenerator>();
        tmpTime = 0;
        time = 0f;
        x = gameObject.transform.position.x;
        y = gameObject.transform.position.y;
       
    }
	
	// Update is called once per frame
	void Update () {

        squarePos = Camera.main.WorldToScreenPoint(transform.position);
        //time = time + Time.deltaTime;
        

        /*
        if(health > 0)
        {
            healthText.text = health.ToString("0.0");
        } else
        {
            healthText.text = "You survived for: " + time.ToString("0");
            playing = false;
        }*/

        if ((int)Mathf.Floor(time) != tmpTime)
        {
            tmpTime++;
            increaseSpeed();
        }

        Vector2 lel = Input.mousePosition;
        healthText.text = "Dist: " + line.distanceToLine(lel).ToString();

        if (Input.touchCount > 0 && playing)
        {
            time = time + Time.deltaTime;
            Vector3 touchPos = Input.GetTouch(0).position;
            float touchX = touchPos.x;
            float touchY = touchPos.y;

            healthText.text = "Dist: " + line.distanceToLine(Input.GetTouch(0).position).ToString();

            if (Mathf.Max(Mathf.Abs(touchX - squarePos.x), Mathf.Abs(touchY - squarePos.y)) > 100)
            {
                health = health - 1 * Time.deltaTime;
            }

            if (squarePos.y > Screen.height - marginal || squarePos.y < marginal)
            {
                speed = -speed;
                moveSquare();
            } else
            {
                moveSquare();
            }
                     
        }
    }


    void increaseSpeed()
    {
        speed = speed*1.2f;
    }
    
    void moveSquare()
    {
        gameObject.transform.Translate(speed * Time.deltaTime);
    }
}
