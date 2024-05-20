using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float offset;

    public Animator anim;
    public GameObject projectile;
    public Transform shotPoint;

    private float timeReload;
    public float startReload;

    private int count = 2;

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (timeReload <= 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Attack();
                StartCoroutine(IsAttack());
            }
        }
        else
        {
            timeReload -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        anim.SetTrigger("attack");
        Instantiate(projectile, shotPoint.position, transform.rotation);
        timeReload = startReload;
        count = 2;
    }

    private IEnumerator IsAttack()
    {
        while (count >= 0)
        {
            yield return new WaitForSeconds(1);
            count--;
            anim.SetBool("isAttacking", true);
        }
        anim.SetBool("isAttacking", false);
    }
}
