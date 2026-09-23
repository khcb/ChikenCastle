using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private PlayerResources playerResources;
    [SerializeField] private TMP_Text resourceText;

    private void OnEnable()
    {
        if (playerResources != null)
        {
            playerResources.OnResourcesChanged += UpdateUI;
        }
    }

    private void Start()
    {
        if (playerResources != null)
        {
            UpdateUI(playerResources.CurrentResources);
        }
    }

    private void OnDisable()
    {
        if (playerResources != null)
        {
            playerResources.OnResourcesChanged -= UpdateUI;
        }
    }

    private void UpdateUI(int amount)
    {
        resourceText.text = $"{amount}$";
    }
}