using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Guard : MonoBehaviour
{
    public enum State { Chase, Idle, Seeking }
    public State state = State.Idle;
    public AudioSource audioSource;

    [Header("Movement")]
    public Transform tf;
    public Transform directablePart;
    public Rigidbody2D rb;
    public float currentSpeed;
    public float maxSpeed;
    public MoveDirection currentDirection;
    public Animator anim;
    public Animation animation;
    public Vector2 startPosition;

    [Header("Chase")]
    public Transform target;
    public float targetLoseTime;
    public float targetDistance { get { return Mathf.Abs(target.position.x - tf.position.x); } }
    public float minTargetDistance;
    public bool meleeEnable { get { return targetDistance < minTargetDistance; } }
    public MoveDirection sideOfTarget { get { if (target.position.x < tf.position.x) return MoveDirection.Left; else return MoveDirection.Right; } }

    [Header("Sight")]
    public Transform eye;
    public float sightRange;
    public bool canSight { get { return targetDistance < sightRange; } }

    [Header("Combat")]
    public float health;
    public bool attackEnable;
    public float attackFrequency;
    public BoxCollider2D damagingPart;
    public Vector2 bounceForce;
    public Transform healthBar;
    public Animation healthBarBox;
    public Animation attackEffect;
    bool healthBarVisible;
    public string faceSprite;
    public IEnumerator onDie;
    public bool dead = false;

    [Header("StateChanger")]
    public IEnumerator onSetToChase;

    public void Update() 
    {
        Sight();

        switch (state) 
        {
            case State.Chase: Chase(); break;
            case State.Seeking: Seeking(); break;
        }
    }
    private void Start() 
    {
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), damagingPart);
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), target.GetComponent<BoxCollider2D>());

        startPosition = tf.position;
    }
    public void StopGuard() { SetToIdle(); this.enabled = false; }
    public void ContinueGuard() { this.enabled = true; }

    #region Movement
    public void Move(MoveDirection direction) 
    {
        Rotate(direction);
        switch (direction)
        {
            case MoveDirection.Left: if (rb.velocity.x > -maxSpeed) rb.AddForce(new Vector2(-currentSpeed, 0)); break;
            case MoveDirection.Right: if (rb.velocity.x < maxSpeed) rb.AddForce(new Vector2(currentSpeed, 0)); break;
        }
    }
    public void Rotate(MoveDirection direction) 
    {
        switch (direction) 
        {
            case MoveDirection.Left: directablePart.localScale = new Vector3(-1, 1, 1); break;
            case MoveDirection.Right: directablePart.localScale = new Vector3(1, 1, 1); break;
        }
    }
    #endregion

    #region Sight
    public void Sight()
    {
        bool targetFound = false;
        if (canSight)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(eye.position, (target.position - tf.position));
            Debug.DrawLine(eye.position, target.position, Color.red);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == target.gameObject) { targetFound = true; break; }
                else if (!hit.collider.isTrigger && hit.collider.gameObject != gameObject && hit.collider.gameObject.tag != "Antagonist") break;
            }
        }

        if (targetFound)
            SetToChase();
        else if (state == State.Chase)
        {
            SetToSeeking();
        }
    }
    #endregion

    #region Chase
    public void Chase() 
    {
        if (meleeEnable)
            Attack();
        else
            Move(sideOfTarget);
    }
    #endregion

    #region Seeking
    public void Seeking() 
    {
        Move(sideOfTarget);
    }
    #endregion

    #region Combat
    public void Attack() 
    {
        if (attackEnable)
            StartCoroutine(attack());
    }
    private IEnumerator attack() 
    {
        attackEffect.Play("AttackEffect");
        attackEnable = false;
        damagingPart.enabled = true;
        anim.SetTrigger("Hit");
        yield return new WaitForSeconds(0.5f);
        damagingPart.enabled = false;
        yield return new WaitForSeconds(attackFrequency);
        attackEnable = true;
    }

    public void ShowHealthBar() 
    {
        healthBarBox.Play("ShowHealthBarOfGuard");
        healthBarVisible = true;
    }
    public void HideHealthBar() 
    {
        healthBarBox.Play("HideHealthBarOfGuard");
        healthBarVisible = false;
    }
    public void SetHealthBarValue(float health) 
    {
        float xPos;
        xPos = (health / 15f) - 6.73f;

        healthBar.localPosition = new Vector3(xPos, 0, -0.1f);
    }
    public void Hurt(MoveDirection bounceDirection, float damage) 
    {
  //      if (!healthBarVisible)
  //          ShowHealthBar();

        if (bounceDirection == MoveDirection.Right)
            rb.AddForce(bounceForce);
        else
            rb.AddForce(new Vector2(-bounceForce.x, bounceForce.y));

        health -= damage;
   //     SetHealthBarValue(health);
        FightPanel.SetAntagonistHealthBarValue(gameObject.name, (int)health);

        if (health <= 0)
            Die(bounceDirection);
    }

    public void Die(MoveDirection bounceDirection) 
    {
        Debug.Log("Guard Die");
        if (bounceDirection != currentDirection)
            anim.SetTrigger("FacedownDie");
        else
            anim.SetTrigger("Die");

//        HideHealthBar();
        this.enabled = false;

        Invoke("PlayDestroyAnimation", 3);
        FightPanel.RemoveEnemy(gameObject.name, 1);
        Debug.Log("Enemy Removed Caused By Death");

        if (onDie != null)
            StartCoroutine(onDie);

        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<BoxCollider2D>().enabled = false;
        dead = true;
    }
    private void PlayDestroyAnimation() 
    {
        animation.Play("Guard_Destroy");
    }
    #endregion

    #region StateChanger
    public void SetToChase() 
    {
        if (state != State.Chase) { if (state == State.Idle) FightPanel.AddEnemy(gameObject.name, (int)health, faceSprite); state = State.Chase; OnChangedStateToChase(); }
    }
    private void OnChangedStateToChase()
    {
        anim.SetBool("Running", true); if (onSetToChase != null) StartCoroutine(onSetToChase);
    }

    public void SetToIdle() 
    {
        if (state != State.Idle) { state = State.Idle; OnChangedStateToIdle(); }
    }
    private void OnChangedStateToIdle() { anim.SetBool("Running", false); FightPanel.RemoveEnemy(gameObject.name, 0); Debug.Log("Enemy Removed Caused By Passing To Idle State"); }

    public void SetToSeeking() 
    {
        if (state != State.Seeking) 
        {
            state = State.Seeking;
            OnChangedStateToSeeking();
        }
    }
    private void OnChangedStateToSeeking() 
    {
        Invoke("SetToIdle", targetLoseTime);
    }
    #endregion

    #region CollisionEvents
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        Debug.Log("Guard Collide");
        switch (collision.collider.tag)
        {
            case "Damaging": OnContactDamaging(collision); Debug.Log("Guard Damage"); break;
        }
    }
    private void OnContactDamaging(Collision2D collision) 
    {
        MoveDirection eventDirection;
        if (collision.contacts[0].point.x < tf.position.x)
            eventDirection = MoveDirection.Right;
        else
            eventDirection = MoveDirection.Left;

        audioSource.PlayOneShot(Resources.Load<AudioClip>(@"Audio\Character\Attack\PunchDamage_" + Random.Range(0, 3)));
        if (collision.gameObject.name == "Leo")
            collision.collider.enabled = false;
        Hurt(eventDirection, collision.collider.GetComponent<Damaging>().damageUnit);
    }
    #endregion
}
