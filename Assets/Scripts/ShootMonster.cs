using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMonster : MonoBehaviour {

    public GameObject projectile;
    public float shootCooldown = 1.0f;
    private float shootTimer;
    public float projectileSpeed = 10.0f;

    public Line attachedLine;
    private List<GameObject> projectiles = new List<GameObject>();
    public int shootDir = 1;


	// Use this for initialization
	void Start () {
        shootTimer = shootCooldown;
	}
	
    public void setAttachedLine(Line line) {
        attachedLine = line;
        transform.parent = line.transform;
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (GameLoop.playing) {
            foreach (GameObject p in projectiles) {
                p.transform.Translate(new Vector3(shootDir * projectileSpeed, -attachedLine.scrollSpeed, 0) * Time.deltaTime);
            }

            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0) {
                shootTimer = shootCooldown;
                shoot();
            }
        }
	}

    private void shoot() {
        projectiles.Add(Instantiate(projectile, transform.position, Quaternion.identity));
    }
}
