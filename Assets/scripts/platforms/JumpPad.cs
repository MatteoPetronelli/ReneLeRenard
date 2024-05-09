using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private HeroEntity _entity;
    private bool HasExitJumpPad;
    private float countRebootJumpSettings = 50f;

    private void FixedUpdate()
    {
        if (HasExitJumpPad)
            countRebootJumpSettings--;
        if (countRebootJumpSettings <= 0 )
            _entity.IsJumpingWithPad = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            countRebootJumpSettings = 50f;
            _entity._indexJumpSetting = -1;
            _entity.JumpStart();
            _entity._UpdateJump(_entity.currerntJumpSetting);
            _entity.IsJumpingWithPad = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HasExitJumpPad = true;
        }
    }
}
