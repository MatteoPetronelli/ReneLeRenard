using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float distance;
    public float damage;
    public LayerMask whatIsSolid;

    public GameObject rend;
    public GameObject destroyEffect;
    GameObject explosion;

    private void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
    }

    private void Update()
    {
        rend.transform.Rotate(new Vector3(0, 0, -2));
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                hitInfo.collider.GetComponentInParent<Enemy>().TakeDamage(damage);
            }
            DestroyProjectile();
        }
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {
        explosion = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
