using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour {

    public abstract void hit(float damage);
    public abstract void tick();

    public abstract bool isTraversible(Vector2 position);

    public float maxHealth;
    public float curHealth;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
