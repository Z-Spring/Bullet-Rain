using System.Collections;
using battle_player;
using pool;
using UnityEngine;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 200f;
    [SerializeField] private float lifeTime = 2f;
    // [SerializeField]public float bulletDamage = 10f;

    public Player firePlayer;
    private string targetTag;
    private Rigidbody rb;
    private Vector3 dir;
    private TrailRenderer trailRenderer;
    private LayerMask playerLayerMask;
    private const string PlayerLayerName = "player";

    public void Init(string playerTag, Vector3 startDir, float bulletLifeTime = 2f, float bulletSpeed = 120f)
    {
        targetTag = playerTag;
        lifeTime = bulletLifeTime;
        speed = bulletSpeed;
        dir = startDir;
    }

    public void Init(Vector3 startDir, float bulletLifeTime = 2f, float bulletSpeed = 120f)
    {
        StopAllCoroutines();
        StartCoroutine(TimeUp());

        lifeTime = bulletLifeTime;
        speed = bulletSpeed;
        dir = startDir;

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = true;

        rb = GetComponent<Rigidbody>();
        rb.velocity = dir * speed;

        playerLayerMask = LayerMask.GetMask(PlayerLayerName);
    }

    private IEnumerator TimeUp()
    {
        yield return new WaitForSeconds(lifeTime);
        if (gameObject)
        {
            GameObjectPool.Instance.ReturnGameObject<Bullet>(gameObject);
        }
    }

    private void FixedUpdate()
    {
        ColliderDetect();
    }

    private void ColliderDetect()
    {
        if (IsHit(out RaycastHit hit))
        {
            HandleHit(hit);
        }
    }

    private bool IsHit(out RaycastHit hit)
    {
        float distance = speed * Time.fixedDeltaTime;
        Ray ray = new Ray(transform.position, dir);
        return Physics.Raycast(ray, out hit, distance, playerLayerMask);
    }

    private void HandleHit(RaycastHit hit)
    {
        transform.position = hit.point;
        SpawnBloodEffect();

        if (firePlayer is null)
        {
            return;
        }

        BasePlayer hitPlayer = hit.collider.GetComponent<BasePlayer>();
        if (hitPlayer is null)
        {
            return;
        }
//
        if (hitPlayer.camp == firePlayer.camp)
        {
            return;
        }

        SendHitMsg(firePlayer, hitPlayer);
    }

    private void SpawnBloodEffect()
    {
        GameObject bloodEffect = GameObjectPool.Instance.GetGameObject<BloodEffect>();

        bloodEffect.transform.position = transform.position;
        bloodEffect.transform.rotation = Quaternion.identity;

        ParticleSystem bloodPS = bloodEffect.GetComponent<ParticleSystem>();
        if (bloodPS is null)
        {
            return;
        }

        bloodPS.Play();
        StartCoroutine(ReturnBloodEffect(bloodEffect));
    }

    private IEnumerator ReturnBloodEffect(GameObject bloodEffect)
    {
        yield return new WaitForSeconds(1f);
        GameObjectPool.Instance.ReturnGameObject<BloodEffect>(bloodEffect);
    }

    private void SendHitMsg(BasePlayer player, BasePlayer hitPlayer)
    {
        if (hitPlayer is null || player is null)
        {
            return;
        }

        // 如果不是自己发出的炮弹，不发送消息
        if (player.id != StartGameMain.id)
        {
            return;
        }

        var pos = transform.position;
        MsgHit msg = new MsgHit()
        {
            id = player.id,
            targetId = hitPlayer.id,

            x = pos.x,
            y = pos.y,
            z = pos.z
        };
        Debug.Log("发送子弹消息");
        NetManager.Send(msg);
    }
}