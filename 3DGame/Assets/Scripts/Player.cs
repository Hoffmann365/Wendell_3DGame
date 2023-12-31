using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    private CharacterController controller;
    public int totalHealth;
    public float speed;
    public float gravity;
    public float damage;

    private Animator anim;

    private Transform cam;

    private Vector3 moveDirection;

    public AudioSource hitsound;
    public AudioSource diesound;
    public AudioSource bgsound;
    public AudioSource atksound;

    public float smoothRotTime;
    private float turnSmoothVelocity;
    private bool isWalking;
    private bool waitFor;
    private bool hiting;
    public bool isDead;

    public float ColliderRadius;

    public List<Transform> enemyList = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        GameController.instance.UpdateLives(totalHealth);
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Move();
            GetMouseInput();
        }
        
    }

    void Move()
    {
        if (controller.isGrounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            if (direction.magnitude > 0)
            {
                if (!anim.GetBool("attacking"))
                {

                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, smoothRotTime);

                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                    moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;

                    anim.SetInteger("transition", 1);

                    isWalking = true;
                }
                else
                {
                    anim.SetBool("walking", false);
                    moveDirection = Vector3.zero;
                }
            }
            else if(isWalking)
            {
                anim.SetBool("walking", false);
                anim.SetInteger("transition", 0);
                moveDirection = Vector3.zero;
                
                isWalking = false;
            }

            
        }
        
        moveDirection.y -= gravity * Time.deltaTime;
        
        controller.Move(moveDirection * Time.deltaTime);
    }

    void GetMouseInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (anim.GetBool("walking"))
                {
                    anim.SetBool("walking", false);
                    anim.SetInteger("transition", 0);
                }

                if (!anim.GetBool("walking"))
                {
                    StartCoroutine("Attack");
                }
                
            }
        }
    }

    IEnumerator Attack()
    {
        if (!waitFor && !hiting)
        {
            waitFor = true;
            anim.SetBool("attacking", true);
            anim.SetInteger("transition", 2);

            yield return new WaitForSeconds(0.4f);
            
            GetEnemiesList();

            foreach (Transform e in enemyList)
            {
                CombatEnemy enemy = e.GetComponent<CombatEnemy>();
                if (enemy != null)
                {
                    enemy.GetHit(damage);
                }
            }
            atksound.Play();
            yield return new WaitForSeconds(1f);

            anim.SetInteger("transition", 0);
            anim.SetBool("attacking", false);
            waitFor = false;
        }
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * ColliderRadius), ColliderRadius))
        {
            if(c.gameObject.CompareTag("Enemy"))
            {
                enemyList.Add(c.transform);
            }
        }
    }
    
    public void GetHit(int damage)
    {
        totalHealth -= damage;
        GameController.instance.UpdateLives(totalHealth);
        if (totalHealth > 0)
        {
            //player vivo
            StopCoroutine("Attack");
            anim.SetInteger("transition", 3);
            hitsound.Play();
            hiting = true;
            StartCoroutine("RecoveryFromHit");
        }
        else
        {
            //player morre
            StartCoroutine("Die");

        }
    }

    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetInteger("transition", 0);
        waitFor = false;
        hiting = false;
        anim.SetBool("attacking", false);
    }

    IEnumerator Die()
    {
        isDead = true;
        Destroy(controller);
        bgsound.mute = true;
        diesound.Play();
        anim.SetTrigger("die");
        yield return new WaitForSeconds(4f);
        GameController.instance.RestartGame();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((transform.position + transform.forward), ColliderRadius);
    }
}
