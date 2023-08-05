using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using Characters.Models;
using Characters.Player;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    private static GameUIController instance;

    public static GameUIController Instance
    {
        get
        {
            if (instance == null)
            {
                var inst = FindObjectOfType<GameUIController>();
                instance = inst;
            }

            return instance;
        }
    }
    
    [SerializeField] private Transform playerUnitsGrid;
    [SerializeField] private Transform enemyUnitsGrid;

    [SerializeField] private UnitInfoDisplay playerDisplay;
    [SerializeField] private UnitInfoDisplay enemyDisplay;

    [SerializeField] private GameObject playerUnitPrefab;
    [SerializeField] private GameObject enemyUnitPrefab;

    [SerializeField] private Sprite[] unitPortraits;

    public void SetupGameView()
    {
        playerDisplay.Show();
        enemyDisplay.Show();
    }

    public void SpawnCharacter(Character character)
    {
        if (character is PlayerCharacter)
        {
            var player = Instantiate(playerUnitPrefab, playerUnitsGrid);
            var unitPortrait = player.GetComponent<UnitPortrait>();
            unitPortrait.Setup(playerDisplay, character, GetPlayerSprite(character));
            character.portrait = unitPortrait;
        }
        else if (character is EnemyCharacter)
        {
            var enemy = Instantiate(enemyUnitPrefab, enemyUnitsGrid);
            var unitPortrait = enemy.GetComponent<UnitPortrait>();
            unitPortrait.Setup(enemyDisplay, character, GetEnemySprite(character));
            character.portrait = unitPortrait;
        }
    }

    private Sprite GetPlayerSprite(Character character)
    {
        if (character is Grunt) return unitPortraits[0];
        if (character is Jumpship) return unitPortraits[1];
        if (character is Tank) return unitPortraits[2];

        return null;
    }
    
    private Sprite GetEnemySprite(Character character)
    {
        if (character is Drone) return unitPortraits[3];
        if (character is Dreadnought) return unitPortraits[4];
        if (character is CommandUnit) return unitPortraits[5];

        return null;
    }

    public void UpdatePlayerInfo(Character character)
    {
        playerDisplay.UpdateInfo(character);
    }
    
    public void UpdateEnemyInfo(Character character)
    {
        enemyDisplay.UpdateInfo(character);
    }

    public void DeactivePlayer(Character character)
    {
        playerDisplay.DeactivateCharacter(character);
    }
    
    public void DeactiveEnemy(Character character)
    {
        enemyDisplay.DeactivateCharacter(character);
    }
}
