using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game.Classes
{
    internal class Window
    {
        public string Title { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public ConsoleColor BgColor { get; set; }
        public ConsoleColor LetterColor { get; set; }
        public Point MaxPoint { get; set; }
        public Point MinPoint { get; set; }
        private bool _resize = false;
        private bool _textResize = true;
        public static List<Point> GameOver = new List<Point>();

        public Window(string Title, ConsoleColor BgColor, ConsoleColor LetterColor) 
        {
            this.Title = Title;
            this.BgColor = BgColor;
            this.LetterColor = LetterColor;
            Init();
            CalculateFrame();
            DrawFrame();
            
        }

        public void CalculateFrame()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            int frameWidth = Width - 10;
            int frameHeight = Height - 6;
            MaxPoint = new Point((Width - frameWidth) / 2, (Height - frameHeight) / 2);
            MinPoint = new Point(MaxPoint.X + frameWidth - 1, MaxPoint.Y + frameHeight - 1);
        }

        public void ConsoleResize()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            while (_resize)
            {
                if(Width != Console.WindowWidth || Height != Console.WindowHeight)
                {
                    Width = Console.WindowWidth;
                    Height = Console.WindowHeight;
                    
                    CalculateFrame();
                    Console.Clear();
                    DrawFrame();
                }
            }
        }
        public bool ConsoleResizeEnable()
        {
            _resize = true;
            return _resize;
        }
        public bool ConsoleResizeDisable()
        {
            _resize = false;
            return _resize;
        }
        public void Init()
        {
            Console.Title = Title;
            Console.BackgroundColor = BgColor;
            Console.CursorVisible = false;
        }
        
        public void DrawFrame()
        {

            Console.ForegroundColor = LetterColor;
            for (int i = MaxPoint.X; i < MinPoint.X; i++)
            {
                Console.SetCursorPosition(i, MaxPoint.Y);
                Console.Write("═");
                Console.SetCursorPosition(i, MinPoint.Y);
                Console.Write("═");
            }
            for (int i = MaxPoint.Y; i < MinPoint.Y; i++)
            {
                Console.SetCursorPosition(MaxPoint.X, i);
                Console.Write("║");
                Console.SetCursorPosition(MinPoint.X, i);
                Console.Write("║");
            }
            Console.SetCursorPosition(MaxPoint.X, MaxPoint.Y);
            Console.Write("╔");
            Console.SetCursorPosition(MaxPoint.X, MinPoint.Y);
            Console.Write("╚");
            Console.SetCursorPosition(MinPoint.X, MaxPoint.Y);
            Console.Write("╗");
            Console.SetCursorPosition(MinPoint.X, MinPoint.Y);
            Console.Write("╝");
        }

        public void PrintCenteredText(string text, List<Point> gameOverPositions)
        {
            string[] lines = text.Split('\n');

            int frameWidth = MinPoint.X - MaxPoint.X - 1;
            int frameHeight = MinPoint.Y - MaxPoint.Y - 1;

            int startY = MaxPoint.Y + (frameHeight - lines.Length) / 2;

            if (startY < MaxPoint.Y)
            {
                startY = MaxPoint.Y;
            }

            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd();
                int startX = MaxPoint.X + (frameWidth - line.Length) / 2;

                if (startX < MaxPoint.X)
                {
                    startX = MaxPoint.X;
                }
                for (int j = 0; j < line.Length; j++)
                {
                    gameOverPositions.Add(new Point(startX + j, startY + i));
                }

                Console.SetCursorPosition(startX, startY + i);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(line);
            }
        }
        public void ResizeAndPrintCenteredText(string text, CancellationTokenSource token)
        {
            CalculateFrame();
            GameOver.Clear();
            PrintCenteredText(text, GameOver);

            Task.Run(() =>
            {
                while (_textResize && !token.IsCancellationRequested)
                {
                    if (Width != Console.WindowWidth || Height != Console.WindowHeight)
                    {
                        CalculateFrame();
                        Console.Clear();
                        PrintCenteredText(text, GameOver);
                    }

                    Thread.Sleep(50);
                }
            }, token.Token);
            
        }
        public void TextResizeEnable()
        {
            _textResize = true;
        }
        public void TextResizeDisable()
        {
            _textResize = false;
        }

    }
}
