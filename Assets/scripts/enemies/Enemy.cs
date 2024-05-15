using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance { get; set; }
    public HeroLife _heroLife;

    [Header("Life")]
    [SerializeField] private float hps;
    [SerializeField] private float hpsMax = 2;
    [SerializeField] private Sprite healthBarSprite;
    [SerializeField] FloatingHealthBar healthBar;

    [Header("Attack")]
    [SerializeField] private float damages = 1;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _heroLife.TakeDamage(damages);
        }
    }
}
