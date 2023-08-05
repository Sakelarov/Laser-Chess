using System;
using System.Collections.Generic;
using System.Linq;
using Characters.Models;
using Characters.Player;
using DG.Tweening;
using Grid;
using UnityEngine;
using UnityEngine.UIElements;

namespace Characters.Enemy
{
    public class CommandUnit : EnemyCharacter
    {
        private enum MovePriotiy
        { 
            Low = 1,
            Medium = 2,
            High = 4
        }

        private enum MoveType
        {
            Stay,
            Left,
            Right
        }

        private List<Grunt> grunts = new List<Grunt>();
        private List<Jumpship> jumpships = new List<Jumpship>();
        private List<Tank> tanks = new List<Tank>();

        private Cell leftCell, rightCell;
        private bool canMoveLeft, canMoveRight;

        private Dictionary<MoveType, int> moveTypes = new Dictionary<MoveType, int>();
        
        private Animator anim;
        private int paramRun, paramDeath, paramDamage;
        
        private readonly float moveDuration = 1;
        
        public override void Setup(Cell cell)
        {
            base.Setup(cell);
            
            GetPlayers();
            
            anim = GetComponentInChildren<Animator>();
            paramRun = Animator.StringToHash("run");
            paramDeath = Animator.StringToHash("death");
            paramDamage = Animator.StringToHash("damage");
        }

        private void GetPlayers()
        {
            Bm.ForEachCell((cell) =>
            {
                if (cell.IsOccupied)
                {
                    if (cell.Character is Grunt grunt) grunts.Add(grunt);
                    else if (cell.Character is Jumpship jumpship) jumpships.Add(jumpship);
                    else if (cell.Character is Tank tank) tanks.Add(tank);
                }
            });
        }

        public override bool TryMove(ref float delay)
        {
            GetAdjacentCells();
            GetMoveTypes();

            if (moveTypes.Count > 0)
            {
                var direction = moveTypes.OrderByDescending(x => x.Value).First(x => x.Value > 0);

                if (direction.Key == MoveType.Right)
                {
                    Move(rightCell);
                    delay = moveDuration;
                    return true;
                }

                if (direction.Key == MoveType.Left)
                {
                    Move(leftCell);
                    delay = moveDuration;
                    return true;
                }

                else return false;
            }
            
            
            return false;
        }

        private void GetAdjacentCells()
        {
            var ownPsn = Location.Coordinates;
            
            leftCell = BoardManager.TryGetCell(ownPsn.x, ownPsn.y - 1);
            if (leftCell != null && !leftCell.IsOccupied) canMoveLeft = true;
            else canMoveLeft = false;
            
            rightCell = BoardManager.TryGetCell(ownPsn.x, ownPsn.y + 1);
            if (rightCell != null && !rightCell.IsOccupied) canMoveRight = true;
            else canMoveRight = false;
        }

        private void GetMoveTypes()
        {
            moveTypes = new Dictionary<MoveType, int>();
            
            foreach (var grunt in grunts) GetGruntMoves(grunt);
            foreach (var jumpship in jumpships) GetJumpshipMoves(jumpship);
            foreach (var tank in tanks) GetTankMoves(tank);
        }

        private void GetGruntMoves(Grunt grunt) // Try to avoid diagonal attacks of grunt
        {
            var gruntPsn = grunt.Location.Coordinates;
            var ownPsn = Location.Coordinates;
            int xDistance = Mathf.Abs(ownPsn.x - gruntPsn.x);
            int yDistance = Mathf.Abs(ownPsn.y - gruntPsn.y);
            
            if (xDistance < yDistance)
            {
                TryGoAway(gruntPsn, ownPsn, MovePriotiy.Medium);
            }
            else if (xDistance > yDistance && !TryGoClose(gruntPsn, ownPsn, MovePriotiy.Medium)) 
            {
                AddStay(MovePriotiy.Medium);
            }
        }

        private void GetJumpshipMoves(Jumpship jumpship) // Try to block movement of Jumpship or go away from him
        {
            var jumpshipPsn = jumpship.Location.Coordinates;
            var ownPsn = Location.Coordinates;
            int xDistance = Mathf.Abs(ownPsn.x - jumpshipPsn.x);
            int yDistance = Mathf.Abs(ownPsn.y - jumpshipPsn.y);
            int manhattanDistance = Mathf.Max(xDistance, yDistance);

            if (manhattanDistance == 1 && xDistance == 1) 
            {
                TryGoAway(jumpshipPsn, ownPsn, MovePriotiy.High);
            }
            else if (manhattanDistance == 2)
            {
                if ((xDistance == 1 && yDistance == 2) || (xDistance == 2 && yDistance == 1))
                {
                    AddStay(MovePriotiy.High);
                }
                else if (xDistance == 0)
                {
                    TryGoAway(jumpshipPsn, ownPsn, MovePriotiy.High);
                }
                else if (yDistance == 0 || (xDistance == 2 && yDistance == 2))
                {
                    TryAddLeftOrRight(MovePriotiy.High);
                }
            }
            else if (manhattanDistance == 3)
            {
                if (xDistance == 3 && yDistance == 1)
                {
                    TryAddLeftOrRight(MovePriotiy.High);
                }
                else if (yDistance == 3 && xDistance == 1)
                {
                    TryGoAway(jumpshipPsn, ownPsn, MovePriotiy.High);
                }
            }
        }

        private void GetTankMoves(Tank tank) // Try to avoid orthogonal attacks of Tank
        {
            var tankPsn = tank.Location.Coordinates;
            var ownPsn = Location.Coordinates;
            int xDistance = Mathf.Abs(ownPsn.x - tankPsn.x);
            int yDistance = Mathf.Abs(ownPsn.y - tankPsn.y);

            if (yDistance >= 3 && xDistance >= 4)
            {
                TryGoAway(tankPsn, ownPsn, MovePriotiy.High);
            }
        }

        private void AddStay(MovePriotiy priority)
        {
            if (moveTypes.ContainsKey(MoveType.Stay)) moveTypes[MoveType.Stay] += (int)priority;
            else moveTypes.Add(MoveType.Stay, (int)priority);
        }

        private bool TryGoClose(Vector2Int enemy, Vector2Int ownPsn, MovePriotiy priority)
        {
            if (enemy.y < ownPsn.y) return TryAddLeft(priority); 
            
            if (enemy.y > ownPsn.y) return TryAddRight(priority);

            return false;
        }
        private bool TryGoAway(Vector2Int enemy, Vector2Int ownPsn, MovePriotiy priority)
        {
            if (enemy.y > ownPsn.y) return TryAddLeft(priority); 
            
            if (enemy.y < ownPsn.y) return TryAddRight(priority);

            return false;
        }

        private bool TryAddLeft(MovePriotiy priority)
        {
            if (canMoveLeft)
            {
                if (moveTypes.ContainsKey(MoveType.Left)) moveTypes[MoveType.Left] += (int)priority;
                else moveTypes.Add(MoveType.Left, (int)priority);
                return true;
            }

            return false;
        }

        private bool TryAddRight(MovePriotiy priority)
        {
            if (canMoveRight)
            {
                if (moveTypes.ContainsKey(MoveType.Right)) moveTypes[MoveType.Right] += (int)priority;
                else moveTypes.Add(MoveType.Right, (int)priority);
                return true;
            }

            return false;
        }

        private bool TryAddLeftOrRight(MovePriotiy priotiy)
        {
            if (TryAddLeft(priotiy) || TryAddRight(priotiy)) return true;
            
            return false;
        }

        private void Move(Cell cell)
        {
            RegisterMove(cell);
            
            anim.SetBool(paramRun, true);
            var pos = transform.position;
            DOVirtual.Vector3(pos, cell.Position, moveDuration, value => transform.position = value)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() => anim.SetBool(paramRun, false));
        }

        public override bool TryAttack(ref float delay)
        {
            return false;
        }
        
        public override void Die()
        {
            base.Die();
            anim.SetTrigger(paramDeath);
            Bm.CheckPlayerWin();
        }

        public override void GetDamaged()
        {
            GameUIController.Instance.UpdateEnemyInfo(this);
            anim.SetTrigger(paramDamage);
        }
    }
}
