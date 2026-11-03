
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private AbilityButtonUI abilityButtonPrefab;
    [SerializeField] private Transform buttonContainer;

    private readonly List<AbilityButtonUI> generatedButtons = new List<AbilityButtonUI>();

    private void Start()
    {
        GenerateButtons();
    }

    public void GenerateButtons()
    {
        if (abilityManager == null)
        {
            Debug.LogError("AbilityBarUI : ERREUR AbilityManager non assigne", this);
            return;
        }

        if (abilityButtonPrefab == null)
        {
            Debug.LogError("AbilityBarUI : ERREUR prefab AbilityButton non assigne", this);
            return;
        }

        if (buttonContainer == null)
        {
            Debug.LogError("AbilityBarUI : ERREUR conteneur de boutons non assigne", this);
            return;
        }

        // Securite au cas ou il y a les anciens boutons
        ClearGeneratedButtons();

        // Recuperation des competences
        IReadOnlyList<AbilityData> abilities = abilityManager.Abilities;

        // Generation automatique en fonction du nombre de competences
        for (int i = 0; i < abilities.Count; i++)
        {
            AbilityData ability = abilities[i];

            if (ability == null)
            {
                Debug.LogWarning($"AbilityBarUI : ERREUR competence manquante a l'index {i}", this);

                continue;
            }

            // Creation du bouton
            AbilityButtonUI newButton = Instantiate(abilityButtonPrefab, buttonContainer);

            newButton.name = $"AbilityButton_{ability.abilityName}";

            // Initialisation du bouton
            newButton.Setup(ability, abilityManager);

            generatedButtons.Add(newButton);
        }

        // Actualisation du layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer as RectTransform);

        Debug.Log($"AbilityBarUI : {generatedButtons.Count} boutons generes", this);
    }

    private void ClearGeneratedButtons()
    {
        foreach (AbilityButtonUI button in generatedButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }

        generatedButtons.Clear();
    }
}
