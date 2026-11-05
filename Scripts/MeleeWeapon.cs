using UnityEngine;

//I'm adding all of the weapon unique functionality
public class MeleeWeapon : Weapon
{
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float attackCooldown = 0.4f;

    public override void useItem()
    {
        base.useItem();
        if (Time.time < nextAttackTime || durability <= 0) return;
        nextAttackTime = Time.time + attackCooldown;
        //Make an attacking motion and show necessary particles
        if(anim != null) anim.Play();
            //Give it a collider or raycast within attacking range
            //Do set damage if we hit anything
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            targetLayers
        );

        foreach (Collider2D hit in hits)
        {   // Look on the collider's object or its parent.
            EnemyHealth target = hit.GetComponentInParent<EnemyHealth>();
            if (target != null) target.Damage(itemDamage);
        }
        WearDown(-5); //Every time we use this weapon, we want to lose 5 durability points
    }
}