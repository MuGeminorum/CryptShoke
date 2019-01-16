using UnityEngine;
using System.Collections;

public class RayAndHit {
	public Ray ray;
	public RaycastHit hit;
	public RayAndHit(Ray ray, RaycastHit hit) {
		this.ray = ray;
		this.hit = hit;
	}
}

public class HealthController : MonoBehaviour {
	
	public GameObject deathHandler;
	public GameObject player;
	public float maxHealth = 100;
	public float hitDamage = 3;
	public float healingSpeed = 2;
	public GameObject hitParticles;
	public AudioClip hitSound;
	//[HideInInspector]
	public float health;
	public bool isPoison = false;
	public float normalizedHealth { get { return health / maxHealth; } }
	[HideInInspector]
	public float normalHitDamage = 3;
	public GameObject controller;

	public int a = 0;
	public float poisonDamage = 0.5f;

	private HealthSlide healthSlide;
	private ProgressBar progressBar;
	private HumanStatus humanStatus;
	private FreeAI freeAI;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		controller = GameObject.Find("Controller");
		if (gameObject.tag == "Enermies"){
			healthSlide = gameObject.GetComponent<HealthSlide>();
			humanStatus = player.GetComponent<HumanStatus>();
			freeAI = gameObject.GetComponent<FreeAI>();
		}
		if (gameObject.tag == "Player")
			progressBar = controller.GetComponents<ProgressBar>()[0];
	}
	
	// Use this for initialization
	void OnEnable () {
		health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.deltaTime == 0 || Time.timeScale == 0)
			return;
		
		if (health > 0)
			health += Time.deltaTime * healingSpeed;
		health = Mathf.Clamp(health, 0, maxHealth);
	}
	
	void OnHit (RayAndHit rayAndHit) {
		health -= hitDamage;
		health = Mathf.Clamp(health, 0, maxHealth);
		
		if (hitParticles) {
			GameObject particles = Instantiate(
				hitParticles,
				rayAndHit.hit.point,
				Quaternion.LookRotation(-rayAndHit.ray.direction)
			) as GameObject;
			particles.transform.parent = transform;
		}
		if (hitSound) {
			AudioSource.PlayClipAtPoint(hitSound, rayAndHit.hit.point, 0.6f);
		}

		if (gameObject.tag == "Enermies")
		{
			if (humanStatus.gun == 0)
			{

				float m  = Random.Range (0, 8);
				if (m >= 0)
				{
					isPoison = true;
				}
			}

			else if (humanStatus.gun == 1)
			{
				float m  = Random.Range (0, 8);
				if (m >= 0)
				{
					freeAI.isFrozen = true;
				}
			}
		}
	}
	
	/*
	public void PoisonStatus()
	{
		isPoison = true;
		if (gameObject.tag == "Enermies")
			healthSlide.isPoison = true;
		if (gameObject.tag == "Player")
			progressBar.isPoison = true;

		a++;

		if(!isPoison)
		{
			for (int j = 0; j<6; j++)
			{
				bool i = true;
				float time0 = 10.0f;
				if (i==true)
				{
					time0 -= Time.deltaTime;
					if (time0 <= 0)
					{
						health -= 15;
						i = false;
					}
				}

			}
		}

		if (gameObject.tag == "Enermies")
			healthSlide.isPoison = false;
		if (gameObject.tag == "Player")
			//progressBar.isPoison = false;
		isPoison = false;
	}
*/
	public void FixedUpdate()
	{
		if(isPoison)
		{
			a++;
			if(a%50==0 && a<351)
			{
				health -= poisonDamage;
				if (gameObject.tag == "Enermies")
					healthSlide.isPoison = true;
				if (gameObject.tag == "Player")
					progressBar.isPoison = true;
			}

			if(a>=351)
			{
				isPoison = false;
				if (gameObject.tag == "Enermies")
					healthSlide.isPoison = false;
				if (gameObject.tag == "Player")
					progressBar.isPoison = false;
				a = 0;
			}
		}
	}
}
