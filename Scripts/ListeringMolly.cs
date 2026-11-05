using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ListerineMolly : Weapon
{
    //Once we use this molly, it's gone
    [Header("Attack")]
    [SerializeField] private ListerineMolly runtimePrefab; //Annoyingly, doesn't load in time and all that to run the coroutine
    [SerializeField] private float attackRadius = 0.5f;
    [Range(1f, 10f)]
    [SerializeField] private float attackRange, jumpPower; //How far the Molotov travels on controller
    [Range(1,5)]
    [SerializeField] private int numJumps;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float attackTime; //how long the bomb is in the air before exploding
    //Could fetch this time by animating the movement of the bomb and getting its time into this variables

    public override void useItem()
    {
        GameObject spawnPoint = FindObjectOfType<PlayerMovement>().gameObject.transform.GetChild(1).gameObject;
        Debug.Log($"Spawned a molly at {spawnPoint.name}");
        Transform itemTransform = spawnPoint.transform;
        ListerineMolly spawnedMolly = Instantiate(runtimePrefab, itemTransform.position, Quaternion.identity);
        spawnedMolly.beginUse();
    }

    void beginUse()
    {
        if(anim != null)
        {
            attackTime = anim.clip.length;
            anim.Play();
        }
        //Make and play an attacking motion and show necessary particles
        StartCoroutine(LaunchBomb());
    }

    IEnumerator LaunchBomb()
    {
        //Calculate the new input by getting the aim direction from InputManager.Instance.LookInput
        Vector3 look = InputManager.Instance.LookInput;
        Vector3 destination;

        if (look.magnitude > 1.5f) //If we have mouse inputs
        {
            Camera cam = Camera.main;
            // Convert the mouse's screen position to the molly's 2D world plane.
            float cameraDistance = Mathf.Abs(cam.transform.position.z - transform.position.z);
            Vector3 screenPosition = new Vector3(look.x, look.y, cameraDistance);
            destination = cam.ScreenToWorldPoint(screenPosition);
            destination.z = transform.position.z;
        }
        else
        {
            Vector2 direction = new Vector2(look.x, look.y).normalized;
            if (direction == Vector2.zero) direction = Vector2.right; // Fallback if there's no aim input.
            destination = transform.position + (Vector3)(direction * attackRange);
        }

        transform.DOJump(destination, jumpPower, numJumps, attackTime, false);

        //Then tween this object's position to the ideal position and finally explode
        yield return new WaitForSecondsRealtime(attackTime);
        pickedUpParticles(); //This should play the explosion particles
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            gameObject.transform.position,
            attackRadius,
            targetLayers
        );

        foreach (Collider2D hit in hits)
        {   // Look on the collider's object or its parent.
            EnemyHealth target = hit.GetComponentInParent<EnemyHealth>();
            if (target != null) target.Damage(itemDamage);
        }
        //Should delete the object after explosion particles have played out for performance
        Destroy(gameObject, particles.main.duration / 2f);
    }
}