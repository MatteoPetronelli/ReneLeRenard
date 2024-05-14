using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance { get; set; }
    [Header("Life")]
    [SerializeField] private float hps;
    [SerializeField] private float hpsMax = 2;
    [SerializeField] private Sprite healthBarSprite;
    [SerializeField] FloatingHealthBar healthBar;

    private void Awake()
    {
        Instance = this;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    private void Start()
    {
        hps = hpsMax;
    }

    private void Update()
    {
        
    }

    public void TakeDamage(float damageAmount)
    {
        hps -= damageAmount;
        healthBar.UpdateHealthBar(hps, hpsMax);
        if (hps <= 0)
            Destroy(gameObject);
    }

    public void ResetLife()
    {
        hps = hpsMax;
    }
}
