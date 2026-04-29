using System.Collections;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    public int collisionDamage;
    [SerializeField] float damageCooldown = 1.0f;  // Cooldown time in seconds

    private Transform player;
    private bool canDamage = true;  // Tracks if damage can be applied

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.Log("Player not found");
            Application.Quit();
        }

        player = playerObject.transform;
    }

    void Update()
    {
        //Don't want the enemies moving when dialogue is happening (should probably do the same for opening chests but hey, item management)
        if (player != null && !DialogueManager.Instance.isDialogueActive)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.z = 0f;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnCollisionStay2D(Collision2D victim)
    {
        if (victim.gameObject.CompareTag("Player") && canDamage)
        {
            Debug.Log("Damaging player...");
            HealthManager playerHealth = FindObjectOfType<HealthManager>().GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.DamagePlayer(collisionDamage);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}
