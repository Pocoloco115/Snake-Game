using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Game.Interfaces;
using System.Formats.Tar;

namespace Game.Classes
{
    internal class Snake : IGameObject, IInputHandler
    {
        enum Directions
        {
            R, L, U, D
        }
        public LinkedListNode<Point> Head {  get; set; }
        public LinkedList<Point> Body { get; set; }
        public Window Window { get; set; }
        public ConsoleColor HeadColor { get; set; }
        public ConsoleColor BodyColor { get; set; }
        public Point LastPosition { get; set; }
        public bool eating { get; set; }
        private Directions _direction;
        private static readonly object _lock = new object();
        public Snake(LinkedListNode<Point> head, LinkedList<Point> body, Window window, ConsoleColor headColor, 
            ConsoleColor bodyColor)
        {
            Head = head;
            Body = body;
            Window = window;
            HeadColor = headColor;
            BodyColor = bodyColor;

            _direction = Directions.R;
        }
        public void ResetDirection()
        {
            _direction = Directions.R;
        }
        public void Draw()
        {
            Console.SetCursorPosition(Head.Value.X, Head.Value.Y);
            Console.WriteLine("█");
        }

        public void GetDirection()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if(key.Key == ConsoleKey.A && _direction != Directions.R)
                {
                    _direction = Directions.L;
                }
                if(key.Key == ConsoleKey.D && _direction != Directions.L)
                {
                    _direction = Directions.R;
                }
                if (key.Key == ConsoleKey.W && _direction != Directions.D)
                {
                    _direction = Directions.U;
                }
                if (key.Key == ConsoleKey.S && _direction != Directions.U)
                {
                    _direction = Directions.D;
                }
            }
        }

        public void BodyInit(int NumOfParts)
        {
            Body.Clear();
            int x = Head.Value.X - 1;
            for (int i = 0; i < NumOfParts; i++)
            {
                Console.SetCursorPosition(x, Head.Value.Y);
                Body.AddFirst(new Point(x, Head.Value.Y));
                Console.ForegroundColor = BodyColor;
                Console.Write("▓");
                x--;
            }

        }
        public void BodyMove()
        {
            Body.AddLast(LastPosition);
            Console.SetCursorPosition(LastPosition.X, LastPosition.Y);
            Console.ForegroundColor = BodyColor;
            Console.WriteLine("▓");
            if (eating)
            {
                eating = false;
                return;
            }
            Console.SetCursorPosition(Body.First.Value.X, Body.First.Value.Y);
            Console.Write(" ");
            Body.RemoveFirst();
        }
        public void Move()
        {
            GetDirection();
            Console.ForegroundColor = HeadColor;
            Console.SetCursorPosition(Head.Value.X, Head.Value.Y);
            LastPosition = Head.Value;
            Console.WriteLine(" ");
            switch (_direction)
            {
                case Directions.R:
                    Head.Value = new Point(Head.Value.X + 1, Head.Value.Y);
                    break;
                case Directions.L:
                    Head.Value = new Point(Head.Value.X - 1, Head.Value.Y);
                    break;
                case Directions.U:
                    Head.Value = new Point(Head.Value.X, Head.Value.Y - 1);
                    break;
                case Directions.D:
                    Head.Value = new Point(Head.Value.X, Head.Value.Y + 1);
                    break;
            }
            Draw();
        }

        public void DeathAnimation()
        {
            lock (_lock)
            {
                while (Body.Count > 0)
                {
                    Point point = Body.First.Value;

                    if (Window.GameOver.Contains(point))
                    {
                        Body.RemoveFirst();
                        continue;
                    }
                    Console.SetCursorPosition(point.X, point.Y);
                    if (point.Equals(Head.Value))
                    {
                        Console.ForegroundColor = HeadColor;
                        Console.WriteLine("█");
                    }
                    Console.WriteLine(" ");
                    Body.RemoveFirst();
                    Thread.Sleep(100);
                }
            }
        }
    }
}
