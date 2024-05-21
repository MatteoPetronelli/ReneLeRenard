using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pos1Inspect;
    [SerializeField] private Transform pos2Inspect;
    private Transform pos1, pos2;
    private Vector2 targerPos;
    public float speed = 1f;


    private void Start()
    {
        pos1 = pos1Inspect;
        pos2 = pos2Inspect;
        targerPos = pos2.position;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, pos1.position) < .1f) targerPos = pos2.position;
        if (Vector2.Distance(transform.position, pos2.position) < .1f) targerPos = pos1.position;
        transform.position = Vector2.MoveTowards(transform.position, targerPos, Time.deltaTime * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
