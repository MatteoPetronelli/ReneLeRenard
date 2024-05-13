using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackCAC : MonoBehaviour
{
    public float degats = 1;
    public float cooldown = 0.5f;
    public Vector2 attackPosition;
    private Vector3 attackPos;
    public float attackRadius = 1f;
    private bool reload;
    private Collider2D[] victimes;
    //private Animator anim;
    public HeroEntity _entity;

    public static attackCAC instance;

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        //anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (_entity._orientX == 1f)
        {
            attackPos = transform.position + (Vector3)attackPosition;
        }
        else
        {
            attackPos = transform.position + new Vector3(-attackPosition.x, attackPosition.y, 0); ;
        }

        if (Input.GetButtonDown("Fire1") && !reload)
        {
            //anim.SetTrigger("attack");
            //anim.SetBool("attacking", true);
            attacking();
            reload = true;
            StartCoroutine(attackWait());
        }
    }

    public void attacking()
    {
        victimes = Physics2D.OverlapCircleAll(attackPos, attackRadius);
        foreach (Collider2D toDestroy in victimes)
        {
            if (toDestroy.tag == "Enemy")
            {
                toDestroy.transform.parent.GetComponent<Enemy>().TakeDamage(degats);
            }
        }
    }

    IEnumerator attackWait()
    {
        yield return new WaitForSeconds(0.5f);
        //anim.SetBool("attacking", false);
        reload = false;
    }

    private void OnDrawGizmosSelected()
    {
        attackPos = transform.position + (Vector3)attackPosition;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackPos, attackRadius);
    }
}
