using System;
using System.Threading;

class Arkanoid
{
    static int score = 0;
    // Screen dimensions
    static int screenWidth = 20;
    static int screenHeight = 20;

    // Paddle properties
    static int paddleWidth = 5;
    static int paddleX;
    static int paddleY = screenHeight - 2;

    // Ball properties
    static int ballX;
    static int ballY;
    static int ballDirectionX = 1;
    static int ballDirectionY = -1;

    // Block properties
    static int blockRows = 3;
    static int blockColumns = 3;
    static bool[,] blocks = new bool[blockRows, blockColumns];

    // Game screen array
    static char[,] screen = new char[screenHeight, screenWidth];

    static int menuSelect = 0;
    // Game status
    static bool gameRunning = false;

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Thread.Sleep(200);
        Console.Clear();
        WriteInMiddle(-3, "ARKANOID");
        Thread.Sleep(2000);
        while (true)
        {
            if (!gameRunning)
            {
                StartScreen();
                score = 0;
            }
            if (menuSelect == 0)
            {
                Console.Clear();
                gameRunning = true;
                paddleX = (screenWidth - paddleWidth) / 2;
                ballX = screenWidth / 2;
                ballY = paddleY - 1;
                ballDirectionX = 1;
                ballDirectionY = -1;

                for (int row = 0; row < blockRows; row++)
                {
                    for (int col = 0; col < blockColumns; col++)
                    {
                        blocks[row, col] = true;
                    }
                }

                // Game loop
                while (gameRunning)
                {
                    DrawScreen();
                    while (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.LeftArrow && paddleX > 0)
                        {
                            paddleX--;
                        }
                        else if (key == ConsoleKey.RightArrow && paddleX + paddleWidth < screenWidth)
                        {
                            paddleX++;
                        }
                    }
                    Update();
                    if (CheckBlocks())
                    {
                        Console.Clear();
                        WriteInMiddle(-2, "GOOD JOB!!!");

                        WriteInMiddle(-1, "Your score: ");
                        Console.Write(score);
                        Thread.Sleep(2000); // FPS
                        break;


                    }
                    Thread.Sleep(100); // FPS
                }

                if (!gameRunning)
                {
                    Console.Clear();
                    WriteInMiddle(-2, "GAME OVER!!!");
                    WriteInMiddle(-1, "Your score: ");
                    Console.Write(score);
                    menuSelect = 0;
                    Thread.Sleep(2000);
                }
            }
            else if (menuSelect == 1)
            {
                SettingsScreen();
            }
            else if (menuSelect == 2)
            {
                return;
            }
            menuSelect = 0;
        }
    }

    static void DrawScreen()
    {
        for (int y = 0; y < screenHeight; y++)
        {
            for (int x = 0; x < screenWidth; x++)
            {
                screen[y, x] = ' ';
            }
        }

        for (int x = 0; x < screenWidth; x++)
        {
            screen[0, x] = '=';
            screen[screenHeight - 1, x] = '=';
        }

        for (int row = 0; row < blockRows; row++)
        {
            for (int col = 0; col < blockColumns; col++)
            {
                if (blocks[row, col])
                {
                    int blockStartX = col * 5 + 2;
                    if (blockStartX + 4 < screenWidth)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            screen[row + 1, blockStartX + i] = '#';
                        }
                    }
                }
            }
        }

        // Draw paddle
        for (int i = 0; i < paddleWidth; i++)
        {
            screen[paddleY, paddleX + i] = '_';
        }

        // Draw ball
        screen[ballY, ballX] = 'O';

        Console.SetCursorPosition(0,0);
        for (int y = 0; y < screenHeight; y++)
        {
            for (int x = 0; x < screenWidth; x++)
            {
                Console.Write(screen[y, x]);
            }
            Console.WriteLine();
        }
        Console.Write("Score:");
        Console.Write(score);
    }

    static void Input()
    {
        // Check for player input to move the paddle
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.LeftArrow && paddleX > 0)
            {
                paddleX--;
            }
            else if (key == ConsoleKey.RightArrow && paddleX + paddleWidth < screenWidth)
            {
                paddleX++;
            }
        }
    }

    static void Update()
    {
        // Move the ball
        ballX += ballDirectionX;
        ballY += ballDirectionY;

        // Ball collision with walls
        if (ballX <= 0 || ballX >= screenWidth - 1)
        {
            ballDirectionX *= -1;
        }
        if (ballY <= 1)
        {
            ballDirectionY *= -1;
            ballY = 1;
        }

        // Collision with paddle
        if (ballY == paddleY && ballX >= paddleX && ballX < paddleX + paddleWidth)
        {
            ballDirectionY *= -1;
        }

        for (int row = 0; row < blockRows; row++)
        {
            for (int col = 0; col < blockColumns; col++)
            {
                if (blocks[row, col] && ballY == row + 1 && ballX >= col * 5 + 2 && ballX < col * 5 + 7)
                {
                    blocks[row, col] = false; // Destroy block
                    score += 200;
                    ballDirectionY *= -1;
                }
            }
        }

        // Game over
        if (ballY >= screenHeight - 1)
        {
            gameRunning = false;
        }
    }

    static bool CheckBlocks()
    {
        foreach (bool block in blocks)
        {
            if (block)
                return false;
        }
        return true;
    }

    static void WriteInMiddle(int diffY, string content, int diffX=0)
    {
        Console.SetCursorPosition(screenWidth/2+diffX-4, screenHeight/2+diffY);
        Console.Write(content);
    }
    static void StartScreen()
    {
        Console.Clear();
        WriteInMiddle(-5, "ARKANOID");
        WriteInMiddle(-2, ">> START <<", -3);
        WriteInMiddle(-1, "Settings");
        WriteInMiddle(0, "Exit");
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.DownArrow)
                {
                    menuSelect += 1;
                    if (menuSelect > 2)
                    {
                        menuSelect = 0;
                    }
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    menuSelect -= 1;
                    if (menuSelect < 0)
                    {
                        menuSelect = 2;
                    }
                }
                else if (key == ConsoleKey.Enter)
                    break;
                if (menuSelect == 0)
                    WriteInMiddle(-2, ">> START <<", -3);
                else
                    WriteInMiddle(-2, "   START   ", -3);
                if (menuSelect == 1)
                    WriteInMiddle(-1, ">> Settings <<", -3);
                else
                    WriteInMiddle(-1, "   Settings   ", -3);
                if (menuSelect == 2)
                    WriteInMiddle(0, ">> Exit <<", -3);
                else
                    WriteInMiddle(0, "   Exit   ", -3);

            }
        }



    }

    static void SettingsScreen()
    {
        Console.Clear();
        WriteInMiddle(-5, "DIFFICULTY");
        if (paddleWidth == 7)
            WriteInMiddle(-2, ">> Easy <<", -3);
        else
            WriteInMiddle(-2, "   Easy   ", -3);
        if (paddleWidth == 5)
            WriteInMiddle(-1, ">> Medium <<", -3);
        else
            WriteInMiddle(-1, "   Medium   ", -3);
        if (paddleWidth == 3)
            WriteInMiddle(0, ">> Hard <<", -3);
        else
            WriteInMiddle(0, "   Hard   ", -3);
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                {
                    paddleWidth += 2;
                    if (paddleWidth > 7)
                    {
                        paddleWidth = 3;
                    }
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    paddleWidth -= 2;
                    if (paddleWidth < 3)
                    {
                        paddleWidth = 7;
                    }
                }
                else if (key == ConsoleKey.Enter)
                    break;
                if (paddleWidth == 7)
                    WriteInMiddle(-2, ">> Easy <<", -3);
                else
                    WriteInMiddle(-2, "   Easy   ", -3);
                if (paddleWidth == 5)
                    WriteInMiddle(-1, ">> Medium <<", -3);
                else
                    WriteInMiddle(-1, "   Medium   ", -3);
                if (paddleWidth == 3)
                    WriteInMiddle(0, ">> Hard <<", -3);
                else
                    WriteInMiddle(0, "   Hard   ", -3);

            }
        }
    }
}
