using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemiPatrol : MonoBehaviour
{
    //METTRE CE SCRIPT SUR LES ENNEMIS

    public Enemy self;
    public float speed = 2f;                                            // Vitesse de déplacement ennemi
    [SerializeField, Range(0.1f, 50f)] private float limiteDroite = 1f; // distance entre l'ennemi et la limite de patrouille à droite (limité entre 0.1 et 50)
    [SerializeField, Range(0.1f, 50f)] private float limiteGauche = 1f; // distance entre l'ennemi et la limite de patrouille à gauche (limité entre 0.1 et 50)
    private Vector3 limiteDroitePosition;                               // Sert a transformer la distance avec la limite droite en coordonnées X/Y/Z
    private Vector3 limiteGauchePosition;                               // Sert a transformer la distance avec la limite gauche en coordonnées X/Y/Z
    private Rigidbody2D rb;                                             // Le rigidbody de l'ennemi
    private float direction = 1f;                                       // Direction vers laquelle l'ennemi se dirige (1 = droite, -1 = gauche)
    public SpriteRenderer skin;                                        // Le sprite de l'ennemi, pour qu'on puisse le retourner quand il change de direction
    public Animator ani;
    private bool isWaiting;
    private int countWait = 125;
    private int countMin;

    // Au lancement du jeu, on enregistre le rigidbody et le sprite de l'ennemi
    // On transforme aussi les valeurs de limite Droite et Gauche en coordonnées réelles
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        limiteDroitePosition = transform.position + new Vector3(limiteDroite, 0, 0);
        limiteGauchePosition = transform.position - new Vector3(limiteGauche, 0, 0);
    }


    void Update()
    {
        if (self.isDying && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.velocity = Vector3.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (isWaiting & countMin <= 0)
            ani.SetBool("isWaiting", true);
        else
        {
            ani.SetBool("isWaiting", false);
            isWaiting = false;
        } 
        // Si l'ennemi se coince contre quelque chose (sa vitesse plus petite que 0.1 m/s) alors il se retourne
        if (Mathf.Abs(rb.velocity.x) < 0.1f && !isWaiting)
        {
            direction = -direction;
        }

        //Si il dépasse sa limite Droite, il se retourne
        if (transform.position.x > limiteDroitePosition.x)
        {
            if (countMin <= 0)
                isWaiting = true;
            direction = -1f;
        }

        //Si il dépasse sa limite gauche, il se retourne
        if (transform.position.x < limiteGauchePosition.x)
        {
            if (countMin <= 0)
                isWaiting = true;
            direction = 1f;
        }

        // Enfin on met le sprite dans le bon sens
        if (direction == 1f && !self.isDying)
        {
            skin.flipX = true;
        }

        if (direction == -1f && !self.isDying)
        {
            skin.flipX = false;
        }

        // Enfin on fait avancer l'ennemi dans la bonne direction
        if (!isWaiting && !self.isDying)
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        wait();
    }

    //Cette fonction sert a visualiser le chemin de l'ennemi dans l'éditeur
    void OnDrawGizmos()
    {
        if (!Application.IsPlaying(gameObject))
        {
            limiteDroitePosition = transform.position + new Vector3(limiteDroite, 0, 0);
            limiteGauchePosition = transform.position - new Vector3(limiteGauche, 0, 0);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawCube(limiteDroitePosition, new Vector3(0.2f, 1, 0.2f));
        Gizmos.DrawCube(limiteGauchePosition, new Vector3(0.2f, 1, 0.2f));
        Gizmos.DrawLine(limiteDroitePosition, limiteGauchePosition);
    }

    void wait()
    {
        if (isWaiting)
        {
            countWait--;
            if (countWait <= 0)
            {
                isWaiting  = false;
                countWait = 125;
                countMin = 125;
            }
        }

        if (!isWaiting)
        {
            countMin--;
        }

    }
}