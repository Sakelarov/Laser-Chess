using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using Characters.Models;
using UnityEngine;
using UnityEngine.Events;

public static class LevelInfo 
{
    private static readonly float delayBetweenSpawning = 0.2f;
    private static readonly float delayAfterPlayerUnitsSpawned = 0.5f;
    
    private static GameObject gruntPrefab;
    private static GameObject jumpshipPrefab;
    private static GameObject tankPrefab;
    private static GameObject dronePrefab;
    private static GameObject dreadnoughtPrefab;
    private static GameObject commandUnitPrefab;
    
    private static readonly string gruntPath = "Characters/Grunt";
    private static readonly string jumpshipPath = "Characters/Jumpship";
    private static readonly string tankPath = "Characters/Tank";
    private static readonly string dronePath = "Characters/Drone";
    private static readonly string dreadnoughtPath = "Characters/Dreadnought";
    private static readonly string commandPath = "Characters/CommandUnit";

    public static void Init()
    {
        gruntPrefab = (GameObject)Resources.Load(gruntPath);
        jumpshipPrefab = (GameObject)Resources.Load(jumpshipPath);
        tankPrefab = (GameObject)Resources.Load(tankPath);
        dronePrefab = (GameObject)Resources.Load(dronePath);
        dreadnoughtPrefab = (GameObject)Resources.Load(dreadnoughtPath);
        commandUnitPrefab = (GameObject)Resources.Load(commandPath);
    }
    
    public static IEnumerator SetupLevel1(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits, List<Drone> drones,
        List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(1, 1, gruntPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(1, 6, jumpshipPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(0, 4, tankPrefab));
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);

        var drone = (Drone)BoardManager.SpawnPiece(4, 3, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);
        drone = (Drone)BoardManager.SpawnPiece(4, 5, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);

        var dread = (Dreadnought)BoardManager.SpawnPiece(6, 6, dreadnoughtPrefab);
        dreadnoughts.Add(dread);
        enemyUnits.Add(dread);
        yield return new WaitForSeconds(delayBetweenSpawning);

        var command = (CommandUnit)BoardManager.SpawnPiece(6, 2, commandUnitPrefab);
        commandUnits.Add(command);
        enemyUnits.Add(command);

        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }
    
    public static IEnumerator SetupLevel2(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits, List<Drone> drones,
        List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(1, 1, gruntPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(1, 6, gruntPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(0, 3, jumpshipPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(0, 4, tankPrefab));
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);
        
        var drone = (Drone)BoardManager.SpawnPiece(6, 7, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);
        drone = (Drone)BoardManager.SpawnPiece(5, 0, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);
        drone = (Drone)BoardManager.SpawnPiece(4, 1, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);

        var dread = (Dreadnought)BoardManager.SpawnPiece(6, 6, dreadnoughtPrefab);
        dreadnoughts.Add(dread);
        enemyUnits.Add(dread);
        yield return new WaitForSeconds(delayBetweenSpawning);
        dread = (Dreadnought)BoardManager.SpawnPiece(7, 4, dreadnoughtPrefab);
        dreadnoughts.Add(dread);
        enemyUnits.Add(dread);

        var command = (CommandUnit)BoardManager.SpawnPiece(7, 3, commandUnitPrefab);
        commandUnits.Add(command);
        enemyUnits.Add(command);

        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }
    
    public static IEnumerator SetupLevel3(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits, List<Drone> drones,
        List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(1, 1, gruntPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(1, 6, gruntPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(0, 3, jumpshipPrefab));
        yield return new WaitForSeconds(delayBetweenSpawning);
        playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(0, 4, tankPrefab));
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);
        
        var drone = (Drone)BoardManager.SpawnPiece(6, 7, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);
        drone = (Drone)BoardManager.SpawnPiece(5, 0, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);
        drone = (Drone)BoardManager.SpawnPiece(4, 1, dronePrefab);
        drones.Add(drone);
        enemyUnits.Add(drone);
        yield return new WaitForSeconds(delayBetweenSpawning);
        
        var dread = (Dreadnought)BoardManager.SpawnPiece(6, 6, dreadnoughtPrefab);
        dreadnoughts.Add(dread);
        enemyUnits.Add(dread);
        yield return new WaitForSeconds(delayBetweenSpawning);
        dread = (Dreadnought)BoardManager.SpawnPiece(7, 4, dreadnoughtPrefab);
        dreadnoughts.Add(dread);
        enemyUnits.Add(dread);
    
        var command = (CommandUnit)BoardManager.SpawnPiece(7, 3, commandUnitPrefab);
        commandUnits.Add(command);
        enemyUnits.Add(command);
    
        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }

    public static IEnumerator ClassicalChess(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        yield return new WaitForSeconds(delayBetweenSpawning);
    }
    
    public static IEnumerator GruntsWar(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        yield return new WaitForSeconds(delayBetweenSpawning);
    }
    
    public static IEnumerator ThreeManArmy(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        yield return new WaitForSeconds(delayBetweenSpawning);
    }
    
    public static IEnumerator HardcoreLevel(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        yield return new WaitForSeconds(delayBetweenSpawning);
    }
    
    public static IEnumerator Custom(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned, int[] unitsQuantity)
    {
        for (int i = 0; i < unitsQuantity[0]; i++)
        {
            yield return new WaitForSeconds(delayBetweenSpawning);
            var cell = BoardManager.Instance.GetRandomPlayerSpawnCell();
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(cell.Coordinates.x, cell.Coordinates.y, gruntPrefab));
        }
        for (int i = 0; i < unitsQuantity[1]; i++)
        {
            yield return new WaitForSeconds(delayBetweenSpawning);
            var cell = BoardManager.Instance.GetRandomPlayerSpawnCell();
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(cell.Coordinates.x, cell.Coordinates.y, jumpshipPrefab));
        }
        for (int i = 0; i < unitsQuantity[2]; i++)
        {
            yield return new WaitForSeconds(delayBetweenSpawning);
            var cell = BoardManager.Instance.GetRandomPlayerSpawnCell();
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(cell.Coordinates.x, cell.Coordinates.y, tankPrefab));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);

        for (int i = 0; i < unitsQuantity[3]; i++)
        {
            yield return new WaitForSeconds(delayBetweenSpawning);
            var cell = BoardManager.Instance.GetRandomEnemySpawnCell();
            var drone = (Drone)BoardManager.SpawnPiece(cell.Coordinates.x, cell.Coordinates.y, dronePrefab);
            drones.Add(drone);
            enemyUnits.Add(drone);
        }
        for (int i = 0; i < unitsQuantity[4]; i++)
        {
            yield return new WaitForSeconds(delayBetweenSpawning);
            var cell = BoardManager.Instance.GetRandomEnemySpawnCell();
            var dread = (Dreadnought)BoardManager.SpawnPiece(cell.Coordinates.x, cell.Coordinates.y, dreadnoughtPrefab);
            dreadnoughts.Add(dread);
            enemyUnits.Add(dread);
        }
        for (int i = 0; i < unitsQuantity[5]; i++)
        {
            yield return new WaitForSeconds(delayBetweenSpawning);
            var cell = BoardManager.Instance.GetRandomEnemySpawnCell();
            var command = (CommandUnit)BoardManager.SpawnPiece(cell.Coordinates.x, cell.Coordinates.y, commandUnitPrefab);
            commandUnits.Add(command);
            enemyUnits.Add(command);
        }

        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }
}
