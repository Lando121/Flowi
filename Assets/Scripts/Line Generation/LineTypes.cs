using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTypes : MonoBehaviour {

    public float screenTop, screenBottom, screenRight, screenLeft, xMargin, screenWidth, screenHeight;

    public void setScreenVariables(float top, float bottom, float right, float left, float xM) {
        screenTop = top;
        screenBottom = bottom;
        screenRight = right;
        screenLeft = left;
        xMargin = xM;
        screenHeight = Mathf.Abs(screenTop - screenBottom);
        screenWidth = Mathf.Abs(screenRight - screenLeft);
    }

    public Vector3[] forkInitLine(Vector3 previousPoint) {
        Vector3 midPoint = previousPoint + new Vector3(-previousPoint.x, screenHeight/2, 0);
        Vector3[] points = new Vector3[] { previousPoint, midPoint, midPoint + new Vector3(0, screenHeight/2, 0) };
        return points;
    }

    public Vector3[] rightChoiceLine(Vector3 previousPoint) {
        Vector3 rightPoint = previousPoint + new Vector3(screenWidth / 3, 3, 0);
        Vector3[] points = new Vector3[] { previousPoint, rightPoint, rightPoint + new Vector3(0, screenHeight * 1.2f, 0) };
        return points;
    }

    public Vector3[] leftChoiceLine(Vector3 previousPoint) {
        Vector3 leftPoint = previousPoint + new Vector3(-screenWidth / 3, 3, 0);
        Vector3[] points = new Vector3[] { previousPoint, leftPoint, leftPoint + new Vector3(0, screenHeight * 1.2f, 0) };
        return points;
    }

    public Vector3[] straightLine(Vector3 from, Vector3 to) {
        return new Vector3[] { from, to };
    }

    /// <summary>
    /// Generates an inverse U-shaped line.
    /// </summary>
    /// <param name="height">Height of the turn.</param>
    /// <param name="width">Width of the turn.</param>
    /// <param name="previousPoint">Point that the line starts from.</param>
    /// <returns>The generated line.</returns>
    public Vector3[] uLine(float height, float width, Vector3 previousPoint) {
        Vector3 oppositePoint = previousPoint;
        height = Mathf.Min(height, Mathf.Abs(screenTop - screenBottom - 2) / 3);
        int direction = (previousPoint.x > 0) ? -1 : 1;
        float distToEdge = (direction == 1) ? Mathf.Abs(screenRight - previousPoint.x - xMargin) : Mathf.Abs(previousPoint.x - (xMargin + screenLeft));
        width = Mathf.Min(distToEdge, width);
        oppositePoint.x = oppositePoint.x + width * 0.5f * direction;
        Vector3 middlePoint1 = previousPoint + new Vector3(0, height, 0);
        Vector3 middlePoint2 = oppositePoint + new Vector3(0, height, 0);
        Vector3 turnPoint = oppositePoint + new Vector3(direction * width * 0.25f, 0, 0);
        Vector3 endPoint = turnPoint + new Vector3(direction * width * 0.25f, 0, 0);
        Vector3[] line = new Vector3[] { previousPoint, middlePoint1, middlePoint2, oppositePoint, turnPoint, endPoint, endPoint + new Vector3(0, 20, 0) };
        return line;
    }

    public Vector3[] zigZagLine(float height, float zagWidth, int zags, Vector3 previousPoint) {
        Vector3[] points = new Vector3[zags * 2 + 2];
        points[0] = previousPoint;
        points[1] = previousPoint + new Vector3(0, height, 0);
        int dir = (previousPoint.x > 0) ? -1 : 1;
        zagWidth = Mathf.Min(zagWidth, Mathf.Abs(screenRight - screenLeft - 2 * xMargin) / 2);
        points[1].x = previousPoint.x + dir * zagWidth;

        //points[1].x = Mathf.Clamp(points[1].x, screenLeftPos + xMargin, screenRightPos - xMargin);
        points[2] = points[1] + new Vector3(0, height / 2, 0);
        for (int i = 3; i < zags * 2 + 1; i += 2) {
            dir = -dir;
            points[i] = points[i - 1] + new Vector3(0, height, 0);
            points[i].x = points[i].x + dir * zagWidth;
            points[i + 1] = points[i] + new Vector3(0, height / 2, 0);
        }
        points[zags * 2 + 1] = points[zags * 2] + new Vector3(0, height / 2, 0);
        return points;
    }

    /// <summary>
    /// Generates a randomly shaped line going upwards.
    /// </summary>
    /// <param name="points">Amount of points the line should contain.</param>
    /// <param name="previousPoint">Point that the line starts from.</param>
    /// <returns>The generated line.</returns>
    public Vector3[] randomLine(int points, Vector3 previousPoint, float yStep) {
        if (points <= 1)
            return null;
        Vector3 transition = new Vector3(0, 3, 0);
        Vector3[] newLine = new Vector3[points];
        newLine[0] = previousPoint;
        newLine[1] = previousPoint + transition;

        for (int i = 2; i < points - 1; ++i) {
            newLine[i] = generateNewPoint(newLine[i - 1], yStep);
        }
        newLine[points - 1] = newLine[points - 2] + transition;
        return newLine;
    }

    /// <summary>
    /// Generates a point above a previous one, with a random x-value.
    /// </summary>
    /// <param name="previousPoint">Previous point to step from.</param>
    /// <returns>The new point.</returns>
    public Vector3 generateNewPoint(Vector3 previousPoint, float yStep) {
        float yIncrease = yStep;
        Vector3 newPoint = previousPoint + new Vector3(Random.Range(-2.0f, 2.0f), yIncrease, 0);
        newPoint.x = Mathf.Clamp(newPoint.x, screenLeft + xMargin, screenRight - xMargin);
        //float xPos = Random.Range(xMargin - screenRightPos, screenRightPos- xMargin);
        //float xPos = Mathf.Clamp(3.0f*Mathf.Sin(previousPoint.y), xMargin - screenWidth, screenWidth - xMargin);
        return newPoint;
    }

}
