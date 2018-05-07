using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAttack : MonoBehaviour {

	GameObject myPlayer;
	GameObject enemyPlayer;
	AudioSource myDamageSound;
	AudioSource enemyDamageSound;

	void Start () {
		if (PhotonManager.phase == PhotonManager.PHASE.other) {
			myPlayer = GameObject.FindWithTag("myPlayer").gameObject;
			enemyPlayer = GameObject.FindWithTag("enemyPlayer").gameObject;
			myDamageSound = myPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
			enemyDamageSound = enemyPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
		}
	}
	
	void Update () {
		if (PhotonManager.phase != PhotonManager.PHASE.other) {
			myPlayer = GameObject.FindWithTag("myPlayer").gameObject;
			myDamageSound = myPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
		}
	}

	public void OnTriggerEnter(Collider c) {
		if (PhotonManager.phase == PhotonManager.PHASE.isPlaying) {
			// 通信対戦モード
			if (transform.root.gameObject.tag == "myPlayer") {
				if (c.gameObject.tag == "enemyPlayer") {
					Debug.Log("強攻撃 成功!!");
				}
			} else {
				if (c.gameObject.tag == "myPlayer") {
					Debug.Log("ダメージ！強攻撃!!");
					myDamageSound.Play();
					myPlayer.SendMessage("Damaged", PhotonManager.bigAttackDamage);
				}
			}
		} else if (OfflineManager.isPlaying) {
			// シングルプレイモード
			if (transform.root.gameObject.tag == "myPlayer") {
				if (c.gameObject.tag == "enemyPlayer") {
					Debug.Log("強攻撃 成功!!");
					enemyDamageSound.Play();
					enemyPlayer.SendMessage("Damaged", PhotonManager.bigAttackDamage);
				}
			} else {
				if (c.gameObject.tag == "myPlayer") {
					Debug.Log("ダメージ！強攻撃!!");
					myDamageSound.Play();
					myPlayer.SendMessage("Damaged", PhotonManager.bigAttackDamage);
				}
			}
		}
	}
}
