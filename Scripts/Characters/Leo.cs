using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Leo : MonoBehaviour
{
    public enum AttackType { Throw, Hit, UseZippo, SpecialUsage, Shoot, None }

    [Header("Controller")]
    public GameObject currentController;

    [Header("Movement")]
    public Transform tf;
    public Transform directablePart;
    public Rigidbody2D rb;
    public float maxSpeed, currentSpeed;
    public float jumpForce;
    public Animator animator;
    public Transform walkSighter;
    public MoveDirection currentDirection;

    [Header("Combat")]
    public float health;
    public SpriteRenderer weapon;
    public AttackType attackType;
    public GameObject throwController;
    public BoxCollider2D damagingPart;
    private bool hitEnable = true;
    private Inventory.Item selectedWeapon;
    public Animator light;
    private bool lightOn;
    public Vector2 bounceForce;
    public UnityEngine.U2D.IK.LimbSolver2D leftHand;
    public Transform leftHandSolverTarget, leftHandSolverTarget_Separate;
    public Animation attackEffect;

    [Header("Indicators")]
    public TalkingBaloon talkingBaloon;
    public WeaponIndicator weaponIndicator;
    public Transform healthIndicator;
    public Animation panoramaPassingImage;

    public InventoryPanel inventoryPanel;
    public Inventory inventory;
    public SpriteMask[] lightMasks;

    public AudioSource audioSource;
    private AudioClip punchDamage;

    #region Movement
    public void Move(MoveDirection direction) 
    {
        Rotate(direction);

        float deltaTimeSpeed = currentSpeed * Time.deltaTime * 100;
        Vector2 rayDirection;
        if (direction == MoveDirection.Left)
            rayDirection = Vector2.left;
        else rayDirection = Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(walkSighter.position, rayDirection, 1, 1 << 4);

        if (hit && !hit.collider.isTrigger)
        {
            Debug.DrawRay(walkSighter.position, rayDirection, Color.red);
            float angle = (90 - Vector2.Angle(new Vector2(walkSighter.position.x, walkSighter.position.y) - hit.point, hit.normal)) * Mathf.Deg2Rad;
            float xForce = Mathf.Cos(angle) * rayDirection.x * deltaTimeSpeed;
            float yForce = Mathf.Sin(angle) * deltaTimeSpeed;

            if (Mathf.Abs(rb.velocity.x) < maxSpeed / 2)
                rb.AddForce(new Vector2(xForce, yForce));
        }
        else
        {
            Debug.DrawRay(walkSighter.position, rayDirection, Color.green);
            if(Mathf.Abs(rb.velocity.x) < maxSpeed)
                rb.AddForce(rayDirection * deltaTimeSpeed);
        }
    }
    public void Rotate(MoveDirection direction) 
    {
        currentDirection = direction;

        switch (direction) 
        {
            case MoveDirection.Left: directablePart.localScale = new Vector3(-1, 1, 1); break;
            case MoveDirection.Right: directablePart.localScale = new Vector3(1, 1, 1); break;
        }
    }
    public void Jump() 
    {
        if (IsGrounded())
        {
            animator.SetTrigger("Jump");
            rb.AddForce(new Vector2(0, jumpForce));
        }
    }
    public bool IsGrounded() 
    {
        return RaycastForGrounded(currentDirection);
    }
    private bool RaycastForGrounded(MoveDirection direction) 
    {
        float offset;
        if (direction == MoveDirection.Left)
            offset = 0.5f;
        else
            offset = -0.5f;
        Debug.DrawRay(walkSighter.position + new Vector3(offset, 1), new Vector3(0, -5, 0), Color.red, 1);

        RaycastHit2D[] hits = Physics2D.RaycastAll(walkSighter.position + new Vector3(offset, 1), Vector2.down, 1);
        Debug.Log(walkSighter.position);
        Debug.Log(hits.Length);
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if (!hit.collider.isTrigger && hit.collider.gameObject.name != "Leo")
                return true;
        }
        return false;
    }

    private IEnumerator _Jump() 
    {
        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(0.25f);
        rb.AddForce(new Vector2(0, jumpForce));
    }
    #endregion

    #region Combat
    public void SwitchWeapon(string weaponName) 
    {
        Debug.Log(weaponName);

        if (weaponName != "None")
        {
            Debug.Log("Select" + weaponName);
            weapon.sprite = Resources.Load(@"Items\" + weaponName, typeof(Sprite)) as Sprite;
            SetWeaponTransformValues(weaponName);
            selectedWeapon = inventory.GetItem(weaponName);
        }
        else
            weapon.sprite = null;

        attackType = GeneralVariables.GetAttackTypeOfWeapon(weaponName);
        if (attackType == AttackType.Hit)
            damagingPart.gameObject.GetComponent<Damaging>().damageUnit = GeneralVariables.GetDamageUnitOfWeapon(selectedWeapon.name);
    }
    private void SetWeaponTransformValues(string weaponName) 
    {
        GeneralVariables.WeaponTransformValues wtv = GeneralVariables.GetTransformValuesOfWeapon(weaponName);
        Transform weapon = this.weapon.gameObject.GetComponent<Transform>();
            
        weapon.localPosition = wtv.position;
        weapon.localScale = wtv.scale;
        weapon.localRotation = Quaternion.Euler(0, 0, wtv.zRotation);
    }

    public void Attack() 
    {
        switch (attackType) 
        {
            case AttackType.Hit: Hit(); break;
            case AttackType.Throw: GetReadyForThrow(); break;
            case AttackType.UseZippo: UseZippo(); break;
            case AttackType.SpecialUsage: SpecialUsage(selectedWeapon.name); break;
            case AttackType.Shoot: UseGun(); break;
        }
    }

    // HIT
    public void Hit() 
    { 
        if (hitEnable) 
        {
            if (animator.GetBool("Running"))
                StartCoroutine(hit_Running());
            else
                StartCoroutine(hit_Idle());
            hitEnable = false; 
        } 
    }
    private IEnumerator hit_Running() 
    {
        attackEffect.Play("AttackEffect");
        SeparateLeftHand();
        damagingPart.enabled = true;
        leftHandSolverTarget_Separate.gameObject.GetComponent<Animation>().Play("LeoLeftHandAttack");
        yield return new WaitForSeconds(0.166f);
        leftHandSolverTarget_Separate.gameObject.GetComponent<Animation>().Stop();
        StartCoroutine(moveTowardsToJoinedLeftHand());
        damagingPart.enabled = false;
        yield return new WaitForSeconds(0.2f);
        hitEnable = true;
    }
    private IEnumerator hit_Idle() 
    {
        attackEffect.Play("AttackEffect");
        animator.SetTrigger("Hit");         
        damagingPart.enabled = true;       
        yield return new WaitForSeconds(0.2f);
        damagingPart.enabled = false;
        yield return new WaitForSeconds(0.2f);
        hitEnable = true;
    }

    // THROW
    public void GetReadyForThrow() { SwitchController(throwController); animator.SetTrigger("GetReadyForThrow"); }

    // USE LIGHTER
    public void UseZippo() 
    {
        if (!lightOn)
        {
            Inventory.Item zippo = inventory.GetItem("Zippo");
            if (zippo.availability == 0)
            {
                UIMessenger uiMessenger = GameObject.Find("UIMessenger").GetComponent<UIMessenger>();
                uiMessenger.ShowMessage(uiMessenger.textArray[4], 1.5f);
            }
            else
            {
                animator.SetTrigger("UseLight");
                light.SetBool("Light", true);
                lightOn = true;
            }
        }
        else 
        {
            animator.SetTrigger("BurnOutLight");
            light.SetBool("Light", false);
            lightOn = false;
        }
    }

    // USE GUN
    public void UseGun() { StartCoroutine(useGun()); }
    public IEnumerator useGun() 
    {
        SeparateLeftHand();
        leftHandSolverTarget_Separate.gameObject.GetComponent<Animation>().Play("LeoLeftHandShoot");
        yield return new WaitForSeconds(0.25f);
        GameObject bullet = Instantiate(Resources.Load("Prefabs\\Bullet", typeof(GameObject)), weapon.gameObject.GetComponent<Transform>()) as GameObject;
        bullet.GetComponent<Transform>().localPosition = new Vector3(9.92f, 1.93f, 0);
        bullet.GetComponent<Bullet>().direction = currentDirection;
        yield return new WaitForSeconds(0.5f);
        leftHandSolverTarget_Separate.gameObject.GetComponent<Animation>().Stop();
        StartCoroutine(moveTowardsToJoinedLeftHand());
    }

    // SPECIAL USAGE
    public void SpecialUsage(string itemName) 
    {
        switch (itemName) 
        {
            case "Dynamite": GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().UseDynamite(); break;
            case "MazeBox": GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().UseMazeBox(); break;
            case "AlarmControl": GameObject.Find("Underground_1Events").GetComponent<Underground_1Events>().UseAlarmControl(); break;
        }
    }

    // HURT
    public void Hurt(MoveDirection bounceDirection, float damage) 
    {
        if (bounceDirection == MoveDirection.Right)
            rb.AddForce(bounceForce);
        else
            rb.AddForce(new Vector2(-bounceForce.x, bounceForce.y));

        health -= damage;
        FightPanel.SetLeoHealthBarValue((int)health);
//        healthIndicator.localScale = new Vector2(health / 100f, 1);

        if (health <= 0)
            Die(bounceDirection);
        else
            animator.SetTrigger("Hurt");
    }

    // DIE
    public void Die(MoveDirection bounceDirection) 
    {
        Debug.Log("Die");

        if (currentDirection == bounceDirection)
            animator.SetTrigger("FacedownDie");
        else
            animator.SetTrigger("Die");
        HideUI();

        Guard[] guards = GeneralVariables.GetAllGuards();

        foreach (Guard guard in guards)
            guard.StopGuard();

        StopLeo();
        StartCoroutine(RestartGameFromLastPanorama());
    }
    public IEnumerator RestartGameFromLastPanorama() 
    {
        yield return new WaitForSeconds(2);
        CloseFrontImage();
        yield return new WaitForSeconds(2);
        SettleLeo();
        health = 100;
//        healthIndicator.localScale = new Vector2(1, 1);

        Guard[] guards = GeneralVariables.GetAllGuards();
        foreach (Guard guard in guards) 
        {
            if (!guard.dead)
            {
                guard.ContinueGuard();
                guard.tf.position = guard.startPosition;
                guard.health = 100;
//                guard.HideHealthBar();
            }
        }
        animator.SetTrigger("Jump");

        yield return new WaitForSeconds(0.5f);
        ContinueLeo();
        OpenFrontImage();
        ShowUI();
    }
    private void SettleLeo() 
    {
        float offset;
        if (Camera.main.GetComponent<MultiLayer>().currentPanorama.gameObject.GetComponent<Transform>().position.x > PPR_Manager.lastStartPosition.position.x)
        {
            offset = 5;
            Rotate(MoveDirection.Right);
        }
        else
        {
            offset = -5;
            Rotate(MoveDirection.Left);
        }
        tf.position = PPR_Manager.lastStartPosition.position + new Vector3(offset, 0, 0);
    }
    private IEnumerator blinkDarkImage() 
    {
        Animation panoramaPassingImage = GameObject.Find("PanoramaPassingImage").GetComponent<Animation>();
        panoramaPassingImage.Play("FrontImage_Closing");
        yield return new WaitForSeconds(2);
        animator.SetTrigger("Jump");
        panoramaPassingImage.Play("FrontImage_OnChangedPanorama");
    }
    private void CloseFrontImage() { panoramaPassingImage.Play("FrontImage_Closing"); }
    private void OpenFrontImage() { panoramaPassingImage.Play("FrontImage_OnChangedPanorama"); }

    // LEFT HAND
    public IEnumerator moveTowardsToJoinedLeftHand()
    {
        while (Vector3.Distance(leftHandSolverTarget.position, leftHandSolverTarget_Separate.position) > 0.06f)
        {
            leftHandSolverTarget_Separate.position = Vector3.MoveTowards(leftHandSolverTarget_Separate.position, leftHandSolverTarget.position, 0.05f);
            yield return new WaitForEndOfFrame();
        }
        JoinLeftHand();
        yield return null;
    }
    public void SeparateLeftHand()
    {
        leftHand.GetChain(1).target = leftHandSolverTarget_Separate;
    }
    public void JoinLeftHand()
    {
        leftHand.GetChain(1).target = leftHandSolverTarget;
    }
    #endregion

    #region ControllerManager
    public void SwitchController(GameObject controller) 
    {
        StartCoroutine(switchController(controller));
    }
    private IEnumerator switchController(GameObject controller) 
    {
        currentController.GetComponent<Animation>().Play("HideWeaponIndicator");
        yield return new WaitForSeconds(0.25f);

        currentController.SetActive(false);

        currentController = controller;
        currentController.SetActive(true);
        currentController.GetComponent<Animation>().Play("ShowWeaponIndicator");
    }

    private void StopLeo() 
    {
        DefaultController controller = currentController.GetComponent<DefaultController>();

        controller.enabled = false;
        StartCoroutine(controller.OnUp_MoveLeft());
        StartCoroutine(controller.OnUp_MoveRight());
    }
    private void ContinueLeo() 
    {
        DefaultController controller = currentController.GetComponent<DefaultController>();
        controller.enabled = true;
    }
    #endregion

    #region MainFunctions
    private void OnApplicationQuit() 
    {
        DatabaseManager.DeleteDatabase();
    }
    private void Start() 
    {
        SetEnabledLightMasks(true);
        punchDamage = Resources.Load<AudioClip>(@"Audio\Character\PunchDamage");
        inventory = new Inventory();
        currentController = GameObject.Find("DefaultController");
        inventory = DatabaseManager.GetLeoInventory();
    }

    public void SetEnabledLightMasks(bool enabled) 
    {
        foreach (SpriteMask mask in lightMasks)
            mask.enabled = enabled;
    }
    #endregion

    #region CollisionEvents
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        switch (collision.collider.tag) 
        {
            case "Damaging": OnContactDamaging(collision); break;
        }
    }

    public void OnContactDamaging(Collision2D collision) 
    {
        Debug.Log("Damaging");

        MoveDirection direction;
        if (collision.contacts[0].point.x < tf.position.x)
            direction = MoveDirection.Right;
        else
            direction = MoveDirection.Left;

        Hurt(direction, collision.collider.GetComponent<Damaging>().damageUnit);
        Debug.Log(punchDamage == null);
        audioSource.PlayOneShot(Resources.Load<AudioClip>(@"Audio\Character\Attack\PunchDamage_" + Random.Range(0, 3)));
        if (collision.gameObject.tag == "Antagonist")
            collision.collider.enabled = false;
    }
    #endregion

    #region UserInterface
    public void HideUI() 
    {
        currentController.GetComponent<Animation>().Play("HideWeaponIndicator");
        currentController.SetActive(false);
//        GameObject.Find("HealthIndicator").GetComponent<Animation>().Play("HideWeaponIndicator");
//        if (weaponIndicator.gameObject.GetComponent<CanvasGroup>().alpha == 1)
//            weaponIndicator.gameObject.GetComponent<Animation>().Play("HideWeaponIndicator");
    }
    public void ShowUI() 
    {
        currentController.SetActive(true);
        currentController.GetComponent<Animation>().Play("ShowWeaponIndicator");
//        GameObject.Find("HealthIndicator").GetComponent<Animation>().Play("ShowWeaponIndicator");
//        if (weaponIndicator.gameObject.GetComponent<CanvasGroup>().alpha == 1)
//            weaponIndicator.gameObject.GetComponent<Animation>().Play("ShowWeaponIndicator");
    }
    #endregion

    public class Inventory 
    {
        public class Item
        {
            public string name; 
            public int count, availability;
            public int textID;

            public Item(string name, int count, int availability, int textID)
            {
                this.name = name; this.count = count; this.availability = availability; this.textID = textID;
            }
        }
        public List<Item> items = new List<Item>();

        public void AddItem(string name, int count, int textID) 
        {
            if (!items.Exists(delegate(Item im) { return im.name == name; }))
            {
                Item toBeAddedItem = new Item(name, count, 100, textID);
                items.Add(toBeAddedItem);
            }
            else { items.Find(delegate(Item im) { return im.name == name; }).count += count; }

            DatabaseManager.SetLeoInventory(this);
        }
        public void AddItem(string name, int count, int availability, int textID)
        {
            if (!items.Exists(delegate(Item im) { return im.name == name; }))
            {
                Item toBeAddedItem = new Item(name, count, availability, textID);
                items.Add(toBeAddedItem);
            }
            else { items.Find(delegate(Item im) { return im.name == name; }).count += count; }

            DatabaseManager.SetLeoInventory(this);
        }
        public void AddItem(Item item)
        {
            Debug.Log(items + ", " + item);
            if (!items.Exists(delegate(Item im) { return im.name == item.name; }))
            {                
                items.Add(item);
            }
            else { items.Find(delegate(Item im) { return im.name == item.name; }).count += item.count; }

            DatabaseManager.SetLeoInventory(this);
        }
        public bool ItemExists(string itemName) { return items.Exists(delegate(Item im) { return im.name == itemName; }); }

        public Item GetItem(string itemName) 
        {
            return items.Find(delegate(Item im) { return im.name == itemName; });
        }
    }
}
