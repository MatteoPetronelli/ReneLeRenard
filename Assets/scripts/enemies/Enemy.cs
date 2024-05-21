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

    [Header("Death")]
    public Animator ani;
    public Collider2D col;
    public SpriteRenderer sprite;
    public GameObject drop;
    public Transform main;
    private float alpha = 255;
    private int countBeforeDestroy = 125;
    public bool isDying;

    private void Awake()
    {
        Instance = this;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    private void Start()
    {
        hps = hpsMax;
    }

    private void FixedUpdate()
    {
        if (isDying)
        {
            countBeforeDestroy--;
            alpha -= 1.8f;
            sprite.color = new Color(1, 1, 1, alpha/255);
            if (countBeforeDestroy <= 0) 
            {
                Instantiate(drop, transform.position, transform.rotation, main);
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        hps -= damageAmount;
        healthBar.UpdateHealthBar(hps, hpsMax);
        if (hps <= 0)
        {
            col.enabled = false;
            isDying = true;
            ani.SetBool("isDying", true);
        }
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
