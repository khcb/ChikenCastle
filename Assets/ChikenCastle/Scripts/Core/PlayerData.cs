using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Инфо Игрока 🎮")]
        [SerializeField] private string playerName; // "Левый игрок" или "Правый игрок"

        [Header("Выбранные юниты для боя 🛡️")]
        [SerializeField] private List<UnitData> selectedUnits = new List<UnitData>();

        public string PlayerName => playerName;
        public List<UnitData> SelectedUnits => selectedUnits;
    }
