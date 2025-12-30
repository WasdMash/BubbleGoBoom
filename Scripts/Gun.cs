using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour {
	
	//The bullet and its movement forces
	[SerializeField] LayerMask enemyMask;
	[SerializeField] AudioSource bangSound;
	[SerializeField] AudioClip bangClip;
	public GameObject bullet;
	[SerializeField] int bulletDamage;
	[SerializeField] float shootForce, upwardForce;
	
	//Gun stats
	[SerializeField] float TimeBetweenShots,TimeBetweenShooting, spread, reloadtime;
	[SerializeField] int magazineSize, bulletsPerTap;
	[SerializeField] bool allowButtonHold;
	
	//How many bullets were shot and how many are left
	[SerializeField]
	int bulletsLeft, bulletShot;
	
	[SerializeField]
	bool shooting, readyToShoot, reloading, BulletsLeftAreGreaterThanZero;
	
	[SerializeField] Transform attackPoint;
	
	//Graphics and visual effects
	public GameObject muzzleFlash;
	//public Animator GunAnimator //Might use if I want to play animations when firing and reloading
	public Text bulletUIText;
	public GameObject gunUI;
	[SerializeField] Transform playerTransform;
	[SerializeField] [Range(0f,3f)] float armRadius, rotationSpeed;
	
	//This is used to fix bugs, which I will defintely need
	public bool allowInvoke;
	
	// Use this for initialization
	void Awake () {
		bulletsLeft = magazineSize;
		readyToShoot = true;
	}
	
	void MyInput()
	{
		// Get mouse position in world space
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;  // Ensure the z value is 0 since we're working in 2D

		// Calculate direction from the player to the mouse
		Vector3 directionToMouse = mousePos - playerTransform.position;

		// Calculate the angle between the gun's current forward direction and the direction to the mouse
		float angle = Vector3.SignedAngle(Vector3.up, directionToMouse, Vector3.forward);

		// Apply the calculated rotation to the gun
		transform.rotation = Quaternion.Euler(0, 0, angle+180);

		//Check if I can hold fire button (not really for revolvers)
		if (allowButtonHold)  shooting = Input.GetMouseButton(0);
		else shooting = Input.GetMouseButtonDown(0);
		
		//Triggering the reload functions
		if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
		{
			Reload();
			Debug.Log("You are quite impulsive, aren't you?");
		}
		
		if (readyToShoot && shooting && !BulletsLeftAreGreaterThanZero && !reloading)
		{
		 	Reload();
		 	Debug.Log("Ran out of bullets, fool");
		}
		
		//If I am able to shoot
		if (readyToShoot && shooting && !reloading && BulletsLeftAreGreaterThanZero)
		{
			bulletShot = 0;
			Shoot();
		}
	}
	
	public void Shoot()
	{
		readyToShoot = false;
        //Need to get the position of mouse in world to aim the bullets
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		//Use a raycast to get location of bullet target
		RaycastHit2D hit = Physics2D.Raycast(attackPoint.position - 0.2f*attackPoint.transform.up, mouseWorldPos, 40f);
        if(hit && hit.collider.name != "Player"){
            //Checks if the ray hit something
            //Calculate direction from place of attack to targetpoint
            Vector3 targetPoint = new Vector3(hit.point.x, hit.point.y, 0f);
            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
            
            //Instantiate the bullet to be shot
            GameObject curbullet = Instantiate(bullet, attackPoint.position - 0.2f*attackPoint.transform.up, Quaternion.identity) as GameObject;
            
            //Actually shoots the object
			curbullet.GetComponent<Bullet>().targetEnemyMask = enemyMask;
			curbullet.GetComponent<Bullet>().bulletDamage = bulletDamage;
            curbullet.GetComponent<Rigidbody2D>().velocity = -attackPoint.transform.up * shootForce;

			//Sound the gun
			bangSound.PlayOneShot(bangClip);
            
            bulletsLeft--; // Used to decrement values
            bulletShot++;
        }

        //Used to reload
        if (allowInvoke)
        {
            Invoke("ResetShot", TimeBetweenShooting);
            allowInvoke = false;
        }
        
        //If I want to shoot multiple bullets per Tap, e.g a shotgun
        if(bulletShot < bulletsPerTap && bulletsLeft > 0) Invoke("Shoot", TimeBetweenShots);	
	}
	
	//Allows me to shoot again after some time
	void ResetShot()
	{
		readyToShoot = true;
		allowInvoke = true;
	}
	
	//Begins the process of reloading my gun
	public void Reload()
	{
		reloading = true;
		Invoke("ReloadFinished", reloadtime);
	}
	
	//Actual reloading of my gun
	void ReloadFinished()
	{
		bulletsLeft = magazineSize;
		reloading = false;
	}
	
	
	// Update is called once per frame
	void Update () {
		if (bulletsLeft <= 0) BulletsLeftAreGreaterThanZero = false;
		else BulletsLeftAreGreaterThanZero = true;
		MyInput();
		
		if(gunUI != null) bulletUIText.text = (bulletsLeft / bulletsPerTap).ToString() + " / " + (magazineSize / bulletsPerTap).ToString();
	}
}
