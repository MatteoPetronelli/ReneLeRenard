using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pos1Inspect;
    [SerializeField] private Transform pos2Inspect;
    private Vector2 pos1, pos2;
    public float speed = 1f;
    private int countBeforeReturn = 100;
    private bool IsPlayerOnPlatform;
    private bool IsPlayerOffPlatform;


    private void Start()
    {
        pos1 = pos1Inspect.position;
        pos2 = pos2Inspect.position;
    }

    private void Update()
    {
        if(IsPlayerOnPlatform)
            transform.position = Vector3.MoveTowards(transform.position, pos2, Time.deltaTime * speed);
        
    }

    private void FixedUpdate()
    {
        if (IsPlayerOffPlatform)
        {
            countBeforeReturn--;
            if (countBeforeReturn <= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, pos1, Time.deltaTime * speed);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
            IsPlayerOnPlatform = true;
            IsPlayerOffPlatform = false;
            countBeforeReturn = 100;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            IsPlayerOnPlatform = false;
            IsPlayerOffPlatform = true;
        }
    }
}
