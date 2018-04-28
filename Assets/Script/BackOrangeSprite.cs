using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackOrangeSprite : MonoBehaviour {

	public float animationSpeed;

	void Start () {
		
	}
	
	void Update () {
		transform.Translate(-animationSpeed * Time.deltaTime, 0, 0);
		if (transform.position.x < -1f) {
			transform.position = new Vector3(1.05f, -0.1f, 10);
		}
	}
}
