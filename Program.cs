using System;
using System.Collections.Generic;

class Game
{
    // Game map and player's initial position
    char[,] map = new char[5, 5];
    int playerX = 0, playerY = 0;

    // Special item and puzzle man positions
    int specialItemX, specialItemY;
    int puzzleManX, puzzleManY;

    // Player name and inventory
    string playerName;
    List<string> inventory = new List<string>();

    // Flags to track special item and puzzle man encounters
    public bool specialItemFound = false;
    public bool puzzleManFound = false;
    public bool isSpecialItemTaken = false;
    bool puzzleSolved = false;

    // Player's life count
    public int lifeCount = 2;

    // Constructor for game initialization
    public Game(string name)
    {
        playerName = name;
        InitializeMap();
        PlaceSpecialItem();
        PlacePuzzleMan();
        PlacePlayer();
        DisplayMap();
    }

    // Initialize map with empty spaces
    void InitializeMap()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                map[x, y] = '.';
            }
        }
    }

    // Place special item at a random location
    void PlaceSpecialItem()
    {
        Random random = new Random();
        specialItemX = random.Next(5);
        specialItemY = random.Next(5);
    }

    // Place puzzle man at a random location, ensuring it's different from the special item
    void PlacePuzzleMan()
    {
        Random random = new Random();
        puzzleManX = random.Next(5);
        puzzleManY = random.Next(5);

        // Make sure puzzle man is not placed on the special item or player's position
        if ((puzzleManX == specialItemX && puzzleManY == specialItemY) || (puzzleManX == playerX && puzzleManY == playerY))
        {
            PlacePuzzleMan();
        }
    }

    // Place player at the starting position
    void PlacePlayer()
    {
        map[playerX, playerY] = 'P';
    }

    // Display the current state of the map
    public void DisplayMap()
    {
        Console.Clear();
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (x == specialItemX && y == specialItemY && !isSpecialItemTaken)
                {
                    Console.Write('S' + " ");
                }
                else if (x == puzzleManX && y == puzzleManY)
                {
                    Console.Write('M' + " ");
                }
                else
                {
                    Console.Write(map[x, y] + " ");
                }
            }
            Console.WriteLine();
        }
        DisplayInventory();
        if (specialItemFound)
        {
            Console.WriteLine("\nSpecial item found! Type 'take' to pick it up.");
        }
    }

    // Display player's current life count
    public void DisplayLife()
    {
        Console.WriteLine("\nLife Count: " + lifeCount);
    }

    // Display player's inventory
    public void DisplayInventory()
    {
        Console.WriteLine("\nInventory:");
        if (inventory.Count == 0)
        {
            Console.WriteLine("Empty");
        }
        else
        {
            foreach (var item in inventory)
            {
                Console.WriteLine("- " + item);
            }
        }
    }

    // Handle player movement based on input key
    public void MovePlayer(ConsoleKey key)
    {
        int newX = playerX;
        int newY = playerY;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                newY = Math.Max(playerY - 1, 0);
                break;
            case ConsoleKey.DownArrow:
                newY = Math.Min(playerY + 1, 4);
                break;
            case ConsoleKey.LeftArrow:
                newX = Math.Max(playerX - 1, 0);
                break;
            case ConsoleKey.RightArrow:
                newX = Math.Min(playerX + 1, 4);
                break;
        }

        if (newX != playerX || newY != playerY)
        {
            map[playerX, playerY] = '.';
            playerX = newX;
            playerY = newY;
            map[playerX, playerY] = 'P';
        }

        CheckSpecialItem();
        CheckPuzzleMan();
    }

    // Check if the player has found the special item
    void CheckSpecialItem()
    {
        if (playerX == specialItemX && playerY == specialItemY && !isSpecialItemTaken)
        {
            specialItemFound = true;
        }
        else
        {
            specialItemFound = false;
        }
    }

    // Allow the player to take the special item
    public void TakeItem()
    {
        if (specialItemFound && !isSpecialItemTaken)
        {
            inventory.Add("Special Item");
            isSpecialItemTaken = true;
            specialItemFound = false;
            map[specialItemX, specialItemY] = '.';
            Console.WriteLine("\nYou have taken the special item!");
        }
    }

    // Check if the player has encountered the puzzle man
    public void CheckPuzzleMan()
    {
        if (playerX == puzzleManX && playerY == puzzleManY)
        {
            puzzleManFound = true;
        }
        else
        {
            puzzleManFound = false;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Game initialization with player name input
        Console.Write("Please enter your name: ");
        string name = Console.ReadLine();
        Game game = new Game(name);

        // Main game loop
        bool isPlaying = true;
        while (isPlaying)
        {
            // Player movement input
            ConsoleKey key = Console.ReadKey(true).Key;
            game.MovePlayer(key);
            game.DisplayMap();

            // Interaction with the special item
            if (game.specialItemFound)
            {
                Console.WriteLine("Do you want to take the item? [yes/no]");
                string command = Console.ReadLine().ToLower();
                if (command == "yes" || command == "take")
                {
                    game.TakeItem();
                }
            }

            // Interaction with the puzzle man
            if (game.puzzleManFound)
            {
                Console.WriteLine("\nYou've encountered the Puzzle Man!");
                Console.WriteLine("Puzzle: What is the key to winning this game?");
                Console.WriteLine("1) Luck\n2) Strategy");
                if (game.isSpecialItemTaken)
                {
                    Console.WriteLine("3) The Special Item");
                }

                Console.Write("Your answer (1, 2");
                if (game.isSpecialItemTaken)
                {
                    Console.Write(", 3");
                }
                Console.WriteLine("): ");
                string answer = Console.ReadLine();

                if (game.isSpecialItemTaken && answer == "3")
                {
                    Console.WriteLine("Correct! You've won the game!");
                    isPlaying = false; // End game on correct answer
                }
                else
                {
                    game.lifeCount -= 1;
                    Console.WriteLine("That's not right. Keep exploring.");
                }
            }

            // Display current life count
            game.DisplayLife();

            // Exit game if Escape key is pressed or life count reaches zero
            if (key == ConsoleKey.Escape || game.lifeCount <= 0)
            {
                isPlaying = false;
                if (game.lifeCount <= 0)
                {
                    Console.WriteLine("Game Over. Puzzle Man killed you");
                }
            }
        }
    }
}
