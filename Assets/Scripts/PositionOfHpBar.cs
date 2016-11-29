using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionOfHpBar : MonoBehaviour
{
    Sprite sp;
    // private GameObject HP_line;
    //private LineRenderer lr;
    Vector3 topLeft;
    GameObject myLine;
    LineRenderer lr;
    float lengthOfFullLine;

    //93 % ska linjen vara

    // Use this for initialization
    void Start()
    {
        topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0f));
        // HP_line = new GameObject();
        //HP_line.transform.position = topLeft;
        //HP_line.AddComponent<LineRenderer>();
        //lr = HP_line.GetComponent<LineRenderer>();

        sp = GetComponent<SpriteRenderer>().sprite;
        DrawHPLine();
        //  Debug.Log(sp.ToString());
        Vector3[] spriteBounds = SpriteLocalToWorld(sp);
        float scale = scaleHPBar(spriteBounds);
        // set the scaling

        transform.localScale = new Vector3(scale, scale, scale);


        // set the position
        transform.position = new Vector3(topLeft.x, topLeft.y, -5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    float scaleHPBar(Vector3[] array)
    {
        float xMin = array[0].x;
        float xMax = array[1].x;
        Vector3 tmp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
        float scale = tmp.x / ((xMax - xMin) / 2);
        return scale;
    }

    Vector3[] SpriteLocalToWorld(Sprite sp)
    {
        Vector3 pos = transform.position;
        Vector3[] array = new Vector3[2];
        //top left
        array[0] = pos + sp.bounds.min;
        // Bottom right
        array[1] = pos + sp.bounds.max;

        return array;
    }

    public void DrawHPLine()
    {
        GameObject.Destroy(myLine);
        myLine = new GameObject();
        myLine.transform.position = new Vector3(0, 0, -5);
        myLine.AddComponent<LineRenderer>();
        lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.startWidth = sp.rect.size.y / 150;
        lr.endWidth = sp.rect.size.y / 150;
        Vector3 tmpStart = topLeft;
        tmpStart.y = topLeft.y - lr.startWidth / 2;
        tmpStart.z = -4f;
        Vector3 tmpEnd = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -5f));
        tmpEnd.z = -4f;

        tmpEnd.y = tmpStart.y;

        //93 % of the screenWidth
        lengthOfFullLine = (tmpEnd.x - tmpStart.x) * 0.93f;
        tmpStart.x = tmpStart.x + (0.035f * lengthOfFullLine);
        tmpEnd.x = tmpStart.x + lengthOfFullLine;
        Debug.Log(lengthOfFullLine);
        lr.SetPosition(0, tmpStart);
        lr.SetPosition(1, tmpEnd);
    }

    public void updateHPLine(float missPercentage)
    {
        // Vector3 lastPos = lr.GetPosition(1);
        Vector3 newPos = lr.GetPosition(0);
        newPos.x = newPos.x + (lengthOfFullLine * (1 - missPercentage / 100));
        lr.SetPosition(1, newPos);
    }

    public void destroyHPLine()
    {
        GameObject.Destroy(myLine);
    }

}
