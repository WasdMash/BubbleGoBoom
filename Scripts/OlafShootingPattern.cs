using System.Collections;
using UnityEngine;

public class OlafShootingPattern : MonoBehaviour
{
    EnemyShooting enemyShooting;
    [Range(1f, 10f)]
    [SerializeField] float bulletSpeedMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        enemyShooting = GetComponent<EnemyShooting>();
        if(enemyShooting == null){
            Debug.Log("No enemy shooting script found");
            Application.Quit();
        }
        StartAttackPattern();
    }


    void StartAttackPattern(){ StartCoroutine(AttackPattern()); }

    private IEnumerator AttackPattern(){
        int choice = Random.Range(0, 4);

        switch(choice){
            case 0:
                StartCoroutine(SpiralBurst());
                yield return new WaitForSeconds(7.5f);
                break;
            case 1:
                StartCoroutine(DeathSpin());
                yield return new WaitForSeconds(18f);
                break;
            case 2:
                StartCoroutine(FastCircularBurst());
                yield return new WaitForSeconds(6f);
                break;
            case 3:
                StartCoroutine(FastSlow360Burn());
                yield return new WaitForSeconds(30f);
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(4f);
        StartAttackPattern();
    }




    private IEnumerator SpiralBurst(){
        int bullets = 25;
        float initialAngle = 0f;

        float increment = 20f;
        for(int i = 0; i < bullets; i++){
            enemyShooting.Shoot(5, 2f*bulletSpeedMultiplier, initialAngle + (increment * i));
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator DeathSpin(){
        int bullets = 180;
        float initialAngle = 0f;

        float increment =4f;
        for(int i = 0; i < bullets; i++){
            enemyShooting.Shoot(4, 2f * bulletSpeedMultiplier, initialAngle + (increment * i));
            yield return new WaitForSeconds(0.1f);
        }    
    }


    private IEnumerator FastCircularBurst(){
        int bullets = 60;
        float initialAngle = 69727f;

        float increment = 35f;
        for(int i = 0; i < bullets; i++){
            enemyShooting.Shoot(1, 8f * bulletSpeedMultiplier, initialAngle + (increment * i));
            yield return new WaitForSeconds(0.1f);
        }
    }


    private IEnumerator FastSlow360Burn(){
        int bullets = 60;
        float initialAngle = 0f;

        float increment = 13f;
        for(int i = 0; i < bullets; i+= 2){
            enemyShooting.Shoot(8, 1f * bulletSpeedMultiplier, initialAngle + (increment * i));
            yield return new WaitForSeconds(0.5f);
            enemyShooting.Shoot(8, 4f * bulletSpeedMultiplier, initialAngle + (increment * i));
            yield return new WaitForSeconds(0.5f);
        }
    }
}
