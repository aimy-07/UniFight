using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAttack : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void OnTriggerEnter(Collider c) {
		if (transform.root.gameObject.tag == "myPlayer") {
			if (GameObject.FindWithTag("enemyPlayer").gameObject != null && c.gameObject.tag == "enemyPlayer") {
				Debug.Log("弱攻撃 成功!!");
			}
		} else {
			if (GameObject.FindWithTag("myPlayer").gameObject != null && c.gameObject.tag == "myPlayer") {
				Debug.Log("ダメージ！弱攻撃!!");
				GameObject.FindWithTag("myPlayer").gameObject.SendMessage("Damaged", PhotonManager.smallAttackDamage);
			}
		}
	}
}
