using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] HeroEntity _entity;
    [SerializeField] ParticleSystem movementParticle;

    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;
        
    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPeriod;

    [SerializeField] Rigidbody2D playerRb;

    public ParticleSystem fallParticle;
    public ParticleSystem jumpParticle;

    float counter;

    private void Update()
    {
        counter += Time.deltaTime;
        if(_entity.IsTouchingGround && Mathf.Abs(playerRb.velocity.x) > occurAfterVelocity)
        {
            if(counter > dustFormationPeriod)
            {
                movementParticle.Play();
                counter = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("JumpPad") || collision.CompareTag("MGround") || collision.CompareTag("Wall"))
        {
            fallParticle.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("JumpPad") || collision.CompareTag("MGround") || collision.CompareTag("Wall"))
        {
            jumpParticle.Play();
        }
    }
}
