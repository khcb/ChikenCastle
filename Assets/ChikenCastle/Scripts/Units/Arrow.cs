using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Визуал")]
    [SerializeField] private Transform arrowSprite;
    [SerializeField] private Transform shadow;

    [Header("Полет")]
    [SerializeField] private float flightTime = 0.6f;
    [SerializeField] private float arcHeight = 1.5f;

    private Vector2 startPosition;
    private Vector2 targetPosition;

    private float currentTime;
    private float damage;

    private IDamageable target;


    public void Launch(
        Vector2 start,
        Vector2 targetPosition,
        float damage,
        IDamageable damageTarget)
    {
        startPosition = start;
        this.targetPosition = targetPosition;

        this.damage = damage;
        target = damageTarget;

        currentTime = 0f;

        transform.position = startPosition;
    }


    private void Update()
    {
        currentTime += Time.deltaTime;

        float progress = currentTime / flightTime;

        if (progress >= 1f)
        {
            HitTarget();
            return;
        }

        // Положение стрелы на земле
        Vector2 groundPosition = Vector2.Lerp(
            startPosition,
            targetPosition,
            progress
        );

        // Высота полета
        float height =
            4f * arcHeight * progress * (1f - progress);

        // Позиция самой стрелы
        transform.position =
            groundPosition + Vector2.up * height;

        // Поворот стрелы по направлению движения
        RotateArrow(progress);

        // Тень остается на земле
        if (shadow != null)
        {
            shadow.position = groundPosition;
        }
    }


    private void RotateArrow(float progress)
    {
        Vector2 direction =
            targetPosition - startPosition;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        if (arrowSprite != null)
        {
            arrowSprite.rotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    }


    private void HitTarget()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}