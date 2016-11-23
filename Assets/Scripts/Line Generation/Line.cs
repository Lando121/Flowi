using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    public LineRenderer line;
    public Vector3[] points;
    public Vector3 endPoint;

    public float scrollSpeed;
    public float screenBottom;

    public float lineDifficulty = 1.0f;
    public float distanceToFork = 300;
    public float initWidth = 2.0f;
    public float lineWidth = 2.0f;

    public float pointRemovalOffset = 10.0f;
    public float generationOffset = 10.0f;
    private float thickness = 1.0f;
    public bool shouldGenerate = true;
    public bool shouldFork = false;

    private Renderer rend;

    // Use this for initialization
    void Start() {
        line = GetComponent<LineRenderer>();
        line.numCornerVertices = 5;
    }

    // Update is called once per frame
    void Update() {
        if (GameLoop.playing) {
            points = pruneOldPoints(points, transform.position);
            line.numPositions = points.Length;
            line.SetPositions(points);
            line.widthMultiplier = thickness;
            float travelDist = scrollSpeed * Time.deltaTime;
            transform.Translate(0, -travelDist, 0);
            distanceToFork -= travelDist;
            line.widthMultiplier = lineWidth;
            shouldFork = (distanceToFork <= 0) ? true : false;
            if (endPoint.y + transform.position.y < screenBottom - pointRemovalOffset) {
                die();
            }
        }

    }

    public void setDifficulty(float newDif) {
        lineDifficulty = newDif;
    }

    private void die() {
        GameObject.Find("Smooth Line Renderer").GetComponent<LineGenerator>().removeLine(this);
        Destroy(gameObject);
    }

    public void setSpeed(float speed) {
        scrollSpeed = speed;
    }

    public void drawLine(Vector3[] newPoints) {
        Vector3[] mergedPoints = mergeWithLine(newPoints);
        points = mergedPoints;
        endPoint = newPoints[newPoints.Length - 1];
    }

    public void setThickness(float amount) {
        thickness = amount;
    }

    private Vector3[] pruneOldPoints(Vector3[] points, Vector3 offset) {
        int maxY = int.MinValue;
        List<Vector3> keep = new List<Vector3>();
        for (int i = 0; i < points.Length - 1; i++) {
            Vector3 pos = points[i];
            maxY = Mathf.RoundToInt(Mathf.Max(maxY, (pos + offset).y));
            float cutoffPoint = screenBottom - pointRemovalOffset;
            if ((points[i + 1] + offset).y >= cutoffPoint || (Mathf.RoundToInt((points[i + 1] + offset).y) > maxY && maxY > cutoffPoint)) {
                keep.Add(pos);
            }
        }
        keep.Add(points[points.Length - 1]);
        return keep.ToArray();
    }

    private Vector3[] mergeWithLine(Vector3[] newPoints) {
        Vector3[] curPositions = points;
        Vector3[] mergedLine = new Vector3[curPositions.Length + newPoints.Length];
        System.Array.Copy(curPositions, mergedLine, curPositions.Length);
        System.Array.Copy(newPoints, 0, mergedLine, curPositions.Length, newPoints.Length);

        return mergedLine;
    }

}
