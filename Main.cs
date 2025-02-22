using Godot;
using System;

public partial class Main : Node
{
	// Don't forget to rebuild the project so the editor knows about the new export variable.

	[Export] public PackedScene MobScene { get; set; }

	private int _score;

	public void GameOver()
	{
		GD.Print("GameOver");
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<Hud>("HUD").ShowGameOver();
	}

	public void NewGame()
	{
		_score = 0;
		// Note that for calling Godot-provided methods with strings,
		// we have to use the original Godot snake_case name.
		// 调用CallGroup前检查组内节点数量
		var mobs = GetTree().GetNodesInGroup("mobs");
		GD.Print("当前mobs组节点数：", mobs.Count);
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		var hud = GetNode<Hud>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");
		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
	}

	// We also specified this function name in PascalCase in the editor's connection window.
	private void OnScoreTimerTimeout()
	{
		_score++;
		GetNode<Hud>("HUD").UpdateScore(_score);
	}

// We also specified this function name in PascalCase in the editor's connection window.
	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	// We also specified this function name in PascalCase in the editor's connection window.
	private void OnMobTimerTimeout()
	{
		// Create a new instance of the Mob scene.
		Mob mob = MobScene.Instantiate<Mob>();
		GD.Print("Mob instantiated:", mob != null); // 验证实例化结果
		GD.Print("Mob position:", mob.Position); // 检查生成坐标
		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		// Choose the velocity.
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}

	public override void _Ready()
	{
		// NewGame();
		GD.Print("New game started");
	}
}
