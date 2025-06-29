using Godot;

public class MiniMap : CanvasLayer
{

    private Sprite Location;
    private LevelArea LevelArea = null;
    private int? TileCollisionLayer = null;
    private Vector2 Resolution = Vector2.Zero;

    public override void _Ready()
    {
        GetResolution();
        TileCollisionLayer = GetWorldTileCollisionLayer();
        Location = GetNode<Sprite>("%Location");
        GetAreaNode();
        ScanRooms();
    }

    private void GetResolution()
    {
        Resolution = OS.WindowSize;
    }

    private void GetAreaNode()
    {
        var parent = GetParent();

        foreach (Node child in parent.GetChildren())
        {
            if (child.GetType().ToString().Equals("LevelArea"))
            {
                LevelArea = (LevelArea)child;
            }
        }
    }

    private void OnLocationBlinkTimerTimeout()
    {
        Location.Visible = !Location.Visible;
    }

    private int? GetWorldTileCollisionLayer()
    {
        for (int i = 1; i <= 32; ++i)
        {
            var layer = ProjectSettings.GetSetting("layer_names/2d_physics/layer_" + i).ToString();

            if (layer.Equals("Tiles"))
            {
                GD.Print("Tiles collision layer found on layer " + i);
                return i;
            }
        }
        GD.PrintErr("Could not find 'Tiles' collision layer!");
        GD.PrintErr("Make sure to set name 'Tiles' under 'project settinngs > layer names for collision layer.");
        return null;
    }

    private void ScanRooms()
    {
        if (LevelArea == null)
        {
            GD.PrintErr("Cannot scan rooms. Level node is null!");
            return;
        }

        foreach (Node2D node in LevelArea.GetChildren())
        {
            if (node.GetType().ToString().Equals("LevelRoom"))
            {
                LevelRoom room = (LevelRoom)node;
                ScanRoom(room, room.Name, room.Position);
            }
        }
    }

    private void ScanRoom(Node2D room, string roomName, Vector2 roomPosition)
    {
        foreach (Node2D node in room.GetChildren())
        {
            if (node.GetClass().Equals("TileMap"))
            {
                TileMap tileMap = (TileMap)node;

                if (tileMap.CollisionLayer == TileCollisionLayer)
                {
                    GD.Print("Tilemap with tiles collision layer found: " + tileMap.Name);
                }
            }
        }
    }
}
