using UnityEngine;
using System.Collections;

public class TargetMove : MonoBehaviour {
    
    public Vector3 Bound;
    public float Speed = 20.0f;

    private Vector3 initialPosition;
    private Vector3 nextMovementPoint;

    private void Start()
    {
        initialPosition = transform.position;
        CalculateNextMovementPoint();
    }

    private void CalculateNextMovementPoint()
    {
        float posX = Random.Range(initialPosition.x - Bound.x, initialPosition.x + Bound.x);
        float posY = Random.Range(initialPosition.y - Bound.y, initialPosition.y + Bound.y);
        float posZ = Random.Range(initialPosition.z - Bound.z, initialPosition.z + Bound.z);

        nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
    }
    
    void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), 1.0f * Time.deltaTime);

        if (Vector3.Distance(nextMovementPoint, transform.position) <= 10.0f)
            CalculateNextMovementPoint();
    }
}
