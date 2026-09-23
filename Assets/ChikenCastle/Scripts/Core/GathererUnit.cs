using UnityEngine;

public class GathererUnit : UnitBase
{
    private enum GathererState
    {
        GoingToResource,
        Gathering,
        Returning
    }

    private GathererData gathererData;

    private IResourceSource currentResource;
    private PlayerResources playerResources;

    private Transform homeBase;

    private GathererState currentState;

    private float gatherTimer;
    private int carriedResource;

    protected override void Awake()
    {
        base.Awake();

        gathererData = unitData as GathererData;

        currentState = GathererState.GoingToResource;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GathererState.GoingToResource:
                UpdateGoingToResource();
                break;

            case GathererState.Gathering:
                UpdateGathering();
                break;

            case GathererState.Returning:
                UpdateReturning();
                break;
        }
    }

    private void UpdateGoingToResource()
    {
        if (currentResource == null)
        {
            FindResource();

            if (currentResource == null)
            {
                StopMove();
                return;
            }
        }

        Component resourceComponent =
            currentResource as Component;

        if (resourceComponent == null)
        {
            currentResource = null;
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            resourceComponent.transform.position
        );

        if (distance > gathererData.GatherRange)
        {
            MoveTo(resourceComponent.transform.position);
            return;
        }

        StopMove();

        gatherTimer = 0f;
        currentState = GathererState.Gathering;
    }

    private void UpdateGathering()
    {
        StopMove();

        gatherTimer += Time.deltaTime;

        if (gatherTimer < gathererData.GatherTime)
            return;

        carriedResource =
            currentResource.Gather(
                gathererData.GatherAmount
            );

        if (carriedResource <= 0)
        {
            currentResource = null;
            currentState = GathererState.GoingToResource;
            return;
        }

        currentState = GathererState.Returning;
    }

    private void UpdateReturning()
    {
        if (homeBase == null)
        {
            Debug.LogWarning(
                "GathererUnit: не назначена Home Base!"
            );

            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            homeBase.position
        );

        if (distance > gathererData.GatherRange)
        {
            MoveTo(homeBase.position);
            return;
        }

        StopMove();

        DeliverResource();

        Die();
    }

    private void DeliverResource()
    {
        if (playerResources == null)
        {
            Debug.LogWarning("GathererUnit: PlayerResources не назначен!");
            return;
        }

        playerResources.AddResources(carriedResource);

        carriedResource = 0;
    }

    private void FindResource()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(
            transform.position,
            gathererData.DetectionRange
        );

        float closestDistance = Mathf.Infinity;
        IResourceSource closestResource = null;

        foreach (Collider2D obj in objects)
        {
            IResourceSource resource =
                obj.GetComponentInParent<IResourceSource>();

            if (resource == null)
                continue;

            if (!resource.HasResource)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                obj.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestResource = resource;
            }
        }

        currentResource = closestResource;
    }

    public void SetPlayerResources(PlayerResources resources)
{
    playerResources = resources;
}

    public void SetHomeBase(Transform homeBase)
    {
        this.homeBase = homeBase;
    }
}