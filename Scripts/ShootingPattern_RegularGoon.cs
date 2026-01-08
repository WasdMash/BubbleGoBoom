using UnityEngine;

public class ShootingPattern_RegularGoon : MonoBehaviour
{
    [SerializeField] int amount;
    [SerializeField] float speed;
    [SerializeField] float cooldown;
    EnemyShooting enemyShooting;

    // Start is called before the first frame update
    void Start()
    {
        enemyShooting = GetComponent<EnemyShooting>();
        if(enemyShooting == null){
            Debug.Log("No enemy shooting script found");
            Application.Quit();
        }

        InvokeRepeating("Shoot", 0f, cooldown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot() => enemyShooting.Shoot(amount, speed, 0);
}

