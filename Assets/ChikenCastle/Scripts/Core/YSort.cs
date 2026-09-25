using UnityEngine;

public class YSort : MonoBehaviour
{
    [SerializeField] private int offset = 0;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder =
            Mathf.RoundToInt(-transform.position.y * 100) + offset;
    }
}