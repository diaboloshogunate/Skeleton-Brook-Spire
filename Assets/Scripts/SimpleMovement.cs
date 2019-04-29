using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    private float velocity = 1f;
    [SerializeField]
    private float amplitude = 4f;
    [SerializeField]
    private bool moveX = false;
    [SerializeField]
    private bool moveY = false;
    private Vector2 startPosition;
    private Vector2 movePosition;
    private float startTime = 0f;

    private void Start()
    {
        this.startPosition = this.transform.position;
        this.movePosition = this.startPosition;
    }

    private void Update()
    {
        this.movePosition = this.transform.position;
        float delta = Mathf.Cos((this.startTime - Time.time) * this.velocity) * this.amplitude;
        if(this.moveX) { this.movePosition.x = this.startPosition.x + delta; }
        if(this.moveY) { this.movePosition.y = this.startPosition.y + delta; }
        this.transform.position = this.movePosition;
    }
}
