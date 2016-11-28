using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour {

    public Camera mainCamera;

    private List<Line> lines = new List<Line>();

    public float xMargin = 0.5f;
    public float yStep = 0.5f;

    public float difficultySpeedIncrease = 0.05f;
    public float difficultyIncrement = 1.0f;

    public float startThickness = 2.0f;
    public float lineThickness = 1.0f;


    [Header("Scrolling attributes")]
    public float startSpeed = 0.1f;
    public float scrollSpeed;
    public float acceleration = 0.005f;
    public float choiceSnapDistance = 3.0f;

    private int accelerationMult = 1;
    private bool accelerating = false;
    private float targetSpeed;

    private Vector3 nextForkPos;

    public GameObject monster;
    public GameObject shootingMonster;
    public GameObject LinePrefab;
    private LineTypes lineTypes;

    private float screenLeft, screenRight, screenTop, screenBottom, screenWidth, screenHeight;

    private Line mainLine;

    // Use this for initialization
    void Start() {
        lineTypes = GetComponent<LineTypes>();

        lineThickness = startThickness;
        scrollSpeed = startSpeed;

        Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        screenLeft = lowerLeft.x;
        screenRight = upperRight.x;
        screenTop = upperRight.y;
        screenBottom = lowerLeft.y;
        screenWidth = Mathf.Abs(screenRight - screenLeft);
        screenHeight = Mathf.Abs(screenTop - screenBottom);

        createLine(lineTypes.straightLine(new Vector3(0, screenBottom, 0), new Vector3(0, screenBottom + screenHeight * 2, 0)), lineThickness);

    }

    private bool accelerate() {
        scrollSpeed += acceleration;
        scrollSpeed = Mathf.Max(0, scrollSpeed);
        return !(scrollSpeed < targetSpeed && accelerationMult == -1 || scrollSpeed > targetSpeed && accelerationMult == 1);
    }

    private void accelerateTo(float speed, float inDistance) {
        accelerating = true;
        targetSpeed = speed;
        accelerationMult = (speed > scrollSpeed) ? 1 : -1;
        float v0 = scrollSpeed;
        float t = inDistance / Mathf.Max(scrollSpeed, speed);
        acceleration = (speed - v0) / t;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (GameLoop.playing) {
            xMargin = mainLine.lineWidth / 2 + screenWidth/10;
            lineThickness = startThickness - mainLine.lineDifficulty * 0.2f;
            lineTypes.setScreenVariables(screenTop, screenBottom, screenRight, screenLeft, xMargin);
            updateLines();
            if (nextForkPos != Vector3.zero) {
                float dif = nextForkPos.y + mainLine.transform.position.y - screenTop;
                float brakeDistance = -0.5f * screenHeight;
                if (dif < brakeDistance) {
                    accelerateTo(0.0f, -brakeDistance);
                    nextForkPos = Vector3.zero;
                }
            }
            if (accelerating) {
                accelerating = accelerate();
            }
        }

    }

    private Line createLine(Vector3[] points, float thickness, string tag = "Line") {
        Vector3 offset = new Vector3(0, 0, -1);
        GameObject newLineObject = Instantiate(LinePrefab, offset, Quaternion.identity);
        Line newLine = newLineObject.GetComponent<Line>();
        newLine.lineWidth = thickness;
        newLine.transform.tag = tag;
        newLine.drawLine(points);
        newLine.setSpeed(scrollSpeed);
        if (lines.Count == 0) {
            mainLine = newLine;
        }
        lines.Add(newLine);
        newLine.setDifficulty(mainLine.lineDifficulty);
        return newLine;
    }

    public void removeLine(Line line) {
        lines.Remove(line);
    }

    private void updateLines() {
        foreach (Line line in lines) {
            line.setSpeed(scrollSpeed);
        }
        if (mainLine != null && mainLine.endPoint.y + mainLine.transform.position.y < screenTop + screenHeight / 3 && mainLine.shouldGenerate) {
            generateFurther();
        }
    }

    private Vector3[] createFork(Vector3 from) {
        Vector3[] initLine = lineTypes.forkInitLine(from);
        Vector3 offset = mainLine.transform.position;
        offset.z = 0;
        Vector3 forkPos = initLine[initLine.Length - 1] + offset - new Vector3(0,(lineThickness-mainLine.outlineThickness)/2,0);
        Line leftLine = createLine(lineTypes.leftChoiceLine(forkPos), lineThickness, "ChoicePath");
        //leftLine.transform.position = leftLine.transform.position + new Vector3(0, 0, 0.5f);
        Line rightLine = createLine(lineTypes.rightChoiceLine(forkPos), lineThickness, "ChoicePath");
    //    rightLine.transform.position = rightLine.transform.position + new Vector3(0, 0, 0.5f);
        leftLine.lineDifficulty = mainLine.lineDifficulty + difficultyIncrement;
        rightLine.lineDifficulty = Mathf.Max(mainLine.lineDifficulty - difficultyIncrement, 1.0f);
        nextForkPos = forkPos - offset;
        mainLine.transform.position = mainLine.transform.position + new Vector3(0, 0, 0.5f);
        return initLine;
    }

    /// <summary>
    /// Calculates the distance to the line.
    /// </summary>
    /// <param name="position">Position in pixel-space.</param>
    /// <returns>Distance to line.</returns>
    public float distanceToLine(Vector2 position) {
        return distanceToLine(mainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, mainCamera.nearClipPlane)));
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
        Line closestLine = mainLine;
        foreach (Line line in lines) {
            Vector3[] points = line.points;
            Vector3 offset = line.transform.position;
            for (int i = 0; i < points.Length - 1; ++i) {
                Vector3 dir = points[i + 1] - points[i];
                float distToPoint = distancePointLine(position, points[i] + offset, points[i + 1] + offset);
                //float distToPoint = Mathf.Abs((position - (points[i] + offset)).magnitude);
                if (distToPoint < minDist) {
                    minDist = distToPoint;
                    closestLine = line;
                }
                distToOriginLine = (line == mainLine) ? Mathf.Min(distToPoint, distToOriginLine) : distToOriginLine;
            }
        }
        if (minDist <= 2.0f && distToOriginLine > choiceSnapDistance) {
            checkChoiceLine(closestLine);
        }
        return minDist;
    }

    public float distancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        return Vector3.Magnitude(projectPointLine(point, lineStart, lineEnd) - point);
    }
    public Vector3 projectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f) {
            lhs = (lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + (lhs * num2));
    }

    private void checkChoiceLine(Line line) {
        if (line.transform.tag == "ChoicePath") {
            chooseLine(line);
            //    generateLine(randomLine(numberOfPoints, line.Value),lineObject.transform.position);
        }
    }

    private void chooseLine(Line line) {
        mainLine = line;
        line.distanceToFork *= line.lineDifficulty * (1 + difficultySpeedIncrease);
        foreach (Line l in lines) {
            if (l != line) {
                Destroy(l.gameObject);
            }
        }
        lines.Clear();
        lines.Add(mainLine);
        line.transform.tag = "Line";
        accelerateTo(startSpeed + mainLine.lineDifficulty * difficultySpeedIncrease, screenHeight);
    }

    /// <summary>
    /// Generates a new line and chooses the type of line it should be.
    /// </summary>
    /// <returns>The generated line.</returns>
    private void generateFurther() {
        float typeRoll = Random.Range(0, 100);
        float lineRoll = Random.Range(0, 1000);
        Vector3[] addedLine;
        Line line = mainLine;
        Vector3 from = line.endPoint;
        lineRoll *= mainLine.lineDifficulty / 5;

        if (mainLine.shouldFork) {
            addedLine = createFork(from);
            mainLine.shouldGenerate = false;
        } else if (typeRoll >= 50) {
             if (lineRoll < 600) {
                addedLine = lineTypes.randomLine(10, from, 3, mainLine.lineDifficulty);
            }
            else if (lineRoll < 900) {
                addedLine = lineTypes.zigZagLine(4, 40, 2, from, mainLine.lineDifficulty);
            }
            else {
                addedLine = lineTypes.uLine(40, 10, from);
            }
        } else {
            if (Random.Range(0,10) >= 5) {
                addedLine = generateMonsterObstacle(from, screenWidth * 0.9f, 2 + Mathf.RoundToInt(mainLine.lineDifficulty / 3));
            } else {
                addedLine = shootingObstacle(from, mainLine.lineDifficulty);
            }
        }

        line.drawLine(addedLine);
    }

    private Vector3[] shootingObstacle(Vector3 from, float difficulty) {
        int monsterAmount = 3 + Mathf.RoundToInt(difficulty);
        float yStep = screenHeight / 3;
        float obstacleDist = monsterAmount * yStep;
        Vector3 startSpawn = from + mainLine.transform.position;
        for (int i = 0; i < monsterAmount; ++i) {
            GameObject newMonster = Instantiate(shootingMonster);
            newMonster.transform.parent = mainLine.transform;
            Vector3 monsterSpawnPos;
            ShootMonster monsterScript = newMonster.GetComponent<ShootMonster>();
            if (Random.Range(0,100) >= 50) {
                monsterSpawnPos = new Vector3(screenLeft + xMargin, startSpawn.y + i * yStep, -1.5f);
            } else {
                monsterSpawnPos = new Vector3(screenRight - xMargin, startSpawn.y + i * yStep, -1.5f);
                newMonster.GetComponent<SpriteRenderer>().flipX = true;
                monsterScript.shootDir = -1;
            }
            newMonster.transform.position = monsterSpawnPos;
            monsterScript.setAttachedLine(mainLine);
        }
        return new Vector3[] { from, new Vector3(0, from.y + obstacleDist, 0), new Vector3(0, from.y + 1.5f * obstacleDist, 0) };
    }

    private Vector3[] generateMonsterObstacle(Vector3 from, float midWidth, int monsterCount) {

     //   midWidth = Mathf.Clamp(midWidth, 0, screenWidth - 2 * xMargin);
        Vector3 startPoint = new Vector3(screenLeft + screenWidth * 0.5f, from.y + (screenHeight * 0.2f), 0);
        Vector3[] startPos = new Vector3[] { from, startPoint, startPoint + new Vector3(0, 5, 0) };

        Vector3[] midPoints = new Vector3[] { startPos[startPos.Length - 1] + mainLine.transform.position, startPos[startPos.Length - 1] + mainLine.transform.position + new Vector3(0, screenHeight * 0.7f, 0) };

        Line monsterLine = createLine(midPoints, midWidth);


        for (int i = 0; i < monsterCount; i++) {
            Vector3 monsterSpawnPos = new Vector3(Random.Range(-midWidth, midWidth), Random.Range(midPoints[0].y, midPoints[1].y), -1.5f);
            GameObject newMonster = Instantiate(monster);
            newMonster.transform.parent = monsterLine.transform;
            newMonster.GetComponent<MonsterController>().initiateMonster(monsterSpawnPos + monsterLine.transform.position, 1.0f, -midWidth / 2, midWidth / 2);
        }

        Vector3[] endPoints = new Vector3[] { midPoints[midPoints.Length - 1], midPoints[midPoints.Length - 1] + new Vector3(0, screenHeight * 0.1f, 0) };
        Line endLine = createLine(endPoints, lineThickness);
        setMainLine(endLine);
        return startPos;
    }

    private void setMainLine(Line line) {
        line.distanceToFork = mainLine.distanceToFork;
        mainLine = line;
    }

}
