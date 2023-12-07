using System;
using System.Collections.Generic;
using battle_player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OnPlayFireSoundUnityEvent : UnityEvent<EnemyController>
{
}

public class OnPlayFootstepSoundUnityEvent : UnityEvent<EnemyController>
{
}

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance {get; private set;}
    // public  UnityEvent<> OnPlayerDie;
    public NavMeshAgent agent;
    public Transform bulletPrefab;
    [FormerlySerializedAs("weaponSlot")] public Transform firePoint;
    public Transform attackTarget;
    public OnPlayFireSoundUnityEvent onPlayFireSound;
    public OnPlayFootstepSoundUnityEvent onPlayFootstepSound;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float nextShootTime;
    [SerializeField] private float shootInterval = 0.3f;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float maxAttackDistance = 13f;
    [SerializeField] private float maxChaseDistance = 16f;
    [SerializeField] private Image bloodBar;

    private Damageable damageable;
    private int patrolIndex ;
    private Transform viewIndicator;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // private Player player;

    [Space(10)] [Header("RayParams")] [SerializeField]
    private float maxScanDistance = 16f;

    private enum AIState
    {
        Patrol,
        Chase,
        Attack,
        Die
    }

    private AIState aiState;

    private void Awake()
    {
        // Instance = this;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 使对象持久
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 销毁新的实例
        }

        onPlayFireSound = new OnPlayFireSoundUnityEvent();
        onPlayFootstepSound = new OnPlayFootstepSoundUnityEvent();
    }

    private void Start()
    {
        aiState = AIState.Patrol;
        bloodBar.fillAmount = 1;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        if (agent.avoidancePriority == 0)
        {
            agent.avoidancePriority = Random.Range(30, 60);
        }

        viewIndicator = transform.Find("ViewIndicator");
        meshFilter = viewIndicator.GetComponent<MeshFilter>();
        meshRenderer = viewIndicator.GetComponent<MeshRenderer>();
        damageable = GetComponent<Damageable>();
        damageable.OnDie.AddListener(OnDie);
    }


    private void Update()
    {
        // if (agent && !agent.isStopped)
        // {
        //     onPlayFootstepSound.Invoke(this);
        // }
        switch (aiState)
        {
            case AIState.Patrol:
                agent.speed = walkSpeed;
                Patrol();

                break;
            case AIState.Chase:
                agent.speed = runSpeed;
                Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
            case AIState.Die:
                break;
        }
    }

    private void Patrol()
    {
        Debug.Log("patrol");

        Transform target = TargetScan();
        if (target && Vector3.Distance(transform.position, target.position) < maxAttackDistance)
        {
            Debug.Log("< maxAttackDistance");
            attackTarget = target;
            aiState = AIState.Attack;
        }
        else if (target && Vector3.Distance(transform.position, target.position) < maxChaseDistance)
        {
            Debug.Log("< maxChaseDistance");
            attackTarget = target;
            aiState = AIState.Chase;
        }


        if (patrolPoints.Count == 0)
        {
            return;
        }

        if (agent.isStopped)
        {
            agent.isStopped = false;
            agent.SetDestination(patrolPoints[patrolIndex].position);
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (patrolPoints.Count == 1)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, patrolPoints[0].rotation, 0.3f);
                return;
            }

            patrolIndex = (patrolIndex + 1) % patrolPoints.Count;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void Attack()
    {
        Debug.Log("attack");
        // todo； 这里先注释掉，Player单例有点问题
        // if (Player.Instance.IsDie())
        // {
        //     aiState = AIState.Patrol;
        //     agent.isStopped = false;
        //     attackTarget = null;
        //     return;
        // }

        // if (attackTarget is null)
        // {
        //     aiState = AIState.Patrol;
        //     return;
        // }

        Transform target = TargetScan();
        if (target is null)
        {
            Debug.Log("111");
            aiState = AIState.Patrol;
            attackTarget = null;
            // aiState = AIState.Chase;
            agent.isStopped = false;
            return;
        }

        attackTarget = target;
        if (Vector3.Distance(attackTarget.position, transform.position) > maxAttackDistance)
        {
            // todo : 下面这句哪里有问题
            aiState = AIState.Chase;
            // agent.isStopped = false;
            // agent.SetDestination(attackTarget.position);
            return;
        }


        agent.isStopped = true;
        transform.LookAt(attackTarget.position);
        // transform.rotation = Quaternion.Lerp(transform.rotation, attackTarget.rotation, 0.3f);

        // agent.isStopped = true;
        Shoot();
    }

    private void Chase()
    {
        Debug.Log("Chase");
        // todo； 这里先注释掉，单例有点问题

        // if (Player.Instance.IsDie())
        // {
        //     aiState = AIState.Patrol;
        //     agent.isStopped = false;
        //     attackTarget = null;
        //     return;
        // }


        // if (attackTarget is null)
        // {
        //     Debug.Log("attackTarget is null");
        //     aiState = AIState.Patrol;
        //     return;
        // }

        if (Vector3.Distance(transform.position, attackTarget.position) > maxChaseDistance)
        {
            aiState = AIState.Patrol;
            agent.isStopped = false;
            attackTarget = null;
            return;
        }

        Transform target = TargetScan();
        if (target is null)
        {
            Debug.Log("target is null");
            Debug.Log(attackTarget is null); // false
            aiState = AIState.Patrol;
            agent.isStopped = false;
            attackTarget = null;
            return;
        }

        if (Vector3.Distance(transform.position, target.position) < maxAttackDistance)
        {
            attackTarget = target;
            aiState = AIState.Attack;
            // agent.isStopped = true;
            return;
        }

        // 追击过程中，如果目标位置发生变化，重新设置目标位置
        if (Vector3.Distance(target.position, agent.destination) > 0.5f)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }


    private Transform TargetScan()
    {
        List<Vector3> points = new();
        GenerateRay(points);
        DrawRayRangeImage(points);
        return GetPlayerFromCollider();
    }

    private void GenerateRay(List<Vector3> points)
    {
        Vector3 offset = new Vector3(0, 0.5f, 0);
        points.Add(offset);
        for (int i = -60; i < 60; i += 4)
        {
            Vector3 forwardV = Quaternion.Euler(0, i, 0) * transform.forward;
            Ray ray = new Ray(transform.position + offset, forwardV);
            if (!Physics.Raycast(ray, out RaycastHit hit, maxScanDistance))
            {
                Vector3 localForwardV = transform.InverseTransformVector(forwardV);
                points.Add(localForwardV * maxScanDistance + offset);
            }
            else
            {
                Vector3 localV = transform.InverseTransformPoint(hit.point);
                points.Add(localV + offset);
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
    }

    private void DrawRayRangeImage(List<Vector3> points)
    {
        List<int> tris = new();
        for (int i = 2; i < points.Count; i++)
        {
            tris.Add(0);
            tris.Add(i - 1);
            tris.Add(i);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshFilter.mesh = mesh;
    }

    private Transform GetPlayerFromCollider()
    {
        Vector3 offset = new Vector3(0, 0.5f, 0);
        Collider[] colliders = new Collider[10];
        int colliderNums = Physics.OverlapSphereNonAlloc(transform.position, maxScanDistance, colliders,
            LayerMask.GetMask("player"));

        for (int i = 0; i < colliderNums; i++)
        {
            Collider c = colliders[i];


            // 从当前角色到目标的向量
            Vector3 pos = c.gameObject.transform.position - transform.position;
            if (Vector3.Angle(transform.forward, pos) > 60)
            {
                continue;
            }

            // 很重要，还要判断Player是否没被墙挡住
            Ray ray = new Ray(transform.position + offset, pos);
            if (!Physics.Raycast(ray, out RaycastHit hit, maxScanDistance))
            {
                continue;
            }

            if (hit.collider != c)
            {
                continue;
            }

            if (c.name == "Player")
            {
                Debug.Log(c.name);
                // Debug.Break();
            }

            if (c.CompareTag("Player"))
            {
                return c.transform;
            }

            // targets.Add(c.transform);
        }

        // if (targets.Count > 0)
        // {
        //     return targets[0];
        // }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxScanDistance);
    }

    private void Shoot()
    {
        if (Time.time < nextShootTime)
        {
            return;
        }

        onPlayFireSound?.Invoke(this);
        Vector3 dir = transform.forward;
        dir.y = 0;
        dir.Normalize();

        Transform bullet = Instantiate(bulletPrefab, firePoint);
        bullet.localEulerAngles = Vector3.zero;
        bullet.localPosition = Vector3.zero;

        Bullet b = bullet.GetComponent<Bullet>();
        b.Init("Player", dir, 0.2f);

        nextShootTime = Time.time + shootInterval;
    }

    private void OnDie()
    {
        if (aiState == AIState.Die)
        {
            return;
        }

        aiState = AIState.Die;
        agent.isStopped = true;
        agent.enabled = false;
        transform.Rotate(-90, 0, 0);
        Destroy(gameObject, 5f);
    }

    private bool IsDie()
    {
        return damageable.hp <= 0;
    }
}