using System;
using TowerArena.Abilities;
using TowerArena.Enemies;
using TowerArena.Units;
using UnityEngine;
using UnityEngine.UI;

namespace TowerArena.UI
{
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private Button button;
        [SerializeField] private Dropdown abilityDropdown;

        private UnitBase unit;
        private string abilityId;
        private AbilityDefinition abilityDefinition;
        private Action<UnitAbilityRequest> callback;

        private void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnClicked);
            }

            if (abilityDropdown != null)
            {
                abilityDropdown.onValueChanged.AddListener(OnDropdownChanged);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
            }

            if (abilityDropdown != null)
            {
                abilityDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
            }
        }

        private void Update()
        {
            if (button != null && unit != null && !string.IsNullOrEmpty(abilityId))
            {
                button.interactable = unit.CanActivateAbility(abilityId);
            }
        }

        public void Bind(UnitBase targetUnit, Action<UnitAbilityRequest> onActivate)
        {
            unit = targetUnit;
            callback = onActivate;

            if (unit == null)
            {
                abilityId = null;
                abilityDefinition = null;
                UpdateLabel();
                gameObject.SetActive(false);
                return;
            }

            PopulateDropdown();
            UpdateLabel();
            gameObject.SetActive(!string.IsNullOrEmpty(abilityId) && abilityDefinition != null);
        }

        private void UpdateLabel()
        {
            if (label == null)
            {
                return;
            }

            if (abilityDefinition == null)
            {
                label.text = string.Empty;
            }
            else
            {
                label.text = abilityDefinition.DisplayName;
            }

            if (abilityDropdown != null)
            {
                abilityDropdown.interactable = unit != null && abilityDropdown.options.Count > 1;
            }
        }

        private void OnClicked()
        {
            if (unit == null || abilityDefinition == null || string.IsNullOrEmpty(abilityId))
            {
                return;
            }

            var request = new UnitAbilityRequest
            {
                Unit = unit,
                AbilityId = abilityId,
                UseTargetPosition = abilityDefinition.RequiresTargetPosition,
                TargetPosition = abilityDefinition.RequiresTargetPosition ? unit.transform.position : Vector3.zero,
                UseTargetBalloon = abilityDefinition.RequiresTargetBalloon,
                TargetBalloon = null
            };

            callback?.Invoke(request);
        }

        private void PopulateDropdown()
        {
            abilityId = null;
            abilityDefinition = null;

            if (abilityDropdown == null || unit == null)
            {
                return;
            }

            abilityDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();

            foreach (var id in unit.GrantedAbilities)
            {
                if (!unit.AbilityDefinitions.TryGetValue(id, out var definition))
                {
                    continue;
                }

                options.Add(definition.DisplayName);
                if (string.IsNullOrEmpty(abilityId))
                {
                    abilityId = id;
                    abilityDefinition = definition;
                }
            }

            abilityDropdown.AddOptions(options);
            abilityDropdown.value = 0;
            abilityDropdown.RefreshShownValue();
        }

        private void OnDropdownChanged(int index)
        {
            if (unit == null || abilityDropdown == null)
            {
                return;
            }

            var selectedName = abilityDropdown.options[index].text;
            foreach (var id in unit.GrantedAbilities)
            {
                if (unit.AbilityDefinitions.TryGetValue(id, out var definition) && definition.DisplayName == selectedName)
                {
                    abilityId = id;
                    abilityDefinition = definition;
                    UpdateLabel();
                    break;
                }
            }
        }
    }

    public struct UnitAbilityRequest
    {
        public UnitBase Unit;
        public string AbilityId;
        public bool UseTargetPosition;
        public Vector3 TargetPosition;
        public bool UseTargetBalloon;
        public BalloonInstance TargetBalloon;
    }
}
