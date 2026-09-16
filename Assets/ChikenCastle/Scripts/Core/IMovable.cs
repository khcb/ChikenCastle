using UnityEngine;

public interface IMovable
{
    void SetTarget(Transform target);
    void Stop();
    void Resume();
}