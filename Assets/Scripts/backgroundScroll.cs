using UnityEngine;
using System.Collections;

public class backgroundScroll : MonoBehaviour {
	public float scrollSpeed;
	public float backgroundSize;
	private LineGenerator lineGen;
	private GameObject gObj;
	public Camera mainCamera;
	// Use this for initialization
	void Start () {
		gObj = GameObject.Find("Smooth Line Renderer");
		//MeshRenderer sr = GetComponent<MeshRenderer>();
		Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector3 upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

		float height = 2 * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect; 
		transform.localScale = new Vector3 (width, height, 1);


		
	}
	
	// Update is called once per frame
	void Update () {
		lineGen = gObj.GetComponent<LineGenerator> ();
		Vector3 offset = new Vector3 (0, Time.time * 0.3f, 0);
		GetComponent<Renderer>().material.mainTextureOffset = offset;


	}

}
