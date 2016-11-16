﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LineGenerator : MonoBehaviour {
    
    public Camera mainCamera;

    [Header("Line generation attributes")]
    public int numberOfPoints = 10;
    public float startLineLength = 10.0f;
    public float xMargin = 0.5f;
    public float yStep = 0.5f;
   //    public float interpolationSmoothness = 1.0f;
    public float pointRemovalOffset = 10.0f;
    public float generationOffset = 10.0f;
    public float lineThickness = 1.0f;

    [Header("Scrolling attributes")]
    public float scrollSpeed = 0.1f;
    public float acceleration = 0.005f;
    public float choiceSnapDistance = 3.0f;

    private int accelerationMult = 1;
    private bool accelerating = false;
    private float targetSpeed;

    public Material lineMaterial;

    private Vector3 nextForkPos;
    private float screenLeftPos, screenRightPos, screenTop, screenBottom;

    private List<KeyValuePair<LineRenderer,Vector3>> lines = new List<KeyValuePair<LineRenderer, Vector3>>();
    private Dictionary<GameObject, Vector3[]> linePositions = new Dictionary<GameObject, Vector3[]>();

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
        nextForkPos = Vector3.zero;

        float bottom = mainCamera.ScreenToWorldPoint(new Vector3(0,0,0)).y;

        Vector3 startPos = new Vector3(0, bottom, 0);

        LineRenderer initLine = newLine(new Vector3(0, 0, -1), startPos);
        lines[0] = new KeyValuePair<LineRenderer,Vector3>(initLine,generateLine(straightLine(startPos, startPos + new Vector3(0, 30, 0)), startPos, initLine));
    }

    // Update is called once per frame
    void FixedUpdate () {
        //line.transform.Translate(0, -scrollSpeed, 0
        //    scrollSpeed = Mathf.Clamp(scrollSpeed+0.00001f,0,1);
        foreach (GameObject v in linePositions.Keys) {
          //  print(linePositions.Keys.Count);
        }
        foreach(KeyValuePair<LineRenderer,Vector3> p in lines) {
            //print(p.Value);
        }
        updateLines();
        if (nextForkPos != Vector3.zero) {
            float dif = nextForkPos.y + lines[0].Key.gameObject.transform.position.y - screenTop;
            if (dif < -0.5f*(screenTop-screenBottom)) {
                accelerateTo(0.0f);
            }
        }
        if (accelerating) {
            accelerating = accelerate();
        }
    }

    private bool accelerate() {
        scrollSpeed += accelerationMult * acceleration;
        scrollSpeed = Mathf.Max(0, scrollSpeed);
        return !(scrollSpeed < targetSpeed && accelerationMult == -1 || scrollSpeed > targetSpeed && accelerationMult == 1);
    }

    private void accelerateTo(float speed) {
        accelerating = true;
        targetSpeed = speed;
        accelerationMult = (speed > scrollSpeed) ? 1 : -1;
    }

    private LineRenderer newLine(Vector3 position, Vector3 startPoint) {
        GameObject lineObject = new GameObject();
        LineRenderer newLine = lineObject.AddComponent<LineRenderer>();
        newLine.widthMultiplier = lineThickness;
        newLine.material = lineMaterial;
        Vector3[] points = new Vector3[] { startPoint };
        newLine.SetPositions(points);
        newLine.useWorldSpace = false;
        lineObject.transform.position = position;
        Vector3 lastPoint = points[points.Length - 1];
        // Store the line and its last point to easily determine when its last point is no longer visible.
        lines.Add(new KeyValuePair<LineRenderer, Vector3>(newLine, lastPoint));
        linePositions.Add(lineObject, points);
        return newLine;
    }

    /// <summary>
    /// Generates a new line and adds it to the list of currently active lines.
    /// </summary>
    /// <param name="points"> Positions that the line passes through in order. </param>
    /// <param name="from"> Transform position of the new line. </param>
    private Vector3 generateLine(Vector3[] points, Vector3 from, LineRenderer line, string tag = "Line") {
        /* GameObject lineObject = new GameObject();
            lineObject.tag = tag;
            LineRenderer newLine = lineObject.AddComponent<LineRenderer>();
            newLine.SetVertexCount(points.Length);
            newLine.SetPositions(points);
            newLine.useWorldSpace = false;
            lineObject.transform.position = position;
            Vector3 lastPoint = points[points.Length - 1];
            // Store the line and its last point to easily determine when its last point is no longer visible.
            lines.Add(new KeyValuePair<LineRenderer, Vector3>(newLine, lastPoint));
            linePositions.Add(lastPoint, points);*/
        line.gameObject.tag = tag;
        Vector3[] curPositions = pruneOldPoints(linePositions[line.gameObject], line.gameObject.transform.position);
        Vector3[] mergedLine = new Vector3[curPositions.Length + points.Length];
        System.Array.Copy(curPositions, mergedLine, curPositions.Length);
        System.Array.Copy(points, 0, mergedLine, curPositions.Length, points.Length);
        linePositions.Remove(line.gameObject);
        linePositions.Add(line.gameObject, mergedLine);
        line.numPositions = mergedLine.Length;
        line.SetPositions(mergedLine);

        return mergedLine[mergedLine.Length - 1];
    }

    private Vector3[] pruneOldPoints(Vector3[] points, Vector3 offset) {
        int maxY = int.MinValue;
        List<Vector3> keep = new List<Vector3>();
        for (int i = 0; i < points.Length-1; i++) {
            Vector3 pos = points[i];
            maxY = Mathf.RoundToInt(Mathf.Max(maxY, (pos + offset).y));
            float cutoffPoint = screenBottom - pointRemovalOffset;
            if ((points[i+1] + offset).y >=  cutoffPoint || (Mathf.RoundToInt((points[i+1] + offset).y) > maxY && maxY > cutoffPoint)) { 
                keep.Add(pos);
            }
        }
        return keep.ToArray();
    }

    /// <summary>
    /// Generates a new line and chooses the type of line it should be.
    /// </summary>
    /// <returns>The generated line.</returns>
    private Vector3 spawnNewLine(Vector3 from, LineRenderer line) {
        float roll = Random.Range(0, 100);
        if (roll < 40) {
            return generateLine(randomLine(numberOfPoints, lines[lines.Count - 1].Value), from, line);
        }
        else if (roll >= 40 && roll < 60) {
            return generateLine(uLine(40, 10, lines[lines.Count - 1].Value), from, line);
        }
        else if (roll >= 60 && roll < 90) {
            return generateLine(zigZagLine(4,40, 5, lines[lines.Count - 1].Value), from, line);
        }
        else {
           // return generateLine(zigZagLine(4, 10, 3, lines[lines.Count - 1].Value), from, line);
            return createFork(from, line);
        }
    }

    private Vector3[] straightLine(Vector3 from, Vector3 to) {
        return new Vector3[] { from, to };
    }

    private Vector3 createFork(Vector3 from, LineRenderer line) {
     //   accelerateTo(0.02f);
        Vector3 forkPos = generateLine(forkInitLine(from), from, line);
        LineRenderer leftLine = newLine(line.gameObject.transform.position, forkPos);
        lines[lines.Count-1] = new KeyValuePair<LineRenderer, Vector3>(leftLine, generateLine(leftChoiceLine(forkPos), from, leftLine, "ChoicePath"));
        LineRenderer rightLine = newLine(line.gameObject.transform.position, forkPos);
        lines[lines.Count - 1] = new KeyValuePair<LineRenderer, Vector3>(rightLine, generateLine(rightChoiceLine(forkPos), from, rightLine, "ChoicePath"));
        line.gameObject.tag = "Deactivated";
        nextForkPos = forkPos;
        return forkPos;
    }

    private Vector3[] forkInitLine(Vector3 previousPoint) {
        Vector3 midPoint = previousPoint + new Vector3(-previousPoint.x, 7, 0);
        Vector3[] points = new Vector3[] { previousPoint, midPoint, midPoint + new Vector3(0, 8, 0) };
        return points;
    }

    private Vector3[] rightChoiceLine(Vector3 previousPoint) {
        Vector3 rightPoint = previousPoint + new Vector3(Mathf.Abs(previousPoint.x - screenLeftPos) / 1.5f, 3, 0);
        Vector3[] points = new Vector3[] { previousPoint, rightPoint, rightPoint + new Vector3(0, Mathf.Abs(screenBottom-screenTop)*1.2f, 0) };
        return points;
    }

    private Vector3[] leftChoiceLine(Vector3 previousPoint) {
        Vector3 leftPoint = previousPoint + new Vector3(-Mathf.Abs(previousPoint.x - screenLeftPos)/1.5f,3,0);
        Vector3[] points = new Vector3[] { previousPoint, leftPoint, leftPoint + new Vector3(0, Mathf.Abs(screenBottom - screenTop) * 1.2f,0) };
        return points;
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
        height = Mathf.Min(height, Mathf.Abs(screenTop - screenBottom - 2) / 3);
        int direction = (previousPoint.x > 0) ? -1 : 1;
        float distToEdge = (direction == 1) ? Mathf.Abs(screenRightPos - previousPoint.x - xMargin) : Mathf.Abs(previousPoint.x - (xMargin + screenLeftPos));
        width = Mathf.Min(distToEdge, width);
        oppositePoint.x = oppositePoint.x + width * 0.5f * direction;
        Vector3 middlePoint1 = previousPoint + new Vector3(0, height, 0);
        Vector3 middlePoint2 = oppositePoint + new Vector3(0, height, 0);
        Vector3 turnPoint = oppositePoint + new Vector3(direction * width * 0.25f, 0, 0);
        Vector3 endPoint = turnPoint + new Vector3(direction * width * 0.25f, 0, 0);
        Vector3[] line = new Vector3[] { previousPoint, middlePoint1, middlePoint2, oppositePoint, turnPoint, endPoint, endPoint + new Vector3(0,20,0)};
        return line;
    }
    
    private Vector3[] zigZagLine(float height, float zagWidth, int zags, Vector3 previousPoint) {
        Vector3[] points = new Vector3[zags*2+2];
        points[0] = previousPoint;
        points[1] = previousPoint + new Vector3(0, height, 0);
        int dir = (previousPoint.x > 0) ? -1 : 1;
        zagWidth = Mathf.Min(zagWidth, Mathf.Abs(screenRightPos - screenLeftPos - 2*xMargin)/2);
        points[1].x = previousPoint.x + dir * zagWidth;
        
        //points[1].x = Mathf.Clamp(points[1].x, screenLeftPos + xMargin, screenRightPos - xMargin);
        points[2] = points[1] + new Vector3(0, height / 2, 0);
        for (int i = 3; i < zags*2+1; i+=2) {
            dir = -dir;
            points[i] = points[i - 1] + new Vector3(0, height, 0);
            points[i].x = points[i].x + dir * zagWidth;
            points[i + 1] = points[i] + new Vector3(0, height / 2, 0);
        }
        points[zags*2 + 1] = points[zags*2] + new Vector3(0, height/2, 0);
        return points;
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
        Vector3 transition = new Vector3(0, 3, 0);
        Vector3[] newLine = new Vector3[points];
        newLine[0] = previousPoint;
        newLine[1] = previousPoint + transition;

        for (int i = 2; i < points-1; ++i) {
            newLine[i] = generateNewPoint(newLine[i - 1]);
        }
        newLine[points - 1] = newLine[points - 2] + transition;
        return newLine;
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
        
        // Check if any line is outside visible boundary
        for (int i = 0; i < lines.Count; i++) {
            KeyValuePair<LineRenderer,Vector3> line = lines[i];
            line.Key.transform.Translate(0, -scrollSpeed, 0);
            GameObject lineObject = line.Key.gameObject;
            // If below screen bottom
            if (line.Value.y + lineObject.transform.position.y < screenTop + generationOffset && line.Key.gameObject.tag == "Line") {
                lines[i] = new KeyValuePair<LineRenderer, Vector3>(line.Key, spawnNewLine(line.Value, line.Key));
            }
        }
    }


    /// <summary>
    /// Generates a point above a previous one, with a random x-value.
    /// </summary>
    /// <param name="previousPoint">Previous point to step from.</param>
    /// <returns>The new point.</returns>
    private Vector3 generateNewPoint(Vector3 previousPoint) {
        float yIncrease = yStep;
        Vector3 newPoint = previousPoint + new Vector3(Random.Range(-2.0f, 2.0f), yIncrease, 0);
        newPoint.x = Mathf.Clamp(newPoint.x, screenLeftPos + xMargin, screenRightPos - xMargin);
        //float xPos = Random.Range(xMargin - screenRightPos, screenRightPos- xMargin);
        //float xPos = Mathf.Clamp(3.0f*Mathf.Sin(previousPoint.y), xMargin - screenWidth, screenWidth - xMargin);
        return newPoint;
    }

    /// <summary>
    /// Interpolates across a set of points, creating a smooth curved line between them. 
    /// </summary>
    /// <param name="arrayToCurve">Set to interpolate.</param>
    /// <param name="smoothness">Smoothness factor.</param>
    /// <returns>The interpolated positions.</returns>
    private Vector3[] smoothInterpolate(Vector3[] arrayToCurve, float smoothness) {
        int numberOfPoints = arrayToCurve.Length;
        smoothness = Mathf.Max(1.0f, smoothness);
        int curvedLength = numberOfPoints * Mathf.RoundToInt(smoothness) - 1;
        List<Vector3> points;
        List<Vector3> curvedPoints = new List<Vector3>(curvedLength);


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

    private Vector3[] linearInterpolate(Vector3[] arrayToSmooth, float smoothness) {
        int points = arrayToSmooth.Length;
        int smooth = Mathf.Max(1,Mathf.RoundToInt(smoothness));
        Vector3[] linearPoints = new Vector3[points * smooth];

        for (int i = 0; i < points-1; ++i) {
            Vector3 from = arrayToSmooth[i];
            Vector3 to = arrayToSmooth[i+1];
            Vector3 step = (to - from) / smooth;
            linearPoints[i * smooth] = arrayToSmooth[i];
            for (int j = (i * smooth)+1; j < (i+1)*smooth; ++j) 
                linearPoints[j] = linearPoints[j - 1] + step;
        }
        linearPoints[linearPoints.Length-1] = arrayToSmooth[points-1];
        return linearPoints;
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
        float distToOriginLine = float.MaxValue;
        KeyValuePair<LineRenderer,Vector3> closestLine = lines[0];
        foreach (KeyValuePair<LineRenderer, Vector3> line in lines) {
            GameObject lineObject = line.Key.gameObject;
            Vector3[] points = linePositions[lineObject];
            Vector3 offset = lineObject.transform.position;
            for (int i = 0; i < points.Length-1; ++i) {
                Vector3 dir = points[i + 1] - points[i];
                float distToPoint = DistancePointLine(position,points[i]+offset,points[i+1]+offset);
                //float distToPoint = Mathf.Abs((position - (points[i] + offset)).magnitude);
                if (distToPoint < minDist) {
                    minDist = distToPoint;
                    closestLine = line;
                }
                distToOriginLine = (lineObject.tag == "Deactivated") ? Mathf.Min(distToPoint, distToOriginLine) : distToOriginLine;
            }
        }
        if (minDist <= 1.0f && distToOriginLine > choiceSnapDistance)
          checkChoiceLine(closestLine);
        return minDist;
    }

    public float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
    }
    public Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f) {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }

    private void checkChoiceLine(KeyValuePair<LineRenderer,Vector3> line) {
        GameObject lineObject = line.Key.gameObject;
        if (lineObject.tag == "ChoicePath") {
            chooseLine(line);
        //    generateLine(randomLine(numberOfPoints, line.Value),lineObject.transform.position);
        }
    }

    private void chooseLine(KeyValuePair<LineRenderer,Vector3> line) {
        foreach (KeyValuePair<LineRenderer,Vector3> l in lines) {
            if (l.Key.gameObject != line.Key.gameObject) {
                Destroy(l.Key.gameObject);
            }
        }
        lines.Clear();
        lines.Add(line);
        line.Key.gameObject.tag = "Line";
        accelerateTo(0.25f);
        nextForkPos = Vector3.zero;
    }

}
