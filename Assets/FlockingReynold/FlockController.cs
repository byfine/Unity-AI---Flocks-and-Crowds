using UnityEngine;
using System.Collections;

public class FlockController : MonoBehaviour {

    public float minVelocity = 5;       //最小速度
    public float maxVelocity = 50;       //最大速度
    public int flockSize = 20;          //群体中个体数量
    public float centerWeight = 2;      //群体离中心多远 (the more weight stick closer to the center)
    public float velocityWeight = 10;    //Alignment behaviors
    public float separationWeight = 5;  //群体的分散程度
    public float followWeight = 5;      //群体里leader多远 (the more weight make the closer follow)
    public float randomizeWeight = 3;   //随机

    public Flock prefab;
    public Transform target;        //跟随的目标

    public Vector3 FlockCenter;     //群体的中心
    public Vector3 FlockVelocity;   //群体的平均速度

    public readonly ArrayList FlockList = new ArrayList();

    void Start()
    {
        // 初始化生成群体
        for (int i = 0; i < flockSize; i++)
        {
            Flock flock = Instantiate(prefab, transform.position, transform.rotation) as Flock;
            if (flock != null) {
                flock.transform.parent = transform;
                flock.Controller = this;
                flock.transform.position = transform.position + Random.insideUnitSphere * centerWeight;
                FlockList.Add(flock);
            }
        }
    }

    void Update()
    {
        //计算整个群体的 中心 和 速度
        Vector3 center = Vector3.zero;
        Vector3 velocity = Vector3.zero;

        foreach (Flock flock in FlockList)
        {
            center += flock.transform.position;
            velocity += flock.Rigid.velocity;
        }

        FlockCenter = center / flockSize;
        FlockVelocity = velocity / flockSize;
    }
}
