using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ListerineMolly : Weapon
{
    //Once we use this molly, it's gone
    [Header("Attack")]
    [SerializeField] private float attackRadius = 0.5f;
    [Range(2f, 50f)]
    [SerializeField] private float attackRange, jumpPower; //How far the Molotov travels on controller
    [Range(1,5)]
    [SerializeField] private int numJumps;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float attackTime = 3.5f; //how long the bomb is in the air before exploding
    //Could fetch this time by animating the movement of the bomb and getting its time into this variables

    public override void useItem()
    {

        base.useItem();
        if(anim != null) anim.Play();
        //Make and play an attacking motion and show necessary particles
        StartCoroutine(LaunchBomb());
        
    }

    IEnumerator LaunchBomb()
    {
        //Calculate the new input by getting the aim direction from InputManager.Instance.LookInput
        Vector3 look = InputManager.Instance.LookInput;
        bool likelyMouse = Mathf.Abs(look.magnitude) > 1.5f;
        if(likelyMouse) transform.DOJump(look, jumpPower, numJumps, attackTime, false); //movement is simple
        else transform.DOJump(look*attackRange, jumpPower, numJumps, attackTime, false); //Gotta tweak to determine how far to throw on controller - could link to button press
        
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
        Destroy(gameObject, particles.main.duration);
    }
}