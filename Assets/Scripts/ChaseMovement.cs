using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChaseMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    
    private float velocity = 1f;
    private bool isChasing = false;
    private GameObject playerPos;
    private Vector2 move;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.move = Vector2.zero;
        this.velocity = Mathf.Clamp(8 - Random.Range(2f, 3f) * Random.Range(2f, 3f), 1, 4f);
    }

    void Update()
    {
        if(this.isChasing)
        {
            this.move = this.playerPos.transform.position - this.transform.position;
            this.rb.velocity = this.move.normalized * this.velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            this.isChasing = true;
            this.playerPos = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.isChasing = false;
        }
    }
}
