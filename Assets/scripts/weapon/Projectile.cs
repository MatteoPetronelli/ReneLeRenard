using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float distance;
    public float damage;
    public int rotationAmount = -5;
    public LayerMask whatIsSolid;

    public GameObject rend;
    public GameObject destroyEffect;
    GameObject explosion;

    public HeroEntity _entity;

    private void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
    }

    private void Update()
    {
        rend.transform.Rotate(new Vector3(0, 0, rotationAmount));
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, whatIsSolid);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                hitInfo.collider.GetComponentInParent<Enemy>().TakeDamage(damage);
            }
            if (hitInfo.collider.CompareTag("Room"))
            {
                hitInfo.collider.GetComponentInParent<Room>().OpenWall();
            }
            if (hitInfo.collider.CompareTag("Player"))
            {
                Destroy(gameObject);
                return;
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
