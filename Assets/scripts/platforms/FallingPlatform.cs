using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f;

    private Vector2 origins;
    private bool falling = false;
    private bool startEnable = false;
    private int countDisable = 100;
    private int countEnable = 250;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spRenderer;
    [SerializeField] private Collider2D col;

    private void Start()
    {
        origins = new Vector2(transform.position.x, transform.position.y);
    }

    private void FixedUpdate()
    {
        DisablePlatform();
        EnablePlatform();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Avoid calling the coroutine multiple times if it's already been called (falling)
        if (falling)
            return;
        if (startEnable)
            return;
        Rigidbody2D _rigidbodyEntity= collision.gameObject.GetComponent<Rigidbody2D>();
        // If the player landed on the platform, start falling
        if (collision.transform.tag == "Player")
        {
            StartCoroutine(StartFall());
        }
    }

    private IEnumerator StartFall()
    {
        falling = true;

        // Wait for a few seconds before dropping
        yield return new WaitForSeconds(fallDelay);

        // Enable rigidbody and destroy after a few seconds
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void DisablePlatform()
    {
        if (falling)
            countDisable--;
        if (countDisable <= 0)
        {
            falling = false;
            startEnable = true;
            spRenderer.enabled = false;
            col.enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            transform.position = new Vector2(origins.x, origins.y);
        }
    }

    private void EnablePlatform()
    {
        if (startEnable)
            countEnable--;
        if (countEnable <= 0)
        {
            startEnable= false;
            countDisable = 100;
            countEnable = 250;
            spRenderer.enabled = true;
            col.enabled = true;
        }
    }
}
