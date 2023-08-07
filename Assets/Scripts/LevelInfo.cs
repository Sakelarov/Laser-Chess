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
        Dictionary<(int, int), GameObject> players = new Dictionary<(int, int), GameObject>()
        {
            { (1, 3), gruntPrefab}, { (0, 5), jumpshipPrefab}, { (1, 6), tankPrefab}
        };

        foreach (var kvp in players)
        {
            int x = kvp.Key.Item1;
            int y = kvp.Key.Item2;
            yield return new WaitForSeconds(delayBetweenSpawning);
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(x, y, kvp.Value));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);

        (int, int)[] dronePsns = { (4, 3), (4, 5) };
        foreach (var psn in dronePsns)
        {
            var drone = (Drone)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dronePrefab);
            drones.Add(drone);
            enemyUnits.Add(drone);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }
        
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
        Dictionary<(int, int), GameObject> players = new Dictionary<(int, int), GameObject>()
        {
            { (1, 1), gruntPrefab}, { (1, 6), gruntPrefab}, { (0, 3), jumpshipPrefab}, { (0, 4), tankPrefab}
        };

        foreach (var kvp in players)
        {
            int x = kvp.Key.Item1;
            int y = kvp.Key.Item2;
            yield return new WaitForSeconds(delayBetweenSpawning);
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(x, y, kvp.Value));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);

        (int, int)[] dronePsns = { (6, 7), (5, 0), (4, 1) };
        foreach (var psn in dronePsns)
        {
            var drone = (Drone)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dronePrefab);
            drones.Add(drone);
            enemyUnits.Add(drone);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        (int, int)[] dreadPsns = { (6, 6), (7, 4) };
        foreach (var psn in dreadPsns)
        {
            var dread = (Dreadnought)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dreadnoughtPrefab);
            dreadnoughts.Add(dread);
            enemyUnits.Add(dread);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        var command = (CommandUnit)BoardManager.SpawnPiece(7, 7, commandUnitPrefab);
        commandUnits.Add(command);
        enemyUnits.Add(command);

        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }
    
    public static IEnumerator SetupLevel3(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits, List<Drone> drones,
        List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        Dictionary<(int, int), GameObject> players = new Dictionary<(int, int), GameObject>()
        {
            { (1, 0), gruntPrefab}, { (1, 7), gruntPrefab}, { (0, 3), jumpshipPrefab}, { (0, 6), jumpshipPrefab}, { (0, 0), tankPrefab}, { (0, 7), tankPrefab}
        };
        
        foreach (var kvp in players)
        {
            int x = kvp.Key.Item1;
            int y = kvp.Key.Item2;
            yield return new WaitForSeconds(delayBetweenSpawning);
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(x, y, kvp.Value));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);

        (int, int)[] dronePsns = { (5, 2), (5, 3), (5, 4),  (5, 5) };
        foreach (var psn in dronePsns)
        {
            var drone = (Drone)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dronePrefab);
            drones.Add(drone);
            enemyUnits.Add(drone);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        (int, int)[] dreadPsns = { (5, 0), (5, 1), (5, 6), (5, 7) };
        foreach (var psn in dreadPsns)
        {
            var dread = (Dreadnought)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dreadnoughtPrefab);
            dreadnoughts.Add(dread);
            enemyUnits.Add(dread);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        (int, int)[] commandPsns = { (7, 4), (6, 3)};
        foreach (var psn in commandPsns)
        {
            var command = (CommandUnit)BoardManager.SpawnPiece(psn.Item1, psn.Item2, commandUnitPrefab);
            commandUnits.Add(command);
            enemyUnits.Add(command);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }

    public static IEnumerator ClassicalChess(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        Dictionary<(int, int), GameObject> players = new Dictionary<(int, int), GameObject>()
        {
            { (1, 0), gruntPrefab}, { (1, 1), gruntPrefab}, { (1, 2), gruntPrefab}, { (1, 3), gruntPrefab}, 
            { (1, 4), gruntPrefab}, { (1, 5), gruntPrefab}, { (1, 6), gruntPrefab}, { (1, 7), gruntPrefab}, 
            { (0, 0), jumpshipPrefab}, { (0, 1), jumpshipPrefab}, { (0, 6), jumpshipPrefab}, { (0, 7), jumpshipPrefab},
            { (0, 2), tankPrefab}, { (0, 3), tankPrefab}, { (0, 4), tankPrefab}, { (0, 5), tankPrefab},
        };

        foreach (var kvp in players)
        {
            int x = kvp.Key.Item1;
            int y = kvp.Key.Item2;
            yield return new WaitForSeconds(delayBetweenSpawning);
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(x, y, kvp.Value));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);

        (int, int)[] dronePsns = { (6, 0), (6, 1), (6, 2),  (6, 3), (6, 4), (6, 5), (6, 6),  (6, 7) };
        foreach (var psn in dronePsns)
        {
            var drone = (Drone)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dronePrefab);
            drones.Add(drone);
            enemyUnits.Add(drone);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        (int, int)[] dreadPsns = { (7, 0), (7, 1), (7, 2), (7, 5), (7, 6), (7, 7) };
        foreach (var psn in dreadPsns)
        {
            var dread = (Dreadnought)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dreadnoughtPrefab);
            dreadnoughts.Add(dread);
            enemyUnits.Add(dread);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        (int, int)[] commandPsns = { (7, 3), (7, 4)};
        foreach (var psn in commandPsns)
        {
            var command = (CommandUnit)BoardManager.SpawnPiece(psn.Item1, psn.Item2, commandUnitPrefab);
            commandUnits.Add(command);
            enemyUnits.Add(command);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }
    
    public static IEnumerator GruntsWar(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        Dictionary<(int, int), GameObject> players = new Dictionary<(int, int), GameObject>()
        {
            { (0, 0), gruntPrefab}, { (0, 2), gruntPrefab}, { (0, 4), gruntPrefab}, { (0, 6), gruntPrefab},
            { (1, 1), gruntPrefab}, { (1, 3), gruntPrefab}, { (1, 5), gruntPrefab}, { (1, 7), gruntPrefab},
        };
        
        foreach (var kvp in players)
        {
            int x = kvp.Key.Item1;
            int y = kvp.Key.Item2;
            yield return new WaitForSeconds(delayBetweenSpawning);
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(x, y, kvp.Value));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);

        (int, int)[] dronePsns = { (4, 0), (4, 2), (4, 4),  (4, 6), (5, 1), (5, 3), (5, 5), (5, 7), (6, 0), (6, 2), (6, 4), (6, 6) };
        foreach (var psn in dronePsns)
        {
            var drone = (Drone)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dronePrefab);
            drones.Add(drone);
            enemyUnits.Add(drone);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }
        
        (int, int)[] commandPsns = { (7, 3)};
        foreach (var psn in commandPsns)
        {
            var command = (CommandUnit)BoardManager.SpawnPiece(psn.Item1, psn.Item2, commandUnitPrefab);
            commandUnits.Add(command);
            enemyUnits.Add(command);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }
        
        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }
    
    public static IEnumerator ThreeManArmy(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        Dictionary<(int, int), GameObject> players = new Dictionary<(int, int), GameObject>()
        {
            { (0, 3), tankPrefab}, { (0, 4), tankPrefab}, { (0, 5), tankPrefab}
        };
        
        foreach (var kvp in players)
        {
            int x = kvp.Key.Item1;
            int y = kvp.Key.Item2;
            yield return new WaitForSeconds(delayBetweenSpawning);
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(x, y, kvp.Value));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);
        
        (int, int)[] dreadPsns = { (5, 0), (5, 1), (5, 2), (5, 3), (5, 4), (5, 5), (5, 6), (5, 7) };
        foreach (var psn in dreadPsns)
        {
            var dread = (Dreadnought)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dreadnoughtPrefab);
            dreadnoughts.Add(dread);
            enemyUnits.Add(dread);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }
        
        (int, int)[] commandPsns = { (7, 0), (7, 7)};
        foreach (var psn in commandPsns)
        {
            var command = (CommandUnit)BoardManager.SpawnPiece(psn.Item1, psn.Item2, commandUnitPrefab);
            commandUnits.Add(command);
            enemyUnits.Add(command);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }
        
        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
    }
    
    public static IEnumerator HardcoreLevel(List<PlayerCharacter> playerUnits, List<EnemyCharacter> enemyUnits,
        List<Drone> drones, List<Dreadnought> dreadnoughts, List<CommandUnit> commandUnits, UnityEvent allEnemiesSpawned)
    {
        Dictionary<(int, int), GameObject> players = new Dictionary<(int, int), GameObject>()
        {
            { (0, 2), gruntPrefab}, { (1, 3), gruntPrefab}, { (0, 4), gruntPrefab},
            { (0, 1), jumpshipPrefab}, { (0, 3), jumpshipPrefab}, { (0, 5), jumpshipPrefab}, { (0, 7), jumpshipPrefab},
            { (1, 0), jumpshipPrefab}, { (1, 2), jumpshipPrefab}, { (1, 4), jumpshipPrefab}, { (1, 6), jumpshipPrefab},
        };
        
        foreach (var kvp in players)
        {
            int x = kvp.Key.Item1;
            int y = kvp.Key.Item2;
            yield return new WaitForSeconds(delayBetweenSpawning);
            playerUnits.Add((PlayerCharacter)BoardManager.SpawnPiece(x, y, kvp.Value));
        }
        yield return new WaitForSeconds(delayAfterPlayerUnitsSpawned);
        
        (int, int)[] dronePsns =
        {
            (4, 0), (4, 2), (4, 4), (4, 6), (6, 0), (6, 2), (6, 4), (6, 6)
        };
        foreach (var psn in dronePsns)
        {
            var drone = (Drone)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dronePrefab);
            drones.Add(drone);
            enemyUnits.Add(drone);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        (int, int)[] dreadPsns = { (5, 1), (5, 3), (5, 5), (5, 7) };
        foreach (var psn in dreadPsns)
        {
            var dread = (Dreadnought)BoardManager.SpawnPiece(psn.Item1, psn.Item2, dreadnoughtPrefab);
            dreadnoughts.Add(dread);
            enemyUnits.Add(dread);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        (int, int)[] commandPsns = { (7, 1), (7, 6)};
        foreach (var psn in commandPsns)
        {
            var command = (CommandUnit)BoardManager.SpawnPiece(psn.Item1, psn.Item2, commandUnitPrefab);
            commandUnits.Add(command);
            enemyUnits.Add(command);
            yield return new WaitForSeconds(delayBetweenSpawning);
        }

        allEnemiesSpawned.Invoke();
        GameUIController.Instance.ActivatePlayerTurn();
        BoardManager.Instance.IsPlayerTurn = true;
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
