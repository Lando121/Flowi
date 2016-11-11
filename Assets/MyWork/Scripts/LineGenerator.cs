using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineGenerator : MonoBehaviour {

   // private LineRenderer line;
    public int numberOfPoints = 10;
    public float startLineLength = 10.0f;
    public Camera mainCamera;
    private Vector3[] linePositions;
    public float xMargin = 0.5f;
    public float scrollSpeed = 0.1f;
    public float yStep = 0.5f;
    public float interpolationSmoothness = 1.0f;
    
    private List<KeyValuePair<LineRenderer,Vector3>> lines = new List<KeyValuePair<LineRenderer, Vector3>>();

	// Use this for initialization
	void Start () {
        linePositions = new Vector3[numberOfPoints];
        //line = GetComponent<LineRenderer>();
        //line.SetVertexCount(numberOfPoints);

        float bottom = mainCamera.ScreenToWorldPoint(new Vector3(0,0,0)).y;

        Vector3[] startLine = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; ++i) {
            startLine[i] = new Vector3(0, bottom + (startLineLength/numberOfPoints) * i, 0);
        }
        Vector3[] smoothStart = smoothInterpolate(startLine, interpolationSmoothness);
        generateLine(smoothStart,new Vector3(0,0,-1));
        generateLine(randomLine(numberOfPoints, smoothStart[smoothStart.Length - 1]), new Vector3(0, 0, -1));
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //line.transform.Translate(0, -scrollSpeed, 0);
        scrollSpeed = Mathf.Clamp(scrollSpeed+0.00001f,0,1);
        updateNodes();
	}

    private void generateLine(Vector3[] points, Vector3 position) {
        GameObject lel = new GameObject();
        LineRenderer newLine = lel.AddComponent<LineRenderer>();
        newLine.SetVertexCount(points.Length);
        newLine.SetPositions(points);
        newLine.useWorldSpace = false;
        lel.transform.position = position;
        lines.Add(new KeyValuePair<LineRenderer, Vector3>(newLine, points[points.Length - 1]));
    }

    private Vector3[] randomLine(int points, Vector3 previousPoint) {
        if (points <= 1)
            return null;
        Vector3[] newLine = new Vector3[points];
        newLine[0] = previousPoint;

        for (int i = 1; i < points; ++i) {
            newLine[i] = generateNewPoint(newLine[i - 1]);
        }
        return smoothInterpolate(newLine,interpolationSmoothness);
    }

    private void updateNodes() {
        float bottom = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        /*     if (linePositions[1].y + transform.position.y < bottom -1) {
                 for (int i = 1; i < numberOfPoints; ++i) {
                     linePositions[i - 1] = linePositions[i];
                 }
                 linePositions[numberOfPoints - 1] = generateNewPoint(linePositions[numberOfPoints - 2]);
                 Vector3[] smoothLine = smoothInterpolate(linePositions, interpolationSmoothness);
                 line.SetVertexCount(smoothLine.Length);
                 line.SetPositions(smoothLine);
             }*/
        foreach (KeyValuePair<LineRenderer, Vector3> line in lines) {
            line.Key.transform.Translate(0, -scrollSpeed, 0);
        }
       foreach (KeyValuePair<LineRenderer, Vector3> line in lines) {
            //print("Bottom: " + bottom);
            GameObject lineObject = line.Key.gameObject;
            if (line.Value.y + lineObject.transform.position.y < bottom) {
                generateLine(randomLine(numberOfPoints, lines[lines.Count - 1].Value), lineObject.transform.position);
                lines.Remove(line);
                Destroy(lineObject);
                break;
            }
        }
    }

    private Vector3 generateNewPoint(Vector3 previousPoint) {
        float screenWidth = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        float yIncrease = yStep;
        float xPos = Random.Range(xMargin - screenWidth, screenWidth - xMargin);
        //float xPos = Mathf.Clamp(3.0f*Mathf.Sin(previousPoint.y), xMargin - screenWidth, screenWidth - xMargin);
        return new Vector3(xPos, previousPoint.y + yIncrease, 0);
    }

    private Vector3[] smoothInterpolate(Vector3[] arrayToCurve, float smoothness) {
        int numberOfPoints = arrayToCurve.Length;
        int curvedLength = numberOfPoints * Mathf.RoundToInt(smoothness) - 1;
        List<Vector3> points;
        List<Vector3> curvedPoints = new List<Vector3>(curvedLength);

        smoothness = Mathf.Min(1.0f, smoothness);

        float t = 0f;
        for (int step = 0; step < curvedLength+1; ++step) {
            t = Mathf.InverseLerp(0, curvedLength, step);

            points = new List<Vector3>(arrayToCurve);

            for (int j = numberOfPoints-1; j > 0; j--) {
                for (int i = 0; i < j; i++) {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }
            curvedPoints.Add(points[0]);
        }

        return curvedPoints.ToArray();
    }

}
