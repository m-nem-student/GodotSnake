using Godot;
using System;
using System.Collections.Generic;

public partial class node_2d : Node2D
{
	private const int ScreenHeight = 640;
	private const int ScreenWidth = 640;
	private const double GameSpeed = 0.5;
	
	private int score;
	private bool endGame;
	private Pixel berry;
	private Pixel head;
	private Random randomNum;
	private Direction movement;
	private Direction lastMovement;
	private List<int> bodyXPos;
	private List<int> bodyYPos;
	private double time;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
			randomNum = new Random();
			score = 5;
			endGame = false;
			head = new Pixel
			{
				XPos = ScreenWidth / 2,
				YPos = ScreenHeight / 2,
				PixelColor = Colors.Red
			};
			berry = new Pixel
			{
				PixelColor = Colors.Cyan
			};
			movement = Direction.Right;
			bodyXPos = new List<int>();
			bodyYPos = new List<int>();
			GenerateBerry();
			time = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if(endGame) 
		{
			QueueRedraw();
			return;
		}
		time += delta;
		InputHandler();
		if(time < GameSpeed) return;
		
		lastMovement = movement;
		UpdateSnakePosition(ref head, ref bodyXPos, ref bodyYPos, movement);
		QueueRedraw();
		CheckGameOver();
		
		time = 0;
	}
	
	public override void _Draw()
	{
		var viewport = GetViewportRect();
		DrawRect(viewport, Colors.Black);
		DrawBorders(ScreenWidth, ScreenHeight);
		DrawPixelInConsole(head.XPos, head.YPos, head.PixelColor);
		DrawPixelInConsole(berry.XPos, berry.YPos, berry.PixelColor);
		GD.Print(bodyYPos.Count);
		for(int i = 0; i < bodyYPos.Count; i++ ){
			DrawPixelInConsole(bodyXPos[i], bodyYPos[i], Colors.Green);
		}
		
		if(endGame){
			DrawString(ThemeDB.FallbackFont, new Vector2(ScreenWidth/8, ScreenHeight/2), $"GAME OVER\n score: {score}",
			   HorizontalAlignment.Center, 500, 50);
		}
	}
	
	
		private void InputHandler()
		{
			if (Input.IsActionPressed("up") && lastMovement != Direction.Down) {
				movement = Direction.Up;
			}

			if (Input.IsActionPressed("down") && lastMovement != Direction.Up) {
				movement = Direction.Down;
			}

			if (Input.IsActionPressed("left") && lastMovement != Direction.Right) {
				movement = Direction.Left;
			}

			if (Input.IsActionPressed("right") && lastMovement != Direction.Left) {
				movement = Direction.Right;
			}
		}
	
	
		private void UpdateSnakePosition(ref Pixel head, ref List<int> bodyXPos, ref List<int> bodyYPos, Direction movement)
		{
			bodyXPos.Add(head.XPos);
			bodyYPos.Add(head.YPos);

			switch (movement)
			{
				case Direction.Up:
					head.YPos-= 10;
					break;
				case Direction.Down:
					head.YPos+= 10;
					break;
				case Direction.Left:
					head.XPos-= 10;
					break;
				case Direction.Right:
					head.XPos+= 10;
					break;
			}


			if (berry.XPos == head.XPos && berry.YPos == head.YPos)
			{
				score++;
				CheckGameWin();
				GenerateBerry();
			}
			if (bodyXPos.Count > score){
				bodyXPos.RemoveAt(0);
				bodyYPos.RemoveAt(0);
				}
		}
		
		private void GenerateBerry()
		{
			bool isBerryOnSnake = true;
			while (isBerryOnSnake)
			{
				berry.XPos = randomNum.Next(11, ScreenWidth - 11);
				berry.YPos = randomNum.Next(11, ScreenHeight - 11);
				if(berry.XPos % 10 != 0){
					int overflow = berry.XPos % 10;
					berry.XPos -= overflow;
				}
				if(berry.YPos % 10 != 0){
					int overflow = berry.YPos % 10;
					berry.YPos -= overflow;
				}

				isBerryOnSnake = false;
				if (berry.XPos == head.XPos && berry.YPos == head.YPos)
				{
					isBerryOnSnake = true;
					continue;
				}

				for (int i = 0; i < bodyXPos.Count; i++)
				{
					if (berry.XPos == bodyXPos[i] && berry.YPos == bodyYPos[i])
					{
						isBerryOnSnake = true;
						break;
					}
				}
			}
		}
		
		private void DrawBorders(int width, int height)
		{
		var leftBorder = new Rect2(0, 0, 10, width);
		DrawRect(leftBorder, Colors.White);
		var topBorder = new Rect2(0, 0, height, 10);
		DrawRect(topBorder, Colors.White);
		var rightBorder = new Rect2(height, 0, 10, width);
		DrawRect(rightBorder, Colors.White);
		var bottomBorder = new Rect2(0, width, height + 10, 10);
		DrawRect(bottomBorder, Colors.White);
		}


		private void CheckGameOver()
		{
			// Snake crash into wall
			if (head.XPos == ScreenWidth - 10 || head.XPos == 10 || head.YPos == ScreenHeight - 10 || head.YPos == 10)
			{
				endGame = true;
				return;
			}

			// Snake crash into himself
			for (var i = 0; i < bodyXPos.Count; i++)
			{
				if (bodyXPos[i] == head.XPos && bodyYPos[i] == head.YPos)
				{
					endGame = true;
					return;
				}
			}
		}

		private void CheckGameWin()
		{
			int maxBerriesGenerated = (ScreenWidth - 20) * (ScreenHeight - 20) - (score + 1);
			if (maxBerriesGenerated != 0) return;

			endGame = true;
		}

	private void DrawPixelInConsole(int xPos, int yPos, Color pixelColor)
	{
		var point = new Rect2(xPos, yPos,  10, 10);
		DrawRect(point, pixelColor);
	}
	
	private class Pixel
		{
			public int XPos { get; set; }
			public int YPos { get; set; }
			public Color PixelColor { get; init; }
		}
		
		
		private enum Direction
		{
			Left,
			Right,
			Up,
			Down
		}

}

