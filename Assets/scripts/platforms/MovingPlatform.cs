using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector2 pos1;
    public Vector2 pos2;
    public float speed = 1f;

    private void Update()
    {
        transform.position = Vector2.Lerp(pos1, pos2, Mathf.PingPong(Time.time * speed, 1f));
    }
}
