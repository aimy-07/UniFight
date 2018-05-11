using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkCharacter : Photon.MonoBehaviour {

	float myPos = 0;
	Vector3 correctPlayerPos;
    Quaternion correctPlayerRot;

	ParticleSystem psSmallLeft;
	ParticleSystem psSmallRight;
	ParticleSystem psBigLeft;
	ParticleSystem psBigRight;
	GameObject psSkill;
	GameObject psJump;
	GameObject psHit;
	AudioSource[] audios_SE = new AudioSource[7];



	void Start() {
		psSmallLeft = transform.Find("ps_SmallLeft").GetComponent<ParticleSystem>();
		psSmallRight = transform.Find("ps_SmallRight").GetComponent<ParticleSystem>();
		psBigLeft = transform.Find("ps_BigLeft").GetComponent<ParticleSystem>();
		psBigRight = transform.Find("ps_BigRight").GetComponent<ParticleSystem>();
		psJump = GameObject.Find("ps_Jump");
		psSkill = GameObject.Find("ps_Skill");
		psHit = GameObject.Find("ps_Hit");
		audios_SE = transform.Find("audio").gameObject.GetComponents<AudioSource>();
		// for (int i = 0; i < 7; i++) {
		// 	audios_SE[i].volume = 1.0f;
		// }
	}
 
	void Update() {
		if (gameObject.tag == "enemyPlayer") {
			myPos = GameObject.FindWithTag("myPlayer").gameObject.transform.position.x;
			transform.position = Vector3.Lerp (transform.position, this.correctPlayerPos, 0.5f);
            transform.rotation = Quaternion.Lerp (transform.rotation, this.correctPlayerRot, 0.5f);
		}
	}
 
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			PlayerController playerController = GetComponent<PlayerController>();

			//キャラの見た目とプレイヤー名
			stream.SendNext(playerController.playerName);
			stream.SendNext(playerController.chara);
			stream.SendNext(playerController.hairColor);
			stream.SendNext(playerController.eyeColor);
			stream.SendNext(playerController.costumeColor);
			//位置と回転
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(GetComponent<Rigidbody>().velocity);
			//HPとAP
			stream.SendNext(playerController.hp);
			stream.SendNext(playerController.ap);
			
			//アニメーション
			stream.SendNext(playerController.idleFlg);
			stream.SendNext(playerController.walkFlg);
			stream.SendNext(playerController.jumpFlg);
			stream.SendNext(playerController.smallAttackFlg);
			stream.SendNext(playerController.bigAttackFlg);
			stream.SendNext(playerController.skillFlg);
			stream.SendNext(playerController.avoidFlg);
			stream.SendNext(playerController.damageFlg);
			stream.SendNext(playerController.deathFlg);
			playerController.idleFlg = false;
			playerController.walkFlg = false;
			playerController.jumpFlg = false;
			playerController.smallAttackFlg = false;
			playerController.bigAttackFlg = false;
			playerController.skillFlg = false;
			playerController.avoidFlg = false;
			playerController.damageFlg = false;
			playerController.deathFlg = false;

		} else {
			Debug.Log("Receivred : " + gameObject.tag);
			PlayerController playerController = GetComponent<PlayerController>();

			//キャラの見た目とプレイヤー名
			playerController.playerName = (string)stream.ReceiveNext();
			int chara = (int)stream.ReceiveNext();
			playerController.chara = chara;
			playerController.hairColor = (int)stream.ReceiveNext();
			playerController.eyeColor = (int)stream.ReceiveNext();
			playerController.costumeColor = (int)stream.ReceiveNext();
			//位置と回転
			// transform.position = (Vector3)stream.ReceiveNext();
            // transform.rotation = (Quaternion)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext ();
            correctPlayerRot = (Quaternion)stream.ReceiveNext ();
			GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();
			//HPとAP
			playerController.hp = (int)stream.ReceiveNext();
			playerController.ap = (int)stream.ReceiveNext();
			
			//アニメーション
			Animator animator = transform.Find("UTC_Default").gameObject.GetComponent<Animator>();
			bool idleFlg = (bool)stream.ReceiveNext();
			bool walkFlg = (bool)stream.ReceiveNext();
			bool jumpFlg = (bool)stream.ReceiveNext();
			bool smallAttackFlg = (bool)stream.ReceiveNext();
			bool bigAttackFlg = (bool)stream.ReceiveNext();
			bool skillFlg = (bool)stream.ReceiveNext();
			bool avoidFlg = (bool)stream.ReceiveNext();
			bool damageFlg = (bool)stream.ReceiveNext();
			bool deathFlg = (bool)stream.ReceiveNext();
			if (idleFlg) {
        		animator.SetTrigger("Idle");
      		}
			if (walkFlg) {
        		animator.SetTrigger("Walk");
      		}
			if (jumpFlg) {
        		psJump.transform.position = new Vector3(transform.position.x, -0.2f, 0);
				psJump.GetComponent<ParticleSystem>().Play();
				audios_SE[1].Play();
      		}
      		if (smallAttackFlg) {
        		animator.SetTrigger("SmallAttack");
				if (transform.position.x > myPos) {
					psSmallLeft.Play();
				} else {
					psSmallRight.Play();
				} 
				audios_SE[2].Play();
      		}
			if (bigAttackFlg) {
        		animator.SetTrigger("BigAttack");
				if (transform.position.x > myPos) {
					psBigLeft.Play();
				} else {
					psBigRight.Play();
				} 
				audios_SE[2].Play();
      		}
			if (skillFlg) {
        		animator.SetTrigger("Skill");
				psSkill.transform.position = new Vector3(transform.position.x, 0, 0);
				psSkill.GetComponent<ParticleSystem>().Play();
				audios_SE[3].Play();
				audios_SE[4].PlayDelayed(0.5f);
      		}
			if (avoidFlg) {
        		if (transform.position.x > 0) { 
					transform.position = new Vector3(-3, transform.position.y, 0);
					Instantiate(Resources.Load("AvoidEffect/AvoidEffectLeft" + chara) as GameObject, new Vector3(-2, transform.position.y, -1), Quaternion.identity);
				} else {
					transform.position = new Vector3(3, transform.position.y, 0);
					Instantiate(Resources.Load("AvoidEffect/AvoidEffectRight" + chara) as GameObject, new Vector3(2, transform.position.y, -1), Quaternion.identity);
				}
				audios_SE[5].Play();
      		}
			if (damageFlg) {
				psHit.transform.position = new Vector3(transform.position.x, 1.5f, -3);
				psHit.GetComponent<ParticleSystem>().Play();
				audios_SE[6].Play();
        		animator.SetTrigger("Damage");
      		}
			if (deathFlg) {
				psHit.transform.position = new Vector3(transform.position.x, 1.5f, -3);
				psHit.GetComponent<ParticleSystem>().Play();
        		animator.SetTrigger("Death");
			}
		}
	}
}
