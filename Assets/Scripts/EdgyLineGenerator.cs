using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgyLineGenerator : MonoBehaviour {

    public float thickness = 1.0f;
    public float yStep = 2.0f;
    public float scrollSpeed = 0.02f;
    public GameObject lineObject;

    public Camera mainCamera;

    private Vector3[] lastPoints;
    private Vector3 lastPoint;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private float screenLeftPos, screenRightPos, screenTop, screenBottom;

    private List<KeyValuePair<GameObject,Vector3>> meshes = new List<KeyValuePair<GameObject, Vector3>>();

	// Use this for initialization
	void Start () {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        screenLeftPos = lowerLeft.x;
        screenRightPos = upperRight.x;
        screenTop = upperRight.y;
        screenBottom = lowerLeft.y;

        lastPoint = new Vector3(0,-10,0);
        lastPoints = new Vector3[] {
            new Vector3 (-thickness, -10, 0),
            new Vector3 (thickness, -10, 0)
        };

        spawnNewLine(lastPoint + new Vector3(Random.Range(0, 0), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
        spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), Vector3.zero);
    }

    // Update is called once per frame
    void Update () {
        RaycastHit hit;
        Debug.DrawLine(new Vector3(0, -7, -2), new Vector3(0, -7, 2), Color.green, 1.0f);
        if (Physics.Raycast(new Vector3(0,-7,-0.01f), new Vector3(0, -7, 0.5f),out hit)) {
         //   print(hit.transform.name);
        }
        foreach (KeyValuePair<GameObject,Vector3> line in meshes) {
            line.Key.transform.Translate(0, -scrollSpeed, 0);
            Vector3[] vex = line.Key.GetComponent<MeshFilter>().mesh.vertices;
            if (line.Key.transform.position.y + line.Value.y <= screenBottom) {
                Destroy(line.Key);
                meshes.Remove(line);
                spawnNewLine(lastPoint + new Vector3(Random.Range(-3, 3), yStep, 0), meshes[meshes.Count-1].Key.transform.position);
                break;
            }
        }

    }

    private void spawnNewLine(Vector3 to, Vector3 offset) {
        GameObject newObject = Instantiate(lineObject);
        Mesh line = createLineMesh(lastPoint, to, lastPoints, thickness);
        newObject.GetComponent<MeshCollider>().sharedMesh = line;
        newObject.GetComponent<MeshFilter>().mesh = line;
        newObject.transform.position = offset;
        meshes.Add(new KeyValuePair<GameObject, Vector3>(newObject, to));
    }

    private Mesh createLineMesh(Vector3 from, Vector3 to, Vector3[] previousVertices, float thickness) {

        Mesh mesh = new Mesh();

        Vector3 dir = from - to;
        Vector3 dir1 = new Vector3(dir.y, -dir.x, dir.z).normalized;
        Vector3 dir2 = -dir1;

        Vector3 toFirst = to + dir1;
        Vector3 toSecond = to + dir2;

        Vector3 gapVertex;

        if (to.x > from.x) {
            gapVertex = previousVertices[1];
            previousVertices[1] = toSecond + dir;
        } else {
            gapVertex = previousVertices[0];
            previousVertices[0] = toFirst + dir;
        }

        Vector3[] vertices = new Vector3[] {
            previousVertices[0],
            previousVertices[1],
            toFirst,
            toSecond,
            gapVertex
        };

        Vector2[] uv = new Vector2[] {
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(0,0)
        };

        int[] triangles = new int[] {
            2,1,0,
            3,1,2,
            4,0,1
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        lastPoint = to;
        lastPoints = new Vector3[] { toFirst, toSecond };

        Vector3[] norms = new Vector3[] { new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1),new Vector3(0, 0, -1), new Vector3(0, 0, -1) };
        return mesh;
    }
}
