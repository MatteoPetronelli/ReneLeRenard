using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private int countFlip;
    public float offset;

    public Animator anim;
    public GameObject projectile;
    public Transform shotPoint;

    public bool isAtacking;

    private float timeReload;
    public float startReload;

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (timeReload <= 0)
        {
            attack();
        }
        else
        {
            timeReload -= Time.deltaTime;
            anim.SetBool("isAttacking", false);
        }
    }

    public void attack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("attack");
            anim.SetBool("isAttacking", true);
            Instantiate(projectile, shotPoint.position, transform.rotation);
            timeReload = startReload;
        }
    }
}
