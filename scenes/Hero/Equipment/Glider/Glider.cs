using Godot;

public class Glider : Node2D
{
    private AnimatedSprite GliderAnimation;
    public string LastPlayedAnimation = string.Empty;
    public bool FlipHorizontal = false;

    public override void _Ready()
    {
        InitGlider();
    }

    private void InitGlider()
    {
        GliderAnimation = GetNode<AnimatedSprite>("./AnimatedGlider");

        if (GliderAnimation == null)
        {
            GD.PrintErr("Glider.cs – InitGlider() – Could not find the 'AnimatedGlider' node!");
            return;
        }
        GliderAnimation.Connect("animation_finished", this, nameof(AnimationDone));
    }

    private void AnimationDone()
    {
        LastPlayedAnimation = GliderAnimation.Animation.ToString();
    }

    public override void _Process(float delta)
    {
        GliderAnimation.FlipH = FlipHorizontal;

        if (LastPlayedAnimation.Equals("GliderClose"))
        {
            Visible = false;
            LastPlayedAnimation = string.Empty;
        }

        if (LastPlayedAnimation.Equals("GliderOpen"))
        {
            GliderAnimation.Play("Glide");
            LastPlayedAnimation = string.Empty;
        }
    }

    public void OpenGlider()
    {
        Visible = true;
        GliderAnimation.Play("GliderOpen");
    }

    public void CloseGlider()
    {
        GliderAnimation.Play("GliderClose");
    }
}
