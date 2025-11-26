using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float targetRefreshTime = 2f;

    private Vector2 spawnPos;
    private Vector2 currentTarget;
    private float targetTimer;

    private void Awake()
    {
        spawnPos = transform.position;
        PickNewTarget();
    }

    private void Update()
    {
        // 一定時間ごとにターゲット更新
        targetTimer -= Time.deltaTime;
        if (targetTimer <= 0f || Vector2.Distance(transform.position, currentTarget) < 0.5f)
        {
            PickNewTarget();
        }

        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector2 newPos = Vector2.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    private void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        currentTarget = spawnPos + randomOffset;
        targetTimer = targetRefreshTime;
    }
}