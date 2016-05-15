using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class ShipController : SteeringVehicle {
	public GameObject laserBase;
	private int hp = 20;
	public float fireDistance = 25f;
	protected bool canFire = true;
	protected float cooldown = 1.0f;
	protected float depth = 10;
	public int shotsFired = 0;
	private int numberOfHits = 0;
	protected float lifeTime = 0.0f;
	protected float aggressiveness = 1.0f;
	protected List<string> enemyTags = new List<string>();
	public static float areaSize = -1;
	string fileName = "bulletlog.txt";
	protected bool invulnerable = false;
    protected void InitializeShip()
    {
        lifeTime = 0.0f;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = GetComponentInChildren<MeshRenderer>();
        }
        depth = mr.bounds.size.z;

        if (areaSize < 0)
        {
            areaSize = Camera.main.GetComponent<AsteroidSource>().asteroidRadius;
        }
		invulnerable = true;
		Invoke ("LoseInvulnerability", 1.0f);
    }

	public void LoseInvulnerability() {
		invulnerable = false;
	}
	public virtual void Fire() {
		if (canFire) {
			GameObject laser = GameObject.Instantiate (laserBase);
			laser.transform.position = transform.position + transform.forward * (depth + laser.GetComponent<Bullet>().depth);
			laser.transform.rotation = transform.rotation;
			laser.GetComponent<Bullet>().fwd = Forward;
			canFire = false;
			laser.GetComponent<Bullet> ().myController = this;
			shotsFired++;
			Invoke ("ResetCooldown", cooldown);
		}
	}

	protected override void Update ()
	{
		base.Update ();
		lifeTime += Time.deltaTime;
	}
		
	public void TakeDamage(int dmg) {
		hp -= dmg;
		if (hp <= 0) {
			Destroy (gameObject);
		}
	}
	void ResetCooldown() {
		canFire = true;
	}

	public virtual void RegisterHit() {
		numberOfHits++;
	}

	public void LogBulletStat(bool outcome, float targetdist, bool obstacles, int remainingshots)
	{
		using (FileStream fs = new FileStream(fileName,FileMode.Append, FileAccess.Write))
		using (StreamWriter sw = new StreamWriter(fs))
		{
			sw.Write ("{0}", outcome);
			sw.Write (" {0}", targetdist);
			sw.Write (" {0}", obstacles);
			sw.WriteLine (" {0}", remainingshots);
		}	
	}

	public float Accuracy {
		get {
			return ((float)numberOfHits) / shotsFired;
		}
	}

	public float TimeAlive {
		get {
			return lifeTime;
		}
	}

	public float Aggressiveness {
		set
        {
			aggressiveness = value;
		}
        get
        {
            return aggressiveness;
        }
	}

	public bool IsEnemy(string tag) {
		return enemyTags.Contains (tag);
	}

	protected Vector3 StayInArea() {
		if (transform.position.magnitude >= areaSize - 100) {
			return SV_Seek (Vector3.zero) * 100;
		}

		return Vector3.zero;
	}

	void OnTriggerEnter(Collider col) {
		if (!invulnerable && (col.CompareTag ("Obstacle") || col.CompareTag ("Planet"))) {
			Destroy (gameObject);
		}
	}
}
