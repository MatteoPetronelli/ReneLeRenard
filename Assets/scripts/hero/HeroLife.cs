using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLife : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private float hps;
    [SerializeField] private float hpsMax = 7;
    [SerializeField] FloatingHealthBar healthBar;

    [Header("Spawning & Respawning")]
    private Vector3 spawnPosition;
    private bool invincible;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    private void Start()
    {
        spawnPosition = transform.position;
        hps = hpsMax;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            TakeDamage(1f);
        if (Input.GetKeyDown(KeyCode.V))
            if (hps != hpsMax)
                Heal(1f);
    }

    public void TakeDamage(float damageAmount)
    {
        hps -= damageAmount;
        healthBar.UpdateHealthBar(hps, hpsMax);
        if (hps <= 0)
            Destroy(gameObject);
    }

    public void Heal(float amount)
    {
        if ((hps + amount) > hpsMax)
        {
            hps = hpsMax;
        }
        else
        {
            hps += amount;
        }
        healthBar.UpdateHealthBar(hps, hpsMax);
    }

    public void takeDamage(int damage)
    {
        if (invincible) { return; }

        invincible = true;
        hps -= damage;
        healthBar.UpdateHealthBar(hps, hpsMax);
        if (hps <= 0)
        {
            hps = hpsMax;
            healthBar.UpdateHealthBar(hps, hpsMax);
            Enemy.Instance.ResetLife();
            transform.position = spawnPosition;
        }
        StartCoroutine(waitInvincible());
    }

    IEnumerator waitInvincible()
    {
        yield return new WaitForSeconds(1f);
        invincible = false;
    }

    void OnTriggerEnter2D(Collider2D truc)
    {
        if (truc.tag == "Death")
        {
            transform.position = spawnPosition;
        }

        if (truc.tag == "CheckPoint")
        {
            spawnPosition = transform.position;
        }
    }
}
