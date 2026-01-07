using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int time;
    // Start is called before the first frame update
    void Start() => InvokeRepeating("UpdateTime", 1f, 1f);
    void StopTime() => CancelInvoke();
    void ResetTime(){ time = 0;}
    void UpdateTime(){ time += 1;}
}

