using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float health = 1f;
    private float maxHealth = 10f;

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    public void Dmg(float amt)
    {
        this.health = Mathf.Clamp(this.health - amt, 0, this.maxHealth);
        if (this.health == 0f)
        {
            this.Die();
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
