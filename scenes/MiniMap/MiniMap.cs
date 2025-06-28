using Godot;

public class MiniMap : CanvasLayer
{

    private Sprite Location;

    public override void _Ready()
    {
        Location = GetNode<Sprite>("%Location");
    }

    private void OnLocationBlinkTimerTimeout()
    {
        Location.Visible = !Location.Visible;
    }

}
