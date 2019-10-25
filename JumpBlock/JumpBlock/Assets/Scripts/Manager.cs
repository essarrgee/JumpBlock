using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
	private GameObject player;
	private Rigidbody playerrb;
	private GameObject playermodel;
	private Animator playeranimator;
	private TextMeshProUGUI score;
	private Animator levelAnimator;
	private GameObject hpCanvas;
	private GameObject[] hpCount;
	private Camera cam;
	private bool camShaking = false;
	
	public GameObject enemyPrefab;
	private List<GameObject> enemyList;
	
	public int health = 3;
	public float jump = 15f;
	public float gravity = -9.8f;
	private float damageTick = 0f;
	
	private int level;
	private int totalEnemyCount;
	private float enemySpeed;
	private float spawnInterval;
	private float timeTick;
    
    void Awake()
    {
		cam = Camera.main;
		score = GameObject.Find("Canvas").transform.Find("Level").gameObject.GetComponent<TextMeshProUGUI>();
		levelAnimator = GameObject.Find("Canvas").transform.Find("Level").gameObject.GetComponent<Animator>();
		hpCanvas = GameObject.Find("Canvas").transform.Find("Health").gameObject;
		hpCount = new GameObject[3];
		for (int i=0; i<hpCount.Length; i++) {
			hpCount[i] = hpCanvas.transform.Find("Heart"+(i+1)).gameObject;
		}
		player = GameObject.Find("Player");
		playerrb = player.GetComponent<Rigidbody>();
		playermodel = player.transform.Find("Model").gameObject;
		if (playermodel) {
			playeranimator = playermodel.GetComponent<Animator>();
		}
        enemyList = new List<GameObject>();
		level = 1;
		totalEnemyCount = 0;
		enemySpeed = 6f;
		spawnInterval = 3f;
		timeTick = 0f;
    }

    void Update()
    {
		CheckHPCanvas();
		CheckDamageTick();
		if (health <= 0) {
			health = 0;
			GameOver();
		}
		PlayerGrounded();
		if (Input.GetKeyDown(KeyCode.Space)) {
			PlayerJump();
		}
		else {
			ApplyPlayerGravity();
		}
        if (timeTick >= spawnInterval) {
			timeTick = 0f;
			AddEnemy();
			if (totalEnemyCount%5 == 0 && level < 100) {
				LevelUp();
				enemySpeed += 0.05f;
				spawnInterval = spawnInterval <= 0.5f ? 0.5f : spawnInterval-0.05f;
			}
			spawnInterval += Random.Range(-0.1f,1);
		}
		timeTick += Time.deltaTime;
		score.text = "Level: "+level;
    }
	
	void LevelUp() {
		level += 1;
		//levelAnimator.Play("LevelUp");
		levelAnimator.SetTrigger("LeveledUp");
	}
	
	void CheckHPCanvas() {
		if (hpCanvas) {
			switch (health) {
				case 3:
					hpCount[0].SetActive(true);
					hpCount[1].SetActive(true);
					hpCount[2].SetActive(true);
					break;
				case 2:
					hpCount[0].SetActive(false);
					hpCount[1].SetActive(true);
					hpCount[2].SetActive(true);
					break;
				case 1:
					hpCount[0].SetActive(false);
					hpCount[1].SetActive(false);
					hpCount[2].SetActive(true);
					break;
				default:
					hpCount[0].SetActive(false);
					hpCount[1].SetActive(false);
					hpCount[2].SetActive(false);
					break;
			}
		}
	}
	
	void CheckDamageTick() {
		if (damageTick > 0f) {
			damageTick -= Time.deltaTime;
		}
		else {
			damageTick = 0f;
		}
		if (Mathf.Round((damageTick*10)%2) == 1f) {
			playermodel.transform.localScale = new Vector3(0,0,0);
		}
		else {
			playermodel.transform.localScale = new Vector3(5,5,5);
		}
	}
	
	void ApplyPlayerGravity() {
		playerrb.velocity = playerrb.velocity + new Vector3(0,gravity*Time.deltaTime,0);
	}
	
	void PlayerGrounded() {
		if (playerrb && playerrb.velocity.y == 0f) {
			playeranimator.SetBool("Jump", false);
		}
	}
	
	void PlayerJump() {
		if (playerrb && playerrb.velocity.y <= 0.02f && playerrb.velocity.y >= -0.02f) {
			playeranimator.SetBool("Jump", true);
			playerrb.velocity = new Vector3(0,jump,0);
		}
	}
	
	public void TakeDamage(int amount) {
		if (damageTick <= 0f) {
			StartCoroutine("ShakeCamera");
			damageTick = 2f;
			health -= amount;
		}
	}
	
	IEnumerator ShakeCamera() {
		Quaternion orientation = cam.transform.rotation;
		for (int i=0; i<10; i++) {
			int shake = (i%2 == 0) ? 1 : -1;
			cam.transform.eulerAngles += new Vector3(orientation.x+Random.Range(0.5f-(i/10),1f-(i/10))*shake,0f,0f);
			yield return new WaitForSeconds(0.02f);
		}
		cam.transform.rotation = orientation;
	}
	
	void AddEnemy() {
		if (enemyPrefab) {
			totalEnemyCount += 1;
			GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(-30,0.5f,0), Quaternion.identity);
			enemyList.Add(newEnemy);
			CubeBehavior enemyScript = newEnemy.GetComponent<CubeBehavior>();
			if (enemyScript) {
				enemyScript.speed = enemySpeed + Random.Range(-1f, 0.5f);
			}
			Destroy(newEnemy, 18f-(enemySpeed/5f));
		}
	}
	
	void GameOver() {
		SceneManager.LoadScene("Scene");
	}
}
