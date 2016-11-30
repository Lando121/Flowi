using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinScript : MonoBehaviour
{
    public GameObject coin;

    public void generateCoins(Vector3[] points, Line mainLine) {
        int spawnChance = (int) mainLine.lineDifficulty * 10;
        float width = mainLine.lineWidth * 0.15f;
        

        foreach (Vector3 p in points) {
            Vector3 temp = p + mainLine.transform.position;
            if (Random.Range(1, 100) <= spawnChance)
            {
               GameObject c =  Instantiate(coin, temp, Quaternion.identity);
               c.transform.parent = mainLine.transform;
                c.transform.localScale = new Vector3(width, width, width);
                c.transform.localPosition = p + new Vector3(0, 0, -2);
            }
        }
    }
}
