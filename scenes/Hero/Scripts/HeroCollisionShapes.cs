using Godot;

public class HeroCollisionShapes
{
    private HeroStateMachine Hero;
    private CollisionShape2D Head;
    private CollisionShape2D Body;
    private CollisionShape2D Slide;
    private bool ShapesInitialized;

    public HeroCollisionShapes(HeroStateMachine hero, ref bool initOk)
    {
        Hero = hero;
        initOk = InitHeroCollisionShapes();
    }

    private bool InitHeroCollisionShapes()
    {
        ShapesInitialized = true;

        Head = InitHeroCollisionShape("CollisionShapeHead");
        if (!ShapesInitialized) return false;
        Body = InitHeroCollisionShape("CollisionShapeBody");
        if (!ShapesInitialized) return false;
        Slide = InitHeroCollisionShape("CollisionShapeSlide");
        if (!ShapesInitialized) return false;

        return true;
    }

    private CollisionShape2D InitHeroCollisionShape(string shapeNodeName)
    {
        string collision2DNodeName = "./" + shapeNodeName;
        var collisionShape = Hero.GetNode<CollisionShape2D>(collision2DNodeName);
        if (collisionShape == null)
        {
            ShapesInitialized = false;
            GD.PrintErr("HeroCollisionShapes.cs – InitHeroCollisionShape() – Could not initialize collision shape, node:" + shapeNodeName + " was not found!");
        }
        return collisionShape;
    }

    public bool IsCollisionShape2DColliding(string collisionShapeName)
    {
        for (int i = 0; i < Hero.GetSlideCount(); i++)
        {
            var collision = Hero.GetSlideCollision(i);
            if (collision.LocalShape is CollisionShape2D)
            {
                var shape = (CollisionShape2D)collision.LocalShape;
                if (shape.Name.Equals(collisionShapeName))
                    return true;
            }
        }
        return false;
    }

    public void ChangeCollisionShapeToSlide()
    {
        Head.Disabled = true;
        Body.Disabled = true;
        Slide.Disabled = false;
    }

    public void ChangeCollisionShapeToStanding()
    {
        Head.Disabled = true;
        Body.Disabled = false;
        Slide.Disabled = true;
    }

    public void ChangeCollisionShapesToSlideStandUp()
    {
        Head.Disabled = false;
        Body.Disabled = true;
        Slide.Disabled = false;
    }

    public void DisableAllCollisionShapes()
    {
        Head.Disabled = true;
        Body.Disabled = true;
        Slide.Disabled = true;
    }
}