using Godot;

public class ParallaxBackground : Godot.ParallaxBackground
{

    private float scrollSpeed = 100;

    public override void _Process(float delta)
    {
        ScrollOffset += new Vector2(scrollSpeed * delta, 0);
    }
}
