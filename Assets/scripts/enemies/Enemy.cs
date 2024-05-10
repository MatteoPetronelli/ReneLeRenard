using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private float hps;
    [SerializeField] private float hpsMax = 2;
    [SerializeField] FloatingHealthBar healthBar;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    private void Start()
    {
        hps = hpsMax;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TakeDamage(1f);
    }

    public void TakeDamage(float damageAmount)
    {
        hps -= damageAmount;
        healthBar.UpdateHealthBar(hps, hpsMax);
        if (hps <= 0)
            Destroy(gameObject);
    }
}
