using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private int countFlip;
    public float offset;

    public GameObject projectile;
    public Transform shotPoint;

    private float timeReload;
    public float startReload;

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (timeReload <= 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Instantiate(projectile, shotPoint.position, transform.rotation);
                timeReload = startReload;
            }
        }
        else
        {
            timeReload -= Time.deltaTime;
        }
    }
}
