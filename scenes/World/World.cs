using Godot;

public class World : Node2D
{
    private Node2D Hero;
    private Camera2D Camera;
    private MiniMap MiniMap;

    public override void _Ready()
    {
        Hero = GetNode<Node2D>("Hero");
        Camera = Hero.GetNode<Camera2D>("Camera2D");
        MiniMap = GetNode<MiniMap>("MiniMap");
        GetCameraLimits();
    }

    private void GetCameraLimits()
    {
        Rect2 AreaCameraLimits = MiniMap.GetCameraAreaLimits();
        Camera.LimitTop = (int)AreaCameraLimits.Position.y;
        Camera.LimitBottom = (int)AreaCameraLimits.Size.y;
        Camera.LimitLeft = (int)AreaCameraLimits.Position.x;
        Camera.LimitRight = (int)AreaCameraLimits.Size.x;
    }

    public override void _Process(float delta)
    {
        MiniMap.UpdateMinMapPosition(Hero.Position);
    }
}
