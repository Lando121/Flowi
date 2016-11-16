using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MonsterObstacle : MonoBehaviour {

	public float lineWidth;
	public float scrollSpeed;
	public Color color;
	private float screenLeftPos, screenRightPos, screenTop, screenBottom, width, height, bottom;
	private int nPoints = 8;
	public float monsterSpeed = 2.0f;
	public Camera mainCamera;

	private List<KeyValuePair<LineRenderer,Vector3>> lines = new List<KeyValuePair<LineRenderer, Vector3>>();
	private Dictionary<Vector3, Vector3[]> linePositions = new Dictionary<Vector3, Vector3[]>();
	private GameObject monster1;
	private GameObject monster2;
	private GameObject monster3;
	private GameObject monster4;
	private float monsterStartx;
	private float monsterStopx;

	// Use this for initialization
	void Start () {
		//Generate two connecting lines to random from/to

		Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

		screenLeftPos = lowerLeft.x;
		screenRightPos = upperRight.x;
		screenTop = upperRight.y;
		screenBottom = lowerLeft.y;
		height = 2 * Camera.main.orthographicSize;
		width = height * Camera.main.aspect; 
		bottom = mainCamera.ScreenToWorldPoint(new Vector3(0,0,0)).y;
		Vector3[] startPos = new Vector3[2];

		startPos [0] = new Vector3 (screenLeftPos + width*0.5f, bottom, 0);
		startPos [1] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.2f), 0); 
		generateLine(startPos);


		startPos [0] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.2f), 0);
		startPos [1] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.9f), 0);
		generateLine (startPos, screenLeftPos + width*1.3f);

		monster1 = new GameObject();
		SpriteRenderer r = monster1.AddComponent<SpriteRenderer> ();
		Sprite texture = (Sprite) Resources.Load<Sprite>("monster");
		r.sprite = texture;
		r.transform.localScale = new Vector3(1,1,0);
		monsterStartx = screenLeftPos + width * 0.15f;
		monsterStopx = screenRightPos - width * 0.15f;
		r.transform.position = new Vector3 (monsterStartx, bottom + (height * 0.8f), -1);
		/*
		monster2 = new GameObject();
		SpriteRenderer r2 = monster1.AddComponent<SpriteRenderer> ();
		Sprite texture2 = (Sprite) Resources.Load<Sprite>("monster");
		r2.sprite = texture;
		r2.transform.localScale = new Vector3(1,1,0);
		monsterStartx = screenLeftPos + width * 0.15f;
		monsterStopx = screenRightPos - width * 0.15f;
		r2.transform.position = new Vector3 (monsterStopx, bottom + (height * 0.6f), -1);

		monster3 = new GameObject();
		SpriteRenderer r3 = monster1.AddComponent<SpriteRenderer> ();
		Sprite texture3 = (Sprite) Resources.Load<Sprite>("monster");
		r3.sprite = texture3;
		r3.transform.localScale = new Vector3(1,1,0);
		monsterStartx = screenLeftPos + width * 0.15f;
		monsterStopx = screenRightPos - width * 0.15f;
		r3.transform.position = new Vector3 (monsterStartx, bottom + (height * 0.4f), -1);

		monster4 = new GameObject();
		SpriteRenderer r4 = monster1.AddComponent<SpriteRenderer> ();
		Sprite texture4 = (Sprite) Resources.Load<Sprite>("monster");
		r4.sprite = texture4;
		r4.transform.localScale = new Vector3(1,1,0);
		monsterStartx = screenLeftPos + width * 0.15f;
		monsterStopx = screenRightPos - width * 0.15f;
		r4.transform.position = new Vector3 (monsterStopx, bottom + (height * 0.2f), -1);

			

*/

			//Sprite.Create (r.material, new Rect (0, 0, 5, 5), new Vector2(0,0));



		//GameObject monster2 = new GameObject ();

		startPos [0] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height * 0.9f), 0);
		startPos [1] = new Vector3 (screenLeftPos + width*0.5f, bottom + (height), 0);
		generateLine (startPos);

	}
	// Update is called once per frame
	void Update () {
		if (monster1.transform.position.x >= monsterStopx && monsterSpeed > 0)
		{
			monsterSpeed = -monsterSpeed;

		} else if (monster1.transform.position.x <= monsterStartx && monsterSpeed < 0)
		{
			monsterSpeed = -monsterSpeed;
		}
		monster1.transform.Translate (Vector2.right * monsterSpeed * Time.deltaTime, Space.World);

		/*
		if (monster2.transform.position.x >= monsterStopx && monsterSpeed > 0)
		{
			monsterSpeed = -monsterSpeed;

		} else if (monster2.transform.position.x <= monsterStartx && monsterSpeed < 0)
		{
			monsterSpeed = -monsterSpeed;
		}
		monster2.transform.Translate (Vector2.right * monsterSpeed * Time.deltaTime, Space.World);

		if (monster3.transform.position.x >= monsterStopx && monsterSpeed > 0)
		{
			monsterSpeed = -monsterSpeed;

		} else if (monster3.transform.position.x <= monsterStartx && monsterSpeed < 0)
		{
			monsterSpeed = -monsterSpeed;
		}
		monster3.transform.Translate (Vector2.right * monsterSpeed * Time.deltaTime, Space.World);

		if (monster4.transform.position.x >= monsterStopx && monsterSpeed > 0)
		{
			monsterSpeed = -monsterSpeed;

		} else if (monster4.transform.position.x <= monsterStartx && monsterSpeed < 0)
		{
			monsterSpeed = -monsterSpeed;
		}
		monster4.transform.Translate (Vector2.right * monsterSpeed * Time.deltaTime, Space.World);
		*/
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

	void generateLine(Vector3[] startPos, float width){
		GameObject zFig = new GameObject ();
		LineRenderer zLine = zFig.AddComponent<LineRenderer> ();
		zLine.SetWidth(width,width);
		zLine.SetVertexCount(startPos.Length);
		zLine.SetPositions(startPos);
		zLine.useWorldSpace = false;
		zFig.transform.position = new Vector3(0,0,-1);

		Vector3 lastPoint = startPos[startPos.Length - 1];
		// Store the line and its last point to easily determine when its last point is no longer visible.
		lines.Add(new KeyValuePair<LineRenderer, Vector3>(zLine, lastPoint));
		linePositions.Add(lastPoint, startPos);
	}
	/*
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
	*/


}
