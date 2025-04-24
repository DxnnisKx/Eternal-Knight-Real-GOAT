using UnityEngine;

public class EnemyPartol : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;

    private Transform currentPoint;

    private void Start()
    {
        currentPoint = pointB.transform;
    }

    public float speed = 5;

    // Update is called once per frame
    void Update()
    {
        Vector2 point = currentPoint.position - transform.position;
        rb.linearVelocity = new Vector2(speed, 0);
        if (currentPoint == pointB.transform)
            rb.linearVelocity = new Vector2(speed, 0);
        else
            rb.linearVelocity = new Vector2(-speed,0);

        if (Vector2.Distance(transform.position, currentPoint.position) < .5f && currentPoint == pointB.transform)
        {
            currentPoint =  pointA.transform;
        }
    }
}
