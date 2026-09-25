using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [Header("Глобальные настройки для создаваемых юнитов")]
    [SerializeField] private Team team;
    [SerializeField] private PlayerResources playerResources;
    [SerializeField] private Transform enemyCastle;
    [SerializeField] private Transform homeCastle;

    public GameObject CreateUnit(UnitData data, Vector3 position)
    {
        if (data == null || data.UnitPrefab == null)
        {
            Debug.LogError("FACTORY: Некорректные данные UnitData или отсутствует префаб!");
            return null;
        }

        GameObject unitObject = Instantiate(data.UnitPrefab, position, Quaternion.identity);
        UnitBase unit = unitObject.GetComponent<UnitBase>();

        if (unit == null)
        {
            Debug.LogError($"FACTORY: На префабе {data.UnitName} отсутствует компонент UnitBase!");
            Destroy(unitObject);
            return null;
        }

        // Инкапсулированная базовая настройка
        unit.SetTeam(team);
        unit.SetDefaultTarget(enemyCastle);

        // Вместо жесткой проверки типов через if (gatherer != null),
        // правильный SOLID подход — использовать интерфейс инициализации,
        // но сохраняя вашу структуру, выносим эту логику сюда, освобождая Spawner:
        GathererUnit gatherer = unitObject.GetComponent<GathererUnit>();
        if (gatherer != null)
        {
            gatherer.SetPlayerResources(playerResources);
            gatherer.SetHomeBase(homeCastle);
        }

        return unitObject;
    }
}
