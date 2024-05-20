using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackTrigger : MonoBehaviour
{
    public Weapon weapon;
    public Animator anim;
    private void attack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("attack");
            weapon.attack();
        }
    }
}
