using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CombatEnemy : MonoBehaviour
{
    [Header("Atributtes")] 
    public float totalHealth;
    public int attackDamage;
    public float movementSpeed;
    public float lookRadius;
    public float ColliderRadius;
    public float rotationSpeed;

    [Header("Components")] 
    private Animator anim;
    private CapsuleCollider capsule;
    private BoxCollider box;
    private NavMeshAgent agent;

    [Header("Others")] 
    private Transform player;
    private bool walking;
    private bool attacking;
    private bool hiting;
    private bool waitFor;
    private bool alive = true;
    private bool playerIsDead;

    [Header("Waypoints")] 
    public List<Transform> wayPoints = new List<Transform>();
    public int currentPathIndex;
    public float pathDistance;
    
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        box = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            //dentro do raio de ação
            if (distance <= lookRadius)
            {
                if (!attacking)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    anim.SetBool("Run Forward", true);
                    walking = true;
                }

                if (distance <= agent.stoppingDistance)
                {
                    //dentro do raio de ataque
                    StartCoroutine("Attack");
                    //LookTarget();
                }
                else
                {
                    attacking = false;
                }
            }
            else
            {
                //fora do raio de ação
                //agent.isStopped = true;
                anim.SetBool("Run Forward", false);
                walking = false;
                attacking = false;
                MoveToWayPoint();

            }
        }
    }

    void MoveToWayPoint()
    {
        if (alive)
        {
            if (wayPoints.Count > 0)
            {
                float distance = Vector3.Distance(wayPoints[currentPathIndex].position, transform.position);
                agent.destination = wayPoints[currentPathIndex].position;
                if (distance <= pathDistance)
                {
                    //parte para o próximo ponto
                    currentPathIndex = Random.Range(0, wayPoints.Count);

                }
            
                anim.SetBool("Run Forward", true);
                walking = true;
            }
        }
        
    }
    
    IEnumerator Attack()
    {
        if (!waitFor && !hiting && !playerIsDead)
        {
            waitFor = true;
            attacking = true;
            walking = false;
            anim.SetBool("Run Forward", false);
            anim.SetBool("Bite Attack", true);
            yield return new WaitForSeconds(1.1f);
            GetPlayer();
            //yield return new WaitForSeconds(1f);
            waitFor = false;
        }

        if (playerIsDead)
        {
            anim.SetBool("Run Forward", false);
            anim.SetBool("Bite Attack", false);
            walking = false;
            attacking = false;
            agent.isStopped = true;
        }
    }

    void GetPlayer()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * ColliderRadius) , ColliderRadius))
        {
            if(c.gameObject.CompareTag("Player"))
            {
                //aplicar dano no player
                c.gameObject.GetComponent<Player>().GetHit(attackDamage);
                playerIsDead = c.gameObject.GetComponent<Player>().isDead;
            }
        }
    }

    public void GetHit(float damage)
    {
        totalHealth -= damage;
        if (totalHealth > 0)
        {
            //inimigo vivo
            StopCoroutine("Attack");
            anim.SetTrigger("Take Damage");
            hiting = true;
            StartCoroutine("RecoveryFromHit");
        }
        else
        {
            //inimigo morre
            StartCoroutine("Die");

        }
    }

    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Run Forward", false);
        anim.SetBool("Bite Attack", false);
        waitFor = false;
        hiting = false;
    }

    IEnumerator Die()
    {
        alive = false;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    void LookTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position , lookRadius);
    }
}
