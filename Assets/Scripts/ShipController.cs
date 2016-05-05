using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ShipController : SteeringVehicle {
	public GameObject laserBase;
	private int hp = 20;
	public float fireDistance = 25f;
	protected bool canFire = true;
	protected float cooldown = 1.0f;
	protected float depth = 10;
	protected int shotsFired = 0;
	private int numberOfHits = 0;
	protected float lifeTime = 0.0f;
	protected float aggressiveness = 1.0f;
	protected List<string> enemyTags = new List<string>();
	void Start() {
		lifeTime = 0.0f;
		depth = GetComponent<MeshRenderer> ().bounds.size.z;
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

	public void RegisterHit() {
		numberOfHits++;
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
		set {
			aggressiveness = value;
		}
	}

	public bool IsEnemy(string tag) {
		return enemyTags.Contains (tag);
	}
}
