using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoxObstacle : MonoBehaviour {

	public float width;
	public float scrollSpeed;
	public Color color;
	private float screenLeftPos, screenRightPos, screenTop, screenBottom;
	private int nPoints = 8;
	public Camera mainCamera;

	private List<KeyValuePair<LineRenderer,Vector3>> lines = new List<KeyValuePair<LineRenderer, Vector3>>();
	private Dictionary<Vector3, Vector3[]> linePositions = new Dictionary<Vector3, Vector3[]>();

	// Use this for initialization
	void Start () {
		//Generate two connecting lines to random from/to

		Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

		screenLeftPos = lowerLeft.x;
		screenRightPos = upperRight.x;
		screenTop = upperRight.y;
		screenBottom = lowerLeft.y;
		float height = 2 * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect; 
		float bottom = mainCamera.ScreenToWorldPoint(new Vector3(0,0,0)).y;
		Vector3[] startPos = new Vector3[6];
		Vector3[] startPos2 = new Vector3[4];

		startPos [0] = new Vector3 (screenLeftPos + width*0.5f, bottom, 0);
		startPos [1] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.3f), 0); 
		startPos [2] = new Vector3 (screenLeftPos + 1f, bottom + (height * 0.3f), 0); 
		startPos [3] = new Vector3 (screenLeftPos + 1f, bottom + (height * 0.8f), 0);
		startPos [4] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.8f), 0);
		startPos [5] = new Vector3 (screenLeftPos + width*0.5f, bottom + height, 0);

		startPos2 [0] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.3f), 0); 
		startPos2 [1] = new Vector3 (screenRightPos - 1f, bottom + (height * 0.3f), 0); 
		startPos2 [2] = new Vector3 (screenRightPos - 1f, bottom + (height * 0.8f), 0);
		startPos2 [3] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.8f), 0);

		generateLine(startPos);
		generateLine(startPos2);

	}
	// Update is called once per frame
	void Update () {
	}
		
	void generateLine(Vector3[] startPos){
		GameObject zFig = new GameObject ();
		LineRenderer zLine = zFig.AddComponent<LineRenderer> ();
		zLine.SetVertexCount(startPos.Length);
		zLine.SetPositions(startPos);
		zLine.useWorldSpace = false;
		zFig.transform.position = new Vector3(0,0,-1);

		Vector3 lastPoint = startPos[startPos.Length - 1];
		// Store the line and its last point to easily determine when its last point is no longer visible.
		lines.Add(new KeyValuePair<LineRenderer, Vector3>(zLine, lastPoint));
		linePositions.Add(lastPoint, startPos);
	}
	public Vector3[] getLine1(){
		Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

		screenLeftPos = lowerLeft.x;
		screenRightPos = upperRight.x;
		screenTop = upperRight.y;
		screenBottom = lowerLeft.y;
		float height = 2 * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect; 
		float bottom = mainCamera.ScreenToWorldPoint(new Vector3(0,0,0)).y;
		Vector3[] startPos = new Vector3[6];
	
		startPos [0] = new Vector3 (screenLeftPos + width*0.5f, bottom, 0);
		startPos [1] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.3f), 0); 
		startPos [2] = new Vector3 (screenLeftPos + 1f, bottom + (height * 0.3f), 0); 
		startPos [3] = new Vector3 (screenLeftPos + 1f, bottom + (height * 0.8f), 0);
		startPos [4] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.8f), 0);
		startPos [5] = new Vector3 (screenLeftPos + width*0.5f, bottom + height, 0);



		return startPos;
	}
	public Vector3[] getLine2(){
		Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

		screenLeftPos = lowerLeft.x;
		screenRightPos = upperRight.x;
		screenTop = upperRight.y;
		screenBottom = lowerLeft.y;
		float height = 2 * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect; 
		float bottom = mainCamera.ScreenToWorldPoint(new Vector3(0,0,0)).y;
		Vector3[] startPos = new Vector3[4];
		startPos [0] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.3f), 0); 
		startPos [1] = new Vector3 (screenRightPos - 1f, bottom + (height * 0.3f), 0); 
		startPos [2] = new Vector3 (screenRightPos - 1f, bottom + (height * 0.8f), 0);
		startPos [3] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.8f), 0);

		return startPos;
	}
}
