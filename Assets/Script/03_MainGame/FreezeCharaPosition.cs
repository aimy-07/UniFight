using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCharaPosition : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		transform.localPosition = new Vector3(0, 0, 0);
	}
}
