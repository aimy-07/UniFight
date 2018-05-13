using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmallAttack : MonoBehaviour {

	GameObject myPlayer;
	GameObject enemyPlayer;


	void Start () {
		if (SceneManager.GetActiveScene().name == "OfflineMain") {
			myPlayer = GameObject.FindWithTag("myPlayer").gameObject;
			enemyPlayer = GameObject.FindWithTag("enemyPlayer").gameObject;
		}
	}
	
	void Update () {
		if (SceneManager.GetActiveScene().name == "Main") {
			myPlayer = GameObject.FindWithTag("myPlayer").gameObject;
		}
	}

	public void OnTriggerEnter(Collider c) {
		if (PhotonManager.phase == PhotonManager.PHASE.isPlaying) {
			// 通信対戦モード
			if (transform.root.gameObject.tag == "myPlayer") {
				if (c.gameObject.tag == "enemyPlayer") {
					Debug.Log("弱攻撃 成功!!");
				}
			} else {
				if (c.gameObject.tag == "myPlayer") {
					Debug.Log("ダメージ！弱攻撃!!");
					myPlayer.SendMessage("Damaged", PhotonManager.smallAttackDamage);
				}
			}
		} else if (OfflineManager.isPlaying) {
			// シングルプレイモード
			if (transform.root.gameObject.tag == "myPlayer") {
				if (c.gameObject.tag == "enemyPlayer") {
					Debug.Log("弱攻撃 成功!!");
					enemyPlayer.SendMessage("Damaged", PhotonManager.smallAttackDamage);
				}
			} else {
				if (c.gameObject.tag == "myPlayer") {
					Debug.Log("ダメージ！弱攻撃!!");
					myPlayer.SendMessage("Damaged", PhotonManager.smallAttackDamage);
				}
			}
		}
	}
}
