using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LineGenerator : MonoBehaviour {

   // private LineRenderer line;
    public int numberOfPoints = 10;
    public float startLineLength = 10.0f;
    public Camera mainCamera;
    //private Vector3[] linePositions;
    public float xMargin = 0.5f;
    public float scrollSpeed = 0.1f;
    public float yStep = 0.5f;
    public float interpolationSmoothness = 1.0f;
    

    private float screenLeftPos, screenRightPos, screenTop, screenBottom;

    private List<KeyValuePair<LineRenderer,Vector3>> lines = new List<KeyValuePair<LineRenderer, Vector3>>();
    private Dictionary<Vector3, Vector3[]> linePositions = new Dictionary<Vector3, Vector3[]>();

	// Use this for initialization
	void Start () {
        //linePositions = new Vector3[numberOfPoints];
        //line = GetComponent<LineRenderer>();
        //line.SetVertexCount(numberOfPoints);

        Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        screenLeftPos = lowerLeft.x;
        screenRightPos = upperRight.x;
        screenTop = upperRight.y;
        screenBottom = lowerLeft.y;

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
        //line.transform.Translate(0, -scrollSpeed, 0
    //    scrollSpeed = Mathf.Clamp(scrollSpeed+0.00001f,0,1);
        updateLines();
    }

    /// <summary>
    /// Generates a new line and adds it to the list of currently active lines.
    /// </summary>
    /// <param name="points"> Positions that the line passes through in order. </param>
    /// <param name="position"> Transform position of the new line. </param>
    private void generateLine(Vector3[] points, Vector3 position) {
        GameObject lineObject = new GameObject();
        LineRenderer newLine = lineObject.AddComponent<LineRenderer>();
        newLine.SetVertexCount(points.Length);
        newLine.SetPositions(points);
        newLine.useWorldSpace = false;
        lineObject.transform.position = position;
        Vector3 lastPoint = points[points.Length - 1];
        // Store the line and its last point to easily determine when its last point is no longer visible.
        lines.Add(new KeyValuePair<LineRenderer, Vector3>(newLine, lastPoint));
        linePositions.Add(lastPoint, points);
    }

    /// <summary>
    /// Generates an inverse U-shaped line.
    /// </summary>
    /// <param name="height">Height of the turn.</param>
    /// <param name="width">Width of the turn.</param>
    /// <param name="previousPoint">Point that the line starts from.</param>
    /// <returns>The generated line.</returns>
    private Vector3[] uLine(float height, float width, Vector3 previousPoint) {
        Vector3 oppositePoint = previousPoint;
        int direction = (previousPoint.x > 0) ? -1 : 1;
        float distToEdge = (direction == 1) ? Mathf.Abs(screenRightPos - previousPoint.x - xMargin) : Mathf.Abs(previousPoint.x - (xMargin + screenLeftPos));
        width = Mathf.Min(distToEdge, width);
        oppositePoint.x = oppositePoint.x + width * 0.7f * direction;
        Vector3 middlePoint = new Vector3((oppositePoint.x + previousPoint.x) / 2, previousPoint.y + height, 0);
        Vector3 turnPoint = oppositePoint + new Vector3(direction * width * 0.15f, -5, 0);
        Vector3 endPoint = turnPoint + new Vector3(direction * width * 0.15f, 20, 0);
        Vector3[] line = new Vector3[5] { previousPoint, middlePoint, oppositePoint, turnPoint, endPoint};
        return smoothInterpolate(line, interpolationSmoothness);
    }

    /// <summary>
    /// Generates a randomly shaped line going upwards.
    /// </summary>
    /// <param name="points">Amount of points the line should contain.</param>
    /// <param name="previousPoint">Point that the line starts from.</param>
    /// <returns>The generated line.</returns>
    private Vector3[] randomLine(int points, Vector3 previousPoint) {
        if (points <= 1)
            return null;
        Vector3 transition = new Vector3(0, 8, 0);
        Vector3[] newLine = new Vector3[points];
        newLine[0] = previousPoint;
        newLine[1] = previousPoint + transition;

        for (int i = 2; i < points-1; ++i) {
            newLine[i] = generateNewPoint(newLine[i - 1]);
        }
        newLine[points - 1] = newLine[points - 2] + transition;
        return smoothInterpolate(newLine,interpolationSmoothness);
    }

    /// <summary>
    /// Iterates over all lines to move them and remove them if outside visible boundary.
    /// </summary>
    private void updateLines() {
        /*     if (linePositions[1].y + transform.position.y < bottom -1) {
                 for (int i = 1; i < numberOfPoints; ++i) {
                     linePositions[i - 1] = linePositions[i];
                 }
                 linePositions[numberOfPoints - 1] = generateNewPoint(linePositions[numberOfPoints - 2]);
                 Vector3[] smoothLine = smoothInterpolate(linePositions, interpolationSmoothness);
                 line.SetVertexCount(smoothLine.Length);
                 line.SetPositions(smoothLine);
             }*/
        // Move all lines 
        foreach (KeyValuePair<LineRenderer, Vector3> line in lines) {
            line.Key.transform.Translate(0, -scrollSpeed, 0);
        }
        // Check if any line is outside visible boundary
        foreach (KeyValuePair<LineRenderer, Vector3> line in lines) {
            GameObject lineObject = line.Key.gameObject;
            // If below screen bottom
            if (line.Value.y + lineObject.transform.position.y < screenBottom) {
                // Spawn new line
                generateLine(spawnNewLine(), lineObject.transform.position);
                // Remove old line
                lines.Remove(line);
                linePositions.Remove(line.Value);
                Destroy(lineObject);
                // Break since we can not continue iteration after altering the collection.
                break;
            }
        }
    }

    /// <summary>
    /// Generates a new line and chooses the type of line it should be.
    /// </summary>
    /// <returns>The generated line.</returns>
    private Vector3[] spawnNewLine() {
        float roll = Random.Range(0, 100);
        if (roll < 60) {
            return randomLine(numberOfPoints, lines[lines.Count - 1].Value);
        }
        else {
            return uLine(20,20, lines[lines.Count - 1].Value);
        }
    }

    /// <summary>
    /// Generates a point above a previous one, with a random x-value.
    /// </summary>
    /// <param name="previousPoint">Previous point to step from.</param>
    /// <returns>The new point.</returns>
    private Vector3 generateNewPoint(Vector3 previousPoint) {
        float yIncrease = yStep;
        float xPos = Random.Range(xMargin - screenRightPos, screenRightPos- xMargin);
        //float xPos = Mathf.Clamp(3.0f*Mathf.Sin(previousPoint.y), xMargin - screenWidth, screenWidth - xMargin);
        return new Vector3(xPos, previousPoint.y + yIncrease, 0);
    }

    /// <summary>
    /// Interpolates across a set of points, creating a smooth curved line between them. 
    /// </summary>
    /// <param name="arrayToCurve">Set to interpolate.</param>
    /// <param name="smoothness">Smoothness factor.</param>
    /// <returns>The interpolated positions.</returns>
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

    /// <summary>
    /// Calculates the distance to the line.
    /// </summary>
    /// <param name="position">Position in pixel-space.</param>
    /// <returns>Distance to line.</returns>
    public float distanceToLine(Vector2 position) {
        return distanceToLine(mainCamera.ScreenToWorldPoint(new Vector3(position.x,position.y,mainCamera.nearClipPlane)));
    }

    /// <summary>
    /// Calculates distance to the line.
    /// </summary>
    /// <param name="position">Position in world-space.</param>
    /// <returns>Distance to the line.</returns>
    public float distanceToLine(Vector3 position) {
        position.z = -1;
        float minDist = float.MaxValue;
        foreach (KeyValuePair<LineRenderer, Vector3> line in lines) {
            Vector3[] points = linePositions[line.Value];
            Vector3 offset = line.Key.gameObject.transform.position;
            for (int i = 0; i < points.Length; ++i) {
                minDist = Mathf.Min(minDist, Mathf.Abs((position - (points[i] + offset)).magnitude));
            }
        }
        return minDist;
    }

}
