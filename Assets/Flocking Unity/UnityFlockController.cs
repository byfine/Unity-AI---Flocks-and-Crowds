using UnityEngine;
using System.Collections;

public class UnityFlockController : MonoBehaviour {
    
    public Vector3 bound;
    public float speed = 20.0f;

    public UnityFlock[] Flocks;
    public int FlockCount;

    private Vector3 initialPosition;
    private Vector3 nextMovementPoint;

    void Awake()
    {
        Flocks = transform.GetComponentsInChildren<UnityFlock>();
        FlockCount = Flocks.Length;
    }

    void Start()
    {
        initialPosition = transform.position;
        CalculateNextMovementPoint();
    }

    private void CalculateNextMovementPoint()
    {
        float posX = Random.Range(initialPosition.x - bound.x, initialPosition.x + bound.x);
        float posY = Random.Range(initialPosition.y - bound.y, initialPosition.y + bound.y);
        float posZ = Random.Range(initialPosition.z - bound.z, initialPosition.z + bound.z);

        nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
    }
     
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), 1.0f * Time.deltaTime);

        if (Vector3.Distance(nextMovementPoint, transform.position) <= 10.0f)
            CalculateNextMovementPoint();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
