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
        shootTimer = Random.Range(0,shootCooldown);
        projectileSpeed += Random.Range(-0.2f, 0.2f);
	}
	
    public void setAttachedLine(Line line) {
        attachedLine = line;
        transform.parent = line.transform;
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (GameLoop.playing) {
            foreach (GameObject p in projectiles) {
                if (p != null) {
                    p.transform.Translate(new Vector3(shootDir * projectileSpeed, -attachedLine.scrollSpeed, 0) * Time.deltaTime);
                }
            }

            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0) {
                shootTimer = shootCooldown;
                shoot();
            }
        }
	}

    private void shoot() {
        GameObject newProjectile = Instantiate(projectile, transform.position + new Vector3(0, 0, -2), Quaternion.identity);
        Destroy(newProjectile, 5);
        projectiles.Add(newProjectile);
    }
}
