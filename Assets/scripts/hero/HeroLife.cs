using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroLife : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private Image healthBarSprite;
    [SerializeField] FloatingHealthBar healthBar;
    public float hps;
    public float hpsMax = 7;
    

    [Header("Spawning & Respawning")]
    private Vector3 spawnPosition;
    private bool invincible;

    private void Start()
    {
        spawnPosition = transform.position;
        hps = hpsMax;
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

    public void TakeDamage(float damage)
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
