using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehavior : MonoBehaviour
{
    public float speed = 1f;
	
	private Manager manager;
	private Rigidbody rb;
	
	void Awake() {
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		rb = GetComponent<Rigidbody>();
	}

    void Update()
    {
        rb.MovePosition(rb.position+new Vector3(speed*Time.deltaTime,0,0));
    }
	
	void OnTriggerEnter(Collider collision) {
		if (collision.gameObject.name == "Player") {
			manager.TakeDamage(1);
			Destroy(gameObject);
		}
	}
}
