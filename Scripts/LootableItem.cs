using UnityEngine;

public class LootableItem : MonoBehaviour
{
    [Header("Item properties")]
    public int itemDamage;
    public int maxDurability;
    public int durability; //This durability can be altered on instances when it is used over time

    [Header("Visible things")]
    public Animation anim;
    public Sprite itemSprite;
    public ParticleSystem particles;
}
