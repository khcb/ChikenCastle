using UnityEngine;

public class AggroArea : MonoBehaviour
{
    private UnitAttack _unitAttack;

    private void Awake()
    {
        _unitAttack = GetComponentInParent<UnitAttack>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Если вошедший объект — враг
        if (_unitAttack != null && other.CompareTag(_unitAttack.EnemyTag))
        {
            _unitAttack.OnEnemyDetected(other.transform);
        }
    }
}