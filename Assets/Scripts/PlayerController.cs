using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CameraShake))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private CameraShake camShake;
    private SpriteRenderer sprite;
    private Animator anim;
    private SceneChange sceneChange;
    private AudioSource audio;

    // health/status
    [SerializeField]
    private SpriteMask healthMask;
    private bool isAlive = true;
    private float maxHealth = 100f;
    private float health = 100f;
    private bool isMovementLocked = false;
    private float startFalshTime;

    // physics/movement
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lastMovementInput;
    private Vector2 velocityInput;
    private float walkVelocity = 5f;
    private float runVelocity = 7.5f;
    private float dashVelocity = 50f;
    private bool isRunning = false;
    private bool isDashing = false;
    private bool isKnockback = false;
    private bool isShielding = false;
    [SerializeField]
    private AudioClip hurtSFX;
    [SerializeField]
    private AudioClip deathSFX;
    [SerializeField]
    private AudioClip shootSFX;
    [SerializeField]
    private GameObject dashEffect;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject aoe;
    [SerializeField]
    private GameObject shield;

    // actions
    private float runDmg = 1f;
    private float dashDmg = 5f;
    private float specialDmg = 10f;
    private float attackDmg = 1f;
    private float shieldDmg = 2f;
    private float healAmt = 1f;

    // time management
    private float runTickTime = 0.5f;
    private float timeUntilRunTick = 0f;
    private float dashTime = 0.1f;
    private float dashTimeLeft = 0f;
    private float dashCoolDownTime = 1f;
    private float dashCoolDownTimeLeft = 0f;
    private float healTick = 1f;
    private float timeUntilHealTick = 1f;
    private float knockbackTimerLeft;

    private void Start()
    {
        this.sceneChange = FindObjectOfType<SceneChange>();
        this.rb = GetComponent<Rigidbody2D>();
        this.camShake = GetComponent<CameraShake>();
        this.sprite = GetComponent<SpriteRenderer>();
        this.anim = GetComponent<Animator>();
        this.audio= FindObjectOfType<AudioSource>();
        this.UpdateHealthUI();
    }

    public void Update()
    {
        if(!this.isAlive) { return; }
        this.Dashing();
        this.Cooldowns();
        this.Flashing();
        this.HandleInput();
        this.RunDmg();
        this.Rest();
    }

    private void HandleInput()
    {
        if(this.isMovementLocked) { return; }

        if (this.movementInput.magnitude > 0f) { this.lastMovementInput = this.movementInput; }
        this.movementInput = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.movementInput.y = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.movementInput.y = -1;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.movementInput.x = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.movementInput.x = -1;
        }
        
        if(Input.GetKey(KeyCode.F))
        {
            this.Shield();
        }
        else if(Input.GetKeyUp(KeyCode.F))
        {
            this.ShieldDown();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            this.StartDash();
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            this.AOE();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            this.Shoot();
        }

        this.isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void Cooldowns()
    {
        if(this.dashCoolDownTimeLeft > 0f) { this.dashCoolDownTimeLeft -= Time.deltaTime; }
        if(this.knockbackTimerLeft > 0f) { this.knockbackTimerLeft -= Time.deltaTime; }
    }

    private void Shoot()
    {
        this.Dmg(1f);
        Vector2 direction = this.movementInput.magnitude > 0f ? this.movementInput : this.lastMovementInput;
        Vector3 spawnPosition = this.transform.position + (Vector3) direction;
        float angle = Vector2.Angle(Vector2.right, direction);
        if (direction.y < 0f) { angle = -1f * angle; }
        GameObject bullet = Instantiate(this.bullet, spawnPosition, Quaternion.identity);
        bullet.GetComponent<Bullet>().velocity = direction.normalized * 10f;
        bullet.transform.Rotate(Vector3.forward, angle);
        this.audio.PlayOneShot(this.shootSFX);
    }

    private void AOE()
    {
        this.Dmg(10f);
        GameObject bullet = Instantiate(this.aoe, this.transform.position, Quaternion.identity);
    }

    private void Shield()
    {
        this.isShielding = true;
        this.shield.SetActive(true);
        this.movementInput = Vector2.zero;
        this.Dmg(this.shieldDmg * Time.deltaTime);
    }

    private void ShieldDown()
    {
        this.isShielding = false;
        this.shield.SetActive(false);
    }

    private void StartDash()
    {
        if(dashCoolDownTimeLeft > 0f) { return; }
        this.isDashing = true;
        this.isMovementLocked = true;
        this.isRunning = false;
        this.Dmg(this.dashDmg);
        this.dashTimeLeft = this.dashTime;
        Instantiate(this.dashEffect, this.transform.position, Quaternion.identity);
        this.camShake.Shake();
        Physics2D.IgnoreLayerCollision(11, 9, true);
        Physics2D.IgnoreLayerCollision(11, 10, true);
    }

    private void Dashing()
    {
        if (!this.isDashing) { return; }

        this.dashTimeLeft -= Time.deltaTime;
        if(this.dashTimeLeft > 0f) { return; }

        this.isDashing = false;
        this.isMovementLocked = false;
        this.dashCoolDownTimeLeft = this.dashCoolDownTime;
        Physics2D.IgnoreLayerCollision(11, 9, false);
        Physics2D.IgnoreLayerCollision(11, 10, false);
    }

    private void RunDmg()
    {
        if (this.isRunning)
        {
            this.timeUntilRunTick -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                this.Dmg(this.runDmg);
                this.timeUntilRunTick = this.runTickTime;

            }
            else if (this.timeUntilRunTick < 0f)
            {
                this.Dmg(this.runDmg);
                this.timeUntilRunTick = this.runTickTime;
            }
        }
    }

    private void Rest()
    {
        if(this.rb.velocity.magnitude > 0f || this.isShielding) { return; }

        this.timeUntilHealTick -= Time.deltaTime;

        if(this.timeUntilHealTick < 0f)
        {
            this.Heal(this.healAmt);
            this.timeUntilHealTick = this.healTick;
        }
    }

    private void Die()
    {
        this.isAlive = false;
        this.isMovementLocked = true;
        this.movementInput = Vector2.zero;
        this.anim.SetTrigger("IsDead");
        this.audio.PlayOneShot(this.deathSFX);
        StartCoroutine(this.OnDeath());
    }

    IEnumerator OnDeath()
    {
        yield return new WaitForSeconds(1);
        this.sceneChange.FadeToLevel("Lose");
    }

   private void FixedUpdate()
    {
        if(this.isDashing)
        {
            this.velocityInput = (this.movementInput.magnitude > 0f ? this.movementInput.normalized : this.lastMovementInput.normalized) * this.dashVelocity;
        }
        else if(!this.isMovementLocked)
        {
            this.velocityInput = this.movementInput.normalized * (this.isRunning ? this.runVelocity : this.walkVelocity);
        }

        this.anim.SetFloat("VelocityX", this.velocityInput.x);
        this.anim.SetFloat("VelocityY", this.velocityInput.y);
        this.anim.SetBool("IsWalking", this.velocityInput.magnitude > 0f && !this.isDashing && !this.isKnockback);
        if (this.rb.velocity != this.velocityInput) { this.rb.velocity = this.velocityInput; }
    }

    public bool IsAlive()
    {
        return this.isAlive;
    }

    public bool HasHealth(float amt)
    {
        return this.health > amt;
    }

    public float GetHealthPercent()
    {
        return this.health / this.maxHealth;
    }

    public void Dmg(float amt)
    {
        this.health = Mathf.Clamp(this.health - amt, 0, this.maxHealth);
        this.UpdateHealthUI();
        if(this.health == 0f)
        {
            this.Die();
        }
    }

    public void Dmg(float amt, Vector2 knockback)
    {
        if(this.isShielding) { return; }
        this.Dmg(amt);
        if(!this.isAlive) { return; }
        this.isKnockback = true;
        this.isMovementLocked = true;
        this.knockbackTimerLeft = 0.1f;
        this.startFalshTime = Time.time;
        this.velocityInput = knockback.normalized * amt * 5f;
        this.audio.PlayOneShot(this.hurtSFX);
    }

    public void Flashing()
    {
        if(!this.isKnockback) { return; }
        if(this.knockbackTimerLeft <= 0f)
        {
            this.sprite.color = new Color(255, 255, 255);
            this.isKnockback = false;
            this.isMovementLocked = false;

            return;
        }
        float red = this.sprite.color.r == 255f ? 0f : 255f;
        this.sprite.color = new Color(red, 0, 0);
    }

    public void Heal(float amt)
    {
        this.health = Mathf.Clamp(this.health + amt, 0, this.maxHealth);
        this.UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if(!this.healthMask) { return; }
        this.healthMask.transform.localScale = new Vector2(1, this.GetHealthPercent());
    }
}
