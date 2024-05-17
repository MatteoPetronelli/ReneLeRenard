using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pos1Inspect;
    [SerializeField] private Transform pos2Inspect;
    public Vector2 pos1;
    public Vector2 pos2;
    public float speed = 1f;

    private void Start()
    {
        pos1 = pos1Inspect.position;
        pos2 = pos2Inspect.position;
    }

    private void Update()
    {
        transform.position = Vector2.Lerp(pos1, pos2, Mathf.PingPong(Time.time * speed, 1f));
    }
}
