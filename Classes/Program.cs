using System;
using Game.Classes;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Security.Principal;

static class GameHandler
{
    static Random rnd = new Random();

    static Window window = new Window("Game", ConsoleColor.Red, ConsoleColor.White);

    static Snake snake = new Snake(new LinkedListNode<Point>(new Point(10, 10)), new LinkedList<Point>(), window,
        ConsoleColor.Black, ConsoleColor.White);

    static Food food = new Food(ConsoleColor.Black, new Point(rnd.Next(window.MaxPoint.X + 1, window.MinPoint.X - 1),
        rnd.Next(window.MaxPoint.Y + 1, window.MinPoint.Y - 1)));

    static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    static bool alive = true;

    private static Task? resizeTask;
    private static readonly object resizeLock = new object();

    static void Main(string[] args)
    {
        window.ConsoleResizeDisable();
        window.TextResizeEnable();
        MainMenu();
    }
    static ConsoleKey GetValidkey(ConsoleKey[] validKeys)
    {
        ConsoleKey key = new ConsoleKey();
        while (!validKeys.Contains(key))
        {
            key = Console.ReadKey(true).Key;
        }
        return key;
    }
    private static void CancelAndResetToken()
    {
        if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        cancellationTokenSource = new CancellationTokenSource();
    }

    static void ResetGame()
    {
        Console.Clear();
        CancelAndResetToken();
        window.ConsoleResizeDisable();
        Window.GameOver.Clear();
        snake.Body.Clear();
        snake.Head.Value = new Point(10, 10);
        snake.ResetDirection();
        snake.eating = false;
        alive = true;
        food.FoodExist = false;
    }
    static void MainMenu()
    {
        ResetGame();
        window.ResizeAndPrintCenteredText(Menu.Title, cancellationTokenSource);
        ConsoleKey key = GetValidkey(new ConsoleKey[]  { ConsoleKey.Escape, ConsoleKey.Enter });
        if (key.Equals(ConsoleKey.Enter))
        {
            ResetGame();
            Start();
        }
        if (key.Equals(ConsoleKey.Escape))
        {
            Environment.Exit(0);
        }
    }
    private static void MonitorConsoleResize()
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            window.ConsoleResize();
            Thread.Sleep(50);
        }
    }
    private static void StartResizeTask()
    {
        lock (resizeLock)
        {
            if (resizeTask == null || resizeTask.IsCompleted)
            {
                resizeTask = Task.Run(() => MonitorConsoleResize(), cancellationTokenSource.Token);
            }
        }
    }
    static void Start()
    {
        Console.Clear();
        window.TextResizeDisable();
        window.CalculateFrame();
        window.DrawFrame();
        window.TextResizeDisable();
        window.ConsoleResizeEnable();
        CancelAndResetToken();
        StartResizeTask();
        snake.BodyInit(2);
        GetFoodPosition();
        Update();
        Console.ReadKey();
    }

    static void Update()
    {
        alive = true;
        while (alive)
        {
            snake.Move();
            snake.BodyMove();
            OnCollisionEnter();
            if(!alive) break;
            Thread.Sleep(75);
        }
    }

    static void GetFoodPosition()
    {
        food.FoodExist = false;
        food.Position = new Point(rnd.Next(window.MaxPoint.X + 1, window.MinPoint.X - 1),
        rnd.Next(window.MaxPoint.Y + 1, window.MinPoint.Y - 1));

        foreach (var item in snake.Body)
        {
            if (food.Position.Equals(item))
            {
                Console.SetCursorPosition(food.Position.X, food.Position.Y);
                Console.WriteLine("▓");
                GetFoodPosition();
            }
            food.Draw();
            food.FoodExist = true;
        }
    }

    static void OnCollisionEnter()
    {
        if(snake.Head.Value.X.Equals(window.MinPoint.X))
        {
            Console.SetCursorPosition(snake.Head.Value.X, snake.Head.Value.Y);
            Console.WriteLine("║");
            snake.Head.Value = new Point(window.MaxPoint.X + 1, snake.Head.Value.Y);
        }
        if(snake.Head.Value.X.Equals(window.MaxPoint.X))
        {
            Console.SetCursorPosition(snake.Head.Value.X, snake.Head.Value.Y);
            Console.WriteLine("║");
            snake.Head.Value = new Point(window.MinPoint.X - 1, snake.Head.Value.Y);
        }
        if(snake.Head.Value.Y.Equals(window.MinPoint.Y))
        {
            Console.SetCursorPosition(snake.Head.Value.X, snake.Head.Value.Y);
            Console.WriteLine("═");
            snake.Head.Value = new Point(snake.Head.Value.X, window.MaxPoint.Y + 1);
        }
        if(snake.Head.Value.Y.Equals(window.MaxPoint.Y))
        {
            Console.SetCursorPosition(snake.Head.Value.X, snake.Head.Value.Y);
            Console.WriteLine("═");
            snake.Head.Value = new Point(snake.Head.Value.X, window.MinPoint.Y - 1);
        }
        if (snake.Head.Value.Equals(food.Position))
        { 
        
            snake.eating = true;
            Console.SetCursorPosition(snake.Head.Value.X, snake.Head.Value.Y);
            Console.WriteLine(" ");
            GetFoodPosition();
            food.FoodExist = false;
        }
        foreach(var itemn in snake.Body)
        {
            if (snake.Head.Value.Equals(itemn))
            {
                alive = false;
                window.TextResizeEnable();
                Task.Run(() => snake.DeathAnimation(), cancellationTokenSource.Token);
                Task.Run(() =>
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    window.ResizeAndPrintCenteredText(Menu.GameOver, cancellationTokenSource);
                }, cancellationTokenSource.Token);
                ConsoleKey key = GetValidkey(new ConsoleKey[] { ConsoleKey.Escape, ConsoleKey.Enter });
                if (key.Equals(ConsoleKey.Enter))
                {
                    MainMenu();
                    Start();
                }
                if (key.Equals(ConsoleKey.Escape))
                {
                    Environment.Exit(0);
                }
                break;
            }
        }
    }
}
