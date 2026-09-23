using UnityEngine;

[CreateAssetMenu(
    fileName = "NewGathererUnit",
    menuName = "Game/Units/Gatherer Unit Data"
)]
public class GathererData : UnitData
{
    [Header("Сбор")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float gatherRange = 1f;
    [SerializeField] private float gatherTime = 2f;
    [SerializeField] private int gatherAmount = 10;

    public float DetectionRange => detectionRange;
    public float GatherRange => gatherRange;
    public float GatherTime => gatherTime;
    public int GatherAmount => gatherAmount;
}