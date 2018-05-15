using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float flap = 550f;
    public float scroll = 3f;
    float direction = 0f;
	bool isGround = true;
	bool isJoystickUpperd = false;
	public Joystick joystick = null;

	Rigidbody rigid;
	Animator animator;
	GameObject enemyPlayer = null;
	float enemyPos = 0;

	ParticleSystem psSmallLeft;
	ParticleSystem psSmallRight;
	ParticleSystem psBigLeft;
	ParticleSystem psBigRight;
	GameObject psSkill;
	GameObject psJump;
	GameObject psHit;
	ParticleSystem psAPwarning;
	ParticleSystem psHPwarning;
	AudioSource[] audios_SE = new AudioSource[7];

	PhotonView myPhotonView;
	public bool idleFlg;
	public bool walkFlg;
	public bool jumpFlg;
	public bool smallAttackFlg;
	public bool bigAttackFlg;
	public bool skillFlg;
	public bool avoidFlg;
	public bool damageFlg;
	public bool deathFlg;

	public string playerName = "";
	public int chara = -1;
	public int hairColor = -1;
	public int eyeColor = -1;
	public int costumeColor = -1;
	bool isEnemyReady = false;

	public int hp;
	public int ap;
	public float apTimer;


	
	void Start () {
		rigid = GetComponent<Rigidbody>();
		myPhotonView = GetComponent<PhotonView>();

		if (myPhotonView.isMine) {
			gameObject.tag = "myPlayer";
			audios_SE = transform.Find("audio").gameObject.GetComponents<AudioSource>();
			for (int i = 1; i < 7; i++) {
				audios_SE[i].volume = AudioSourceManager.ownCharaVolume;
			}
			// キャラのセット
			playerName = PlayerPrefs.GetString("PlayerName", "No Name (Error)");
			chara = PlayerPrefs.GetInt("chara", 0);
			hairColor = PlayerPrefs.GetInt("hair", 0);
			eyeColor = PlayerPrefs.GetInt("eye", 0);
			costumeColor = PlayerPrefs.GetInt("costume", 0);
			SetChara("myPlayer");
		} else {
			gameObject.tag = "enemyPlayer";
			audios_SE = transform.Find("audio").gameObject.GetComponents<AudioSource>();
			for (int i = 1; i < 7; i++) {
				audios_SE[i].volume = AudioSourceManager.enemyCharaVolume;
			}
		}

		psSmallLeft = transform.Find("ps_SmallLeft").GetComponent<ParticleSystem>();
		psSmallRight = transform.Find("ps_SmallRight").GetComponent<ParticleSystem>();
		psBigLeft = transform.Find("ps_BigLeft").GetComponent<ParticleSystem>();
		psBigRight = transform.Find("ps_BigRight").GetComponent<ParticleSystem>();
		psJump = GameObject.Find("ps_Jump");
		psSkill = GameObject.Find("ps_Skill");
		psHit = GameObject.Find("ps_Hit");
		psAPwarning = GameObject.Find("ps_APwarning").GetComponent<ParticleSystem>();
		psHPwarning = GameObject.Find("ps_HPwarning").GetComponent<ParticleSystem>();

		hp = PhotonManager.MAXHP;
		ap = PhotonManager.MAXAP;
	}

	public void SetChara(string tag) {
		GameObject charaObj = Instantiate(Resources.Load("CharaPrefabs/chara" + chara) as GameObject);
		charaObj.transform.parent = GameObject.FindWithTag(tag).transform;
		charaObj.gameObject.name = "UTC_Default";
		charaObj.transform.localPosition = new Vector3(0, 0, 0);
		charaObj.transform.localRotation = new Quaternion(0, 0, 0, 1);
		animator = charaObj.GetComponent<Animator>();
		charaObj.gameObject.AddComponent<FreezeCharaPosition>();
		charaObj.gameObject.GetComponent<AnimationEventScript>().smallAttackCol = transform.root.gameObject.transform.Find("SmallAttackCollider").gameObject.GetComponent<BoxCollider>();
		charaObj.gameObject.GetComponent<AnimationEventScript>().bigAttackCol = transform.root.gameObject.transform.Find("BigAttackCollider").gameObject.GetComponent<BoxCollider>();

		GameObject charaImgObj = null;
		if (transform.position.x < 0) {
			for (int i = 0; i < SelectCostumeManager.CHARA_MAX; i++) {
				if (i == chara) {
					charaImgObj = GameObject.Find("charaLeft" + i);
				} else {
					Debug.Log(GameObject.Find("charaLeft" + i));
					GameObject.Find("charaLeft" + i).SetActive(false);
				}
			}
			GameObject.Find("PlayerNameLeft").GetComponent<Text>().text = playerName;
		} else {
			for (int i = 0; i < SelectCostumeManager.CHARA_MAX; i++) {
				if (i == chara) {
					charaImgObj = GameObject.Find("charaRight" + i);
				} else {
					GameObject.Find("charaRight" + i).SetActive(false);
				}
			}
			GameObject.Find("PlayerNameRight").GetComponent<Text>().text = playerName;
		}

		if (chara == 0) {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
			charaImgObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
		} else {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
			charaImgObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
		}
		switch(OfflineCharaSet.GetCharaNum(chara)) {
			case 0:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
				charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
				charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
				break;
			case 1:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
				charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
				charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
				break;
			case 2:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
				charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
				charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
				break;
		}

		Debug.Log(tag + " : キャラのセッティングが完了しました");
	}
	
	void Update () {
		if (!isEnemyReady) {
			if (gameObject.tag == "myPlayer") {
				if (GameObject.FindWithTag("enemyPlayer") != null) {
					enemyPlayer = GameObject.FindWithTag("enemyPlayer").gameObject;
					isEnemyReady = true;
				}
			}
		}

		if (hp == 0) {
			GameObject.Find("PhotonManager").SendMessage("BattleEnd", gameObject.tag);
		}
		if (PhotonManager.phase == PhotonManager.PHASE.isEnded) {
			// GetComponent<NetworkCharacter>().enabled = false;
			this.enabled = false;
		}

		if (gameObject.tag == "myPlayer" && PhotonManager.phase == PhotonManager.PHASE.isPlaying) {
			/* ---------------------------------
				HP, AP関係
			---------------------------------- */
			if (apTimer < PhotonManager.AUTE_AP_INTERVAL) {
				apTimer += Time.deltaTime;	
			} else {
				apTimer = 0;
				ap++;
			}
			if (ap > PhotonManager.MAXAP) {
				ap = PhotonManager.MAXAP;
			} else if (ap < 0) {
				ap = 0;
			}

			/* ---------------------------------
				敵のいる位置に合わせて向き変更
			---------------------------------- */
			enemyPos = enemyPlayer.transform.position.x;
			if (Mathf.Abs(transform.position.x - enemyPos) > 0.2f) {
				if (transform.position.x > enemyPos) {
					transform.eulerAngles = new Vector3(0, -90, 0);		
				} else {
					transform.eulerAngles = new Vector3(0, 90, 0);	
				}
			}
			
			/* ---------------------------------
				左右の移動
			---------------------------------- */
			if (joystick.Position.x > 0.1f) {
				if (transform.position.x < 3) {
					if (joystick.Position.x < -0.5f) {
						direction = -1;
					} else if (joystick.Position.x > 0.5f) {
						direction = 1;
					} else {
						direction = joystick.Position.x * 2;
					}
				} else {
					direction = 0;
				}
      		} else if (joystick.Position.x < -0.1f) {
      	    	if (transform.position.x > -3) {
					if (joystick.Position.x < -0.5f) {
						direction = -1;
					} else if (joystick.Position.x > 0.5f) {
						direction = 1;
					} else {
						direction = joystick.Position.x * 2;
					}
				} else {
					direction = 0;
				}
        	} else {
				direction = 0;
			}
        	rigid.velocity = new Vector3(scroll * direction, rigid.velocity.y, 0);

			/* ---------------------------------
				移動アニメーション
			---------------------------------- */
			if (joystick.Position.x < -0.1f || 0.1f < joystick.Position.x) {
				walkFlg = true;
				animator.SetTrigger("Walk");
				if (!audios_SE[0].isPlaying) {
					audios_SE[0].Play();
				}
			} else {
				idleFlg = true;
				animator.SetTrigger("Idle");
				if (audios_SE[0].isPlaying) {
					audios_SE[0].Stop();
				}
			}

			/* ---------------------------------
				ジャンプ
			---------------------------------- */
			if (joystick.Position.y > 0.6f && !isJoystickUpperd) {
				jumpFlg = true;
        	    rigid.AddForce(Vector3.up * flap);
				isJoystickUpperd = true;
				isGround = false;
				psJump.transform.position = new Vector3(transform.position.x, -0.2f, 0);
				psJump.GetComponent<ParticleSystem>().Play();
				audios_SE[1].Play();
        	}
			if (joystick.Position.y < 0.05f && isGround) {
				isJoystickUpperd = false;
			}

			/* ---------------------------------
				弱攻撃
			---------------------------------- */
			if (ButtonScript.smallAttackButtonPressed) {
				if (ap >= PhotonManager.SmallAttackAP) {
					smallAttackFlg = true;
					ap -= PhotonManager.SmallAttackAP;
					animator.SetTrigger("SmallAttack");
					if (transform.position.x > enemyPos) {
						psSmallLeft.Play();
					} else {
						psSmallRight.Play();
					} 
					audios_SE[2].Play();
				} else {
					psAPwarning.Play();
					AudioSourceManager.PlaySE(5);
				}
				ButtonScript.smallAttackButtonPressed = false;
       		}

			/* ---------------------------------
				強攻撃
			---------------------------------- */
			if (ButtonScript.bigAttackButtonPressed) {
				if (ap >= PhotonManager.BigAttackAP) {
					bigAttackFlg = true;
					ap -= PhotonManager.BigAttackAP;
					animator.SetTrigger("BigAttack");
					if (transform.position.x > enemyPos) {
						psBigLeft.Play();
					} else {
						psBigRight.Play();
					} 
					audios_SE[2].Play();
				} else {
					psAPwarning.Play();
					AudioSourceManager.PlaySE(5);
				}
				ButtonScript.bigAttackButtonPressed = false;
        	}

			/* ---------------------------------
				スキル
			---------------------------------- */
			if (ButtonScript.skillButtonPressed) {
				if (hp > PhotonManager.SkillOwnDamage) {
					skillFlg = true;
					hp -= PhotonManager.SkillOwnDamage;
					animator.SetTrigger("Skill");
					psSkill.transform.position = new Vector3(transform.position.x, 0, 0);
					psSkill.GetComponent<ParticleSystem>().Play();
					audios_SE[3].Play();
					audios_SE[4].PlayDelayed(0.5f);
				} else {
					psHPwarning.Play();
					AudioSourceManager.PlaySE(5);
				}
				ButtonScript.skillButtonPressed = false;
       		}

			/* ---------------------------------
				回避
			---------------------------------- */
			if (ButtonScript.avoidButtonPressed) {
				if (ap >= PhotonManager.AvoidAP) {
					avoidFlg = true;
					ap -= PhotonManager.AvoidAP;
					if (transform.position.x > 0) { 
						transform.position = new Vector3(-3, transform.position.y, 0);
						Instantiate(Resources.Load("AvoidEffect/AvoidEffectLeft" + chara) as GameObject, new Vector3(-2, transform.position.y, -1), Quaternion.identity);
					} else {
						transform.position = new Vector3(3, transform.position.y, 0);
						Instantiate(Resources.Load("AvoidEffect/AvoidEffectRight" + chara) as GameObject, new Vector3(2, transform.position.y, -1), Quaternion.identity);
					}
					audios_SE[5].Play();
				} else {
					psAPwarning.Play();
					AudioSourceManager.PlaySE(5);
				}
				ButtonScript.avoidButtonPressed = false;
        	}
		}
	}

	void OnCollisionEnter(Collision c) {
		if (c.gameObject.tag == "floor") {
			isGround = true;
		}
	}

	void OnCollisionExit(Collision c) {
		if (c.gameObject.tag == "floor") {
			isGround = false;
		}
	}

	void Damaged(int damage) {
		if (gameObject.tag == "myPlayer") {
			hp -= damage;
			if (damage != 4) {
				ap += damage;
			}
			psHit.transform.position = new Vector3(transform.position.x, 1.5f, -3);
			psHit.GetComponent<ParticleSystem>().Play();
			audios_SE[6].Play();
			if (hp > 0) {
				damageFlg = true;
				animator.SetTrigger("Damage");
			} else {
				hp = 0;
				deathFlg = true;
				animator.SetTrigger("Death");
			}
		}
	}
}
