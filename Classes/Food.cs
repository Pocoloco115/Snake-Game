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
    internal class Food 
    {
        public Point Position { get; set; }
        public ConsoleColor Color { get; set; }
        public bool FoodExist { get; set; }
        public Food(ConsoleColor color, Point position)
        {
            Position = position;
            Color = color;
        }

        public void Draw()
        {
            if (FoodExist)
            {
                return;
            }
            else
            {
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.WriteLine("■");
                FoodExist = true;
            }
        }
    }
}
