using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[RequireComponent(typeof(MeshCollider))]

public class SteeringVehicle : MonoBehaviour {

    public float wanderOffset = 5;
    public float wanderRadius = 2;
    public float fleeDistance = 100;
    protected MovementStatus moveStatus = MovementStatus.Idle;
    public float mass = 1.0f;
    public float maxForce = 100;
    public float maxSpeed = 100;
    private Vector3 velocity;
    private float wanderAng = Mathf.PI;
    public float wanderAngSpeed = Mathf.PI / 2;
    public float slowingDistance = 10;
    public float obstacleAvoidanceDistance = 10;
	protected List<SteeringVehicle> flock;
	public float flockRadius = 300;
	public float separation = 1.0f;
	public float separationDistance = 50;
	public float cohesion = 1.0f;
	public float alignmnet = 1.0f;
	protected float yAngle = 0;

    protected GameObject target = null;
    protected Vector3 targetPos;// = null;

	protected Renderer myRenderer;
	private Vector3 lastFwd = new Vector3(0, 0, 1);
	// Use this for initialization
	void Start () 
	{
		myRenderer = GetComponent<Renderer> ();
		yAngle = Mathf.Atan2 (Forward.x, Forward.z);
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		CalcMoveState ();
		if (moveStatus != MovementStatus.Idle) {
			Vector3 steeringForce = CalcSteeringForce ();
			if (steeringForce.sqrMagnitude > maxForce * maxForce) {
				steeringForce = steeringForce.normalized * maxForce;
			}
			Vector3 acceleration = steeringForce / mass;
			acceleration = new Vector3 (acceleration.x, 0, acceleration.z);
			velocity = velocity + acceleration * Time.deltaTime;
			if (velocity.sqrMagnitude > maxSpeed * maxSpeed) {
				velocity = velocity.normalized * maxSpeed;
			}
			if (velocity != Vector3.zero) {
				float newYA = Mathf.Atan2 (Forward.x, Forward.z);
				transform.Rotate(Vector3.up, (newYA - yAngle) * 180 / Mathf.PI);
				yAngle = newYA;
				transform.position += velocity * Time.deltaTime;
			}
		}
        
	}

    protected virtual Vector3 CalcSteeringForce()
    {
        return SV_Wander();
    }
	protected virtual Vector3 SV_Flock() {
		CreateFlock ();
		return (SV_Alignment () * alignmnet + SV_Cohesion () * cohesion + SV_Separation () * separation) * maxForce;
	}

	protected virtual void CreateFlock() {
	}

	protected Vector3 SV_Alignment() {
		Vector3 al = Vector3.zero;
		int numNeighbors = flock.Count;

		if (numNeighbors > 0) {
			return al;
		}
		foreach (SteeringVehicle sv in flock) {
			al += sv.Forward;
		}
			
		return AlignTo (al / numNeighbors);
	}
	protected Vector3 SV_Cohesion() {
		Vector3 al = transform.position;
		int numNeighbors = flock.Count;

		if (numNeighbors == 0) {
			return Vector3.zero;
		}
		foreach (SteeringVehicle sv in flock) {
			al += sv.transform.position;
		}

		return SV_Seek(al / numNeighbors);
	}

	protected Vector3 SV_Separation() {
		Vector3 al = Forward;
		int numFleeing = 0;
		if (flock.Count == 0) {
			return al;
		}
	
		foreach (SteeringVehicle sv in flock) {
			
			if (Vector3.Distance (transform.position, sv.transform.position) <= separationDistance) {
				al += SV_Flee (sv.transform.position);
				numFleeing++;
			}
		}
			
		return AlignTo (al);
	}
		
    public Vector3 SV_Seek(Vector3 target)
    {
        Vector3 desired_velocity = (target - transform.position).normalized * maxSpeed;
        return desired_velocity - velocity;
    }

	protected Vector3 AlignTo(Vector3 direction) {
		Vector3 dir = direction.normalized;
		dir.y = 0;
		return dir * maxSpeed - velocity;
	}

    public Vector3 SV_Seek(GameObject go)
    {
        return SV_Seek(go.transform.position);
    }

	public Vector3 SV_Pursue(SteeringVehicle sv) {
		return SV_Seek (sv.transform.position + sv.velocity);
	}

    public Vector3 SV_Flee(Vector3 target)
    {
        Vector3 desired_velocity = (transform.position - target).normalized * maxSpeed;
        return desired_velocity - velocity;
    }

    public Vector3 SV_Flee(GameObject go)
    {
        return SV_Flee(go.transform.position);
    }

	public Vector3 SV_Evade(SteeringVehicle sv) {
		return SV_Flee (sv.transform.position + sv.velocity);
	}

    public Vector3 SV_Wander()
    {
        Vector3 offset = transform.position + (transform.forward * wanderOffset);

        float ang = Random.Range(0, wanderAngSpeed * 2) - wanderAngSpeed;
        wanderAng += ang;
        Vector3 circOff = new Vector3(Mathf.Cos(wanderAng), 0, Mathf.Sin(wanderAng)) * wanderRadius;
        
        return SV_Seek(offset + circOff);
    }
    
    public Vector3 SV_Arrive(Vector3 target)
    {
        Vector3 offset = target - transform.position;
        float distance = Vector3.Distance(target, transform.position);
      
        float ramped_speed = maxSpeed * (distance / slowingDistance);
        float clipped_speed = Mathf.Min(ramped_speed, maxSpeed);
        Vector3 desired_velocity = (clipped_speed / distance) * offset;
        return desired_velocity - velocity;
    }

	public virtual Vector3 SV_Avoid_Obstacle(GameObject obstacle) {
		Vector3 fwd = transform.forward.normalized;
		float dot = Vector3.Dot (fwd, obstacle.transform.position - transform.position);
        if (dot > 0 && dot < obstacleAvoidanceDistance)
        {
            return SV_Flee(obstacle);
        }
        return Vector3.zero;
	}

    public virtual Vector3 AvoidObstacles()
    {
        List<GameObject> obstacles = GameObject.FindGameObjectsWithTag("Obstacle").ToList();
        obstacles.AddRange(GameObject.FindGameObjectsWithTag("Planet").ToList());
        Vector3 steerForce = Vector3.zero;
        foreach (GameObject go in obstacles)
        {
            steerForce += SV_Avoid_Obstacle(go);
        }
        return steerForce;
    }

	public GameObject GetRandomFromList(List<GameObject> goList) {
		return goList [Random.Range (0, goList.Count - 1)];
	}

	protected virtual void CalcMoveState() {
	}
	public void Seek() {
		moveStatus = MovementStatus.Seek;
		myRenderer.material.color = Color.red;
	}

    public void Seek(GameObject go)
    {
        Seek();
        target = go;
    }

    public void Seek(Vector3 pos)
    {
        Seek();
        target = null;
        targetPos = pos;
    }

	public void AlertSeek(GameObject go) {
		target = go;
		targetPos = Vector3.zero;
		myRenderer.material.color = Color.red;
		moveStatus = MovementStatus.FlockSeek;
	}
	public void Flee() {
		moveStatus = MovementStatus.Flee;
		myRenderer.material.color = Color.blue;
	}

    public void Flee(GameObject go)
    {
        Flee();
        target = go;
    }

    public void Flee(Vector3 pos)
    {
        Flee();
        target = null;
        targetPos = pos;
    }

	public void AlertFlee(GameObject go) {
		target = go;
		targetPos = Vector3.zero;
		moveStatus = MovementStatus.FlockFlee;
		myRenderer.material.color = Color.blue;
	}

    public void Idle() {
		moveStatus = MovementStatus.Idle;
		myRenderer.material.color = Color.yellow;
	}

	public void Wander() {
		moveStatus = MovementStatus.Wander;
		myRenderer.material.color = Color.green;
	}

	public void Flock() {
		moveStatus = MovementStatus.Flock;
		myRenderer.material.color = Color.white;
	}

	protected virtual void AlertFlock(GameObject tar, bool seek) {
	}

	public Vector3 Forward {
		get {
			Vector3 fwd = velocity.normalized;

			if (fwd.magnitude != 0) {
				lastFwd = fwd;
			}
			return lastFwd;
		}
	}
}

public enum MovementStatus
{
    Idle, Wander, Seek, Arrive, Pursue, Flee, Evade, Flock, FlockSeek, FlockFlee
}
	