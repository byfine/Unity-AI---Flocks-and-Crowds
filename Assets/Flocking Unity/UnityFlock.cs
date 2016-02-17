using UnityEngine;
using System.Collections;

public class UnityFlock : MonoBehaviour {

    public float minSpeed = 20.0f;
    public float turnSpeed = 20.0f;
    public float randomFreq = 10.0f;
    public float randomForce = 10.0f;

    // 对齐变量
    public float toLeaderForce = 50.0f;
    public float toLeaderRange = 100.0f;

    // 分离变量
    public float avoidanceRadius = 20.0f;
    public float avoidanceForce = 20.0f;

    // 凝聚变量
    public float cohesionVelocity = 5.0f;
    public float cohesionRadius = 30.0f;

    // 控制boid运动的变量
    private Vector3 velocity;
    private Vector3 randomPush;
    private Vector3 leaderPush;
    private Vector3 avoidPush;
    private Vector3 centerPush;

    // 头领
    private UnityFlockController leader;
    
    void Start ()
    {
        randomFreq = 1.0f / randomFreq;

        if (transform.parent){
            // 指定父物体为leader
            leader = transform.parent.GetComponent<UnityFlockController>();
        }

        if (leader.Flocks != null && leader.FlockCount > 1){
            transform.parent = null;
            // 开始 计算随机移动 的协程
            StartCoroutine(UpdateRandom());
        }
    }

    /// <summary>
    /// 更新随机运动
    /// </summary>
    private IEnumerator UpdateRandom()
    {
        while (true){
            randomPush = Random.insideUnitSphere * randomForce;
            yield return new WaitForSeconds(randomFreq + Random.Range(-randomFreq / 2.0f, randomFreq / 2.0f));
        }
    }


    private void Update()
    {
        if (leader == null || leader.FlockCount < 2){ return; }
        // 设置速度和leader一致
        minSpeed = turnSpeed = leader.speed;
        // 定义变量
        avoidPush = Vector3.zero;
        Vector3 myPosition = transform.position;
        Vector3 avgPosition = Vector3.zero;
        Vector3 direction;
        float distance;
        float f;
        
        // ===================== 分离原则 =====================
        foreach (UnityFlock flock in leader.Flocks)
        {
            Transform flockTrans = flock.transform;

            if (flockTrans != transform){
                Vector3 otherPosition = flockTrans.position;

                // 计算其他物体的位置和，以便计算群体的中心
                avgPosition += otherPosition;

                // 其他物体指向当前物体的向量
                direction = myPosition - otherPosition;
                distance = direction.magnitude;

                if (distance < avoidanceRadius)
                {
                    // 计算偏移的程度
                    f = 1.0f - (distance / avoidanceRadius);

                    if (distance > 0)
                    {
                        // 在物体指向自身的单位方向上添加速度
                        avoidPush += (direction / distance) * f * avoidanceForce;
                    }
                }
            }
        }
        avoidPush /= (leader.FlockCount - 1);

        // ===================== 对齐原则 =====================

        // 到leader的方向
        direction = leader.transform.position - myPosition;
        distance = direction.magnitude;
        f = distance / toLeaderRange;

        // 计算到leader的速度
        if (distance > 0){
            leaderPush = (direction / distance) * f * toLeaderForce;
        }

        // ===================== 凝聚原则 =====================
        // 计算集群的中心值
        Vector3 centerPos = (avgPosition / (leader.FlockCount - 1));
        // 到中心的方向
        Vector3 toCenter = centerPos - myPosition;
        distance = toCenter.magnitude;

        // 与center保持距离
        if (distance > cohesionRadius){
            f = distance / cohesionRadius - 1.0f;
            centerPush = (toCenter / distance) * f * cohesionVelocity;
        }
        else{
            centerPush = Vector3.zero;
        }

        // ==========================================

        float speed = velocity.magnitude;
        if (speed < minSpeed && speed > 0){
            velocity = (velocity / speed) * minSpeed;
        }
        
        Vector3 wantedVel = velocity;

        // 计算最终的速度
        wantedVel -= wantedVel * Time.deltaTime;
        wantedVel += randomPush * Time.deltaTime;
        wantedVel += leaderPush * Time.deltaTime;
        wantedVel += avoidPush * Time.deltaTime;
        wantedVel += centerPush * Time.deltaTime;
        
        // 应用最终速度来旋转
        velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.00f);
        transform.rotation = Quaternion.LookRotation(velocity);

        // 应用最终速度来移动
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

}
