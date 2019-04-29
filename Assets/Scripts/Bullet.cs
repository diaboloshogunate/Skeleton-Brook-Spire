using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Bullet : MonoBehaviour
{
    public Vector2 velocity;
    private Animator anim;
    private Rigidbody2D rb;

    private void Start()
    {
        this.anim = GetComponent<Animator>();
        this.rb = GetComponent<Rigidbody2D>();
        this.rb.velocity = this.velocity;
    }

    private void FixedUpdate()
    {
        this.rb.velocity = this.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.anim.SetTrigger("Hit");
        if(collision.gameObject.tag == "Enemy")
        {
            this.velocity = Vector2.zero;
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.Dmg(1f);
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
