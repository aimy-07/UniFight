using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflinePlayerController : MonoBehaviour {

	public float flap = 550f;
    public float scroll = 3f;
    float direction = 0f;
	public bool isGround = true;

	Rigidbody rigid;
	Animator animator;
	GameObject enemyPlayer;
	float enemyPos = 0;
	int chara;
	[SerializeField] OfflineEnemyController offlineEnemyController;

	[SerializeField] ParticleSystem psSmallLeft;
	[SerializeField] ParticleSystem psSmallRight;
	[SerializeField] ParticleSystem psBigLeft;
	[SerializeField] ParticleSystem psBigRight;
	[SerializeField] GameObject psSkill;
	[SerializeField] GameObject psJump;
	AudioSource[] audios_SE = new AudioSource[7];

	public int hp;
	[SerializeField] RectTransform hpbarRect;
	public int ap;
	[SerializeField] RectTransform apbarRect;
	

	void Start () {
		rigid = GetComponent<Rigidbody>();
		animator = transform.Find("UTC_Default").gameObject.GetComponent<Animator>();
		enemyPlayer = GameObject.Find("enemyPlayer");
		chara = PlayerPrefs.GetInt("chara", 0);

		audios_SE = transform.Find("audio").gameObject.GetComponents<AudioSource>();
		// for (int i = 0; i < 7; i++) {
		// 	audios_SE[i].volume = 1.0f;
		// }

		hp = PhotonManager.MAXHP;
		ap = PhotonManager.MAXAP;
	}
	
	void Update () {
		if (OfflineManager.isPlaying) {
			/* ---------------------------------
				HP, AP関係
			---------------------------------- */
			if (hp == 0) {
				GameObject.Find("OfflineManager").SendMessage("BattleEnd", gameObject.tag);
			}
			if (OfflineManager.apPlus) {
				ap++;
			}
			if (ap > PhotonManager.MAXAP) {
				ap = PhotonManager.MAXAP;
			} else if (ap < 0) {
				ap = 0;
			}
			hpbarRect.localScale = new Vector3((float)hp / PhotonManager.MAXHP, 1, 1);
			apbarRect.localScale = new Vector3((float)ap / PhotonManager.MAXAP, 1, 1);

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
			if (ButtonScript.rightButtonPressing) {
				if (transform.position.x < 3) {
					direction = 1f;
				} else {
					direction = 0;
				}
      		} else if (ButtonScript.leftButtonPressing) {
      	    	if (transform.position.x > -3) {
					direction = -1f;
				} else {
					direction = 0;
				}
        	} else {
        	    direction = 0f;
        	}
        	rigid.velocity = new Vector3(scroll * direction, rigid.velocity.y, 0);

			/* ---------------------------------
				移動アニメーション
			---------------------------------- */
			if (ButtonScript.leftButtonPressing || ButtonScript.rightButtonPressing) {
				animator.SetTrigger("Walk");
				if (!audios_SE[0].isPlaying) {
					audios_SE[0].Play();
				}
			} else {
				animator.SetTrigger("Idle");
				if (audios_SE[0].isPlaying) {
					audios_SE[0].Stop();
				}
			}

			/* ---------------------------------
				ジャンプ
			---------------------------------- */
			ButtonScript.isGround = isGround;
			if (ButtonScript.upButtonPressing && isGround) {
        	    rigid.AddForce(Vector3.up * flap);
				isGround = false;
				psJump.transform.position = new Vector3(transform.position.x, -0.2f, 0);
				psJump.GetComponent<ParticleSystem>().Play();
				audios_SE[1].Play();
        	}

			/* ---------------------------------
				急降下
			---------------------------------- */
			if (ButtonScript.downButtonPressed && !isGround) {
        	    rigid.AddForce(Vector3.down * flap);
				animator.SetTrigger("JumpDown");
				ButtonScript.downButtonPressed = false;
        	}

			/* ---------------------------------
				弱攻撃
			---------------------------------- */
			if (ButtonScript.smallAttackButtonPressed) {
				if (ap >= PhotonManager.SmallAttackAP) {
					ap -= PhotonManager.SmallAttackAP;
					animator.SetTrigger("SmallAttack");
					if (transform.position.x > enemyPos) {
						psSmallLeft.Play();
					} else {
						psSmallRight.Play();
					} 
					audios_SE[2].Play();
					offlineEnemyController.myAttacked = true;
				} else {
					// AP不足
				}
				ButtonScript.smallAttackButtonPressed = false;
       		}

			/* ---------------------------------
				強攻撃
			---------------------------------- */
			if (ButtonScript.bigAttackButtonPressed) {
				if (ap >= PhotonManager.BigAttackAP) {
					ap -= PhotonManager.BigAttackAP;
					animator.SetTrigger("BigAttack");
					if (transform.position.x > enemyPos) {
						psBigLeft.Play();
					} else {
						psBigRight.Play();
					} 
					audios_SE[2].Play();
					offlineEnemyController.myAttacked = true;
				} else {
					// AP不足
				}
				ButtonScript.bigAttackButtonPressed = false;
        	}

			/* ---------------------------------
				スキル
			---------------------------------- */
			if (ButtonScript.skillButtonPressed) {
				if (hp > PhotonManager.SkillOwnDamage) {
					hp -= PhotonManager.SkillOwnDamage;
					animator.SetTrigger("Skill");
					psSkill.transform.position = new Vector3(transform.position.x, 0, 0);
					psSkill.GetComponent<ParticleSystem>().Play();
					audios_SE[3].Play();
					audios_SE[4].Play(44100 / 2);
					offlineEnemyController.myAttacked = true;
				} else {
					// HP不足
				}
				ButtonScript.skillButtonPressed = false;
       		}

			/* ---------------------------------
				回避
			---------------------------------- */
			if (ButtonScript.avoidButtonPressed) {
				if (ap >= PhotonManager.AvoidAP) {
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
					// AP不足
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
		hp -= damage;
		if (hp > 0) {
			animator.SetTrigger("Damage");
		} else {
			hp = 0;
			animator.SetTrigger("Death");
		}
	}
}
