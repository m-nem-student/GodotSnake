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
			time = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += delta;
		InputHandler();
		if(time < GameSpeed) return;
		
		lastMovement = movement;
		UpdateSnakePosition(ref head, ref bodyXPos, ref bodyYPos, movement);
		QueueRedraw();	
		
		
		time = 0;
	}
	
	public override void _Draw()
	{
		var viewport = GetViewportRect();
		
		DrawRect(viewport, Colors.Black);
		
		DrawBorders(ScreenWidth, ScreenHeight);
				
		DrawPixelInConsole(head.XPos, head.YPos, head.PixelColor);
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

/*
			if (berry.XPos == head.XPos && berry.YPos == head.YPos)
			{
				score++;
				CheckGameWin();
				GenerateBerry();
			}
			*/

/*
			if (bodyXPos.Count <= score)
				return;

			Console.SetCursorPosition(bodyXPos[0], bodyYPos[0]);
			Console.Write(" ");
			bodyXPos.RemoveAt(0);
			bodyYPos.RemoveAt(0);
			*/
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

