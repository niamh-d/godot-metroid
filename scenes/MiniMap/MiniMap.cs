using Godot;

public enum BorderScanE
{
    Empty = 1,
    Partial = 2,
    Solid = 3,
}

public class RoomScanResult
{
    public BorderScanE Top;
    public BorderScanE Bottom;
    public BorderScanE Left;
    public BorderScanE Right;
}

public class MiniMap : CanvasLayer
{

    private Sprite Location;
    private LevelArea LevelArea = null;
    private int? TileCollisionLayer = null;
    private Vector2 Resolution = Vector2.Zero;
    public Vector2 MiniMapOffset = new Vector2(77, 39);

    [Export]
    private int BorderEdgePartialThreshold = 10;
    [Export]
    public Vector2 MiniMapTileSize = new Vector2(19, 19);
    [Export]
    public Vector2 NumTilesXY = new Vector2(40, 23);
    [Export]
    public Rect2 ScanRoomAreaSize = new Rect2(-50, -50, 50, 50);

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
                    ScanMiniMapRooms(roomPosition, tileMap);
                }
            }
        }
    }

    private void ScanMiniMapRooms(Vector2 roomPosition, TileMap tileMap)
    {
        Vector2 roomLevelPos = Vector2.Zero;


        for (int y = (int)ScanRoomAreaSize.Position.y; y < (int)ScanRoomAreaSize.End.y; ++y)
        {
            for (int x = (int)ScanRoomAreaSize.Position.x; x < (int)ScanRoomAreaSize.End.x; ++x)
            {
                roomLevelPos.x = x * NumTilesXY.x;
                roomLevelPos.y = y * NumTilesXY.y;
                RoomScanResult borders = ScanRoomBorders(roomLevelPos, tileMap);

            }
        }
    }

    private RoomScanResult ScanRoomBorders(Vector2 position, TileMap tileMap)
    {
        RoomScanResult scan = new RoomScanResult
        {
            Top = ScanRoomBorderTop(position, tileMap),
            Bottom = ScanRoomBorderBottom(position, tileMap),
            Left = ScanRoomBorderLeft(position, tileMap),
            Right = ScanRoomBorderRight(position, tileMap)
        };
        return scan;
    }

    private BorderScanE ScanRoomBorderTop(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int x = (int)position.x; x < (int)position.x + NumTilesXY.x; ++x)
        {
            if (tileMap.GetCell(x, (int)position.y) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell(x, (int)position.y) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

    private BorderScanE ScanRoomBorderBottom(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int x = (int)position.x; x < (int)position.x + NumTilesXY.x; ++x)
        {
            if (tileMap.GetCell(x, (int)(position.y + NumTilesXY.y) - 1) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell(x, (int)(position.y + NumTilesXY.y) - 1) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

    private BorderScanE ScanRoomBorderLeft(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int y = (int)position.y; y < (int)position.y + NumTilesXY.y; ++y)
        {
            if (tileMap.GetCell((int)position.x, y) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell((int)position.x, y) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

    private BorderScanE ScanRoomBorderRight(Vector2 position, TileMap tileMap)
    {
        BorderScanE result = BorderScanE.Solid;
        int numTiles = 0;

        for (int y = (int)position.y; y < (int)position.y + NumTilesXY.y; ++y)
        {
            if (tileMap.GetCell((int)(position.x + NumTilesXY.x) - 1, y) < 0) { result = BorderScanE.Partial; }

            else if (tileMap.GetCell((int)(position.x + NumTilesXY.x) - 1, y) >= 0) { numTiles++; }
        }

        if (numTiles >= BorderEdgePartialThreshold) { return result; }

        return BorderScanE.Empty;
    }

}
