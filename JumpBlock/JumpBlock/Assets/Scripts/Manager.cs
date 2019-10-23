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
	private TextMeshProUGUI score;
	private TextMeshProUGUI hp;
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
		hp = GameObject.Find("Canvas").transform.Find("Health").gameObject.GetComponent<TextMeshProUGUI>();
		player = GameObject.Find("Player");
		playerrb = player.GetComponent<Rigidbody>();
        enemyList = new List<GameObject>();
		level = 1;
		totalEnemyCount = 0;
		enemySpeed = 4f;
		spawnInterval = 3f;
		timeTick = 0f;
    }

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space)) {
			PlayerJump();
		}
		else {
			ApplyPlayerGravity();
		}
		if (damageTick > 0f) {
			damageTick -= Time.deltaTime;
		}
		else {
			damageTick = 0f;
		}
		if (damageTick%2 != 0) {
			
		}
		else {
			
		}
		if (health <= 0) {
			health = 0;
			GameOver();
		}
        if (timeTick >= spawnInterval) {
			timeTick = 0f;
			AddEnemy();
			if (totalEnemyCount%5 == 0 && level < 100) {
				level += 1;
				enemySpeed += 0.05f;
				spawnInterval = spawnInterval <= 0.5f ? 0.5f : spawnInterval-0.05f;
			}
			spawnInterval += Random.Range(0,2);
		}
		timeTick += Time.deltaTime;
		hp.text = "Health: "+health;
		score.text = "Level: "+level;
    }
	
	void ApplyPlayerGravity() {
		playerrb.velocity = playerrb.velocity + new Vector3(0,gravity*Time.deltaTime,0);
	}
	
	void PlayerJump() {
		if (playerrb && playerrb.velocity.y == 0) {
			playerrb.velocity = new Vector3(0,jump,0);
		}
	}
	
	public void TakeDamage(int amount) {
		ShakeCamera();
		health -= amount;
	}
	
	void ShakeCamera() {
		
	}
	
	void AddEnemy() {
		if (enemyPrefab) {
			totalEnemyCount += 1;
			GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(-20,0.5f,0), Quaternion.identity);
			enemyList.Add(newEnemy);
			CubeBehavior enemyScript = newEnemy.GetComponent<CubeBehavior>();
			if (enemyScript) {
				enemyScript.speed = enemySpeed + Random.Range(-1f, 0.5f);
			}
			Destroy(newEnemy, 10f-(enemySpeed/5f));
		}
	}
	
	void GameOver() {
		SceneManager.LoadScene("Scene");
	}
}
