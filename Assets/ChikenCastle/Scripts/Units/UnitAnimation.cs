using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    private Animator animator;
    private string currentAnimation;
    private bool isAttacking; // Флаг блокировки во время атаки

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Play(string animationName)
    {
        // Если идет атака, переключать на другие анимации нельзя (кроме Смерти)
        if (isAttacking && animationName != "Death")
            return;

        // Если это та же самая анимация (например, Walk вызывается каждый кадр), 
        // мы просто выходим, чтобы не сбрасывать её с 0-го кадра непрерывно.
        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        animator.Play(animationName);
    }

    public void Idle() 
    {
        if (isAttacking) return;
        Play("Idle");
    }
    
    public void Walk() 
    {
        Debug.Log("Walk");
        if (isAttacking) return;
        Play("Walk");
    }

    public void Attack()
    {
        isAttacking = true;
        currentAnimation = "Attack"; // Сбрасываем, чтобы гарантированно запустить анимацию заново
        animator.Play("Attack", 0, 0f); // Атаку форсируем с 0-го кадра
        Debug.Log("Анимация атаки");
    }

    public void Death()
    {
        isAttacking = false;
        currentAnimation = "Death";
        animator.Play("Death", 0, 0f);
    }

    public void EndDeath()
    {
        gameObject.SetActive(false); 
    }

    // Вызывается в конце анимации атаки через Animation Event
    public void EndAttack()
    {
        isAttacking = false;
        currentAnimation = null; // Очищаем, чтобы юнит мог снова пойти или встать в Idle
    }
}