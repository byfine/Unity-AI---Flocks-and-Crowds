using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Flock : MonoBehaviour
{
    public FlockController Controller;
    public Rigidbody Rigid;

    public float RandomFreq = 10f;
    private float randomLoop;
    private Vector3 randomize;

    void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
        RandomFreq = 1f / RandomFreq;
    }

    void Update()
    {
        if (Controller){
            Vector3 relativePos = Steer() * Time.deltaTime;

            if (relativePos != Vector3.zero){
                Rigid.velocity = relativePos;
            }

            // 速度限制在最大和最小范围内
            Vector3 velocity = Rigid.velocity;
            float speed = velocity.magnitude;
            if (speed > Controller.maxVelocity){
                Rigid.velocity = velocity.normalized * Controller.maxVelocity;
            }
            else if (speed < Controller.minVelocity){
                Rigid.velocity = velocity.normalized * Controller.minVelocity;
            }

            // 面向运动方向
            transform.rotation = Quaternion.LookRotation(Rigid.velocity);
        }
    }

    // 根据 Craig Reynold 的算法计算速度 (Cohesion, Alignment, Follow leader and Seperation)
    private Vector3 Steer()
    {
        // 指向中心的方向，Cohesion
        Vector3 center = Controller.FlockCenter - transform.position;
        // 对齐，alignment
        Vector3 velocity = Controller.FlockVelocity - Rigid.velocity; 
        // 到目标的方向，跟随目标
        Vector3 follow = Controller.target.position - transform.position;
        // 分离
        Vector3 separation = Vector3.zero;

        // 计算分离速度
        foreach (Flock flock in Controller.FlockList){
            if (flock != this){
                Vector3 relativePos = transform.position - flock.transform.position;
                separation += relativePos / (relativePos.sqrMagnitude);
            }
        }

        // 计算随机运动
        randomLoop += Time.deltaTime;
        if (randomLoop > RandomFreq){
            randomize = Random.insideUnitSphere * 2;
            randomize.Normalize();
            randomLoop = 0;
        }

        // 返回最终速度
        return (Controller.centerWeight * center + Controller.velocityWeight * velocity + Controller.separationWeight * separation +
                Controller.followWeight * follow + Controller.randomizeWeight * randomize);

    }
}
