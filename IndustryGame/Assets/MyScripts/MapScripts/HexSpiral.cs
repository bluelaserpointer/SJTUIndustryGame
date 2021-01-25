public class HexSpiral
{
    HexCoordinates hexCoordinates;
    int radius;
    int edge;
    int edgeLocation;
    public void setCoordinates(HexCoordinates hexCoordinates)
    {
        this.hexCoordinates = hexCoordinates;
        edgeLocation = radius = 0;
        edge = 6;
    }
    public HexCoordinates next()
    {
        HexDirection moveDirection;
        if (edgeLocation < radius)
        {
            moveDirection = (HexDirection)edge;
            ++edgeLocation;
        } else
         {
            edgeLocation = 0;
            if(edge < 6)
            {
                moveDirection = (HexDirection)edge;
                ++edge;
            } else
            {
                moveDirection = HexDirection.W;
                edge = 0;
                ++radius;
            }
        }
        return hexCoordinates = move(hexCoordinates, moveDirection, 1);
    }
    public HexCoordinates GetCoordinates()
    {
        return hexCoordinates;
    }
    public static HexCoordinates move(HexCoordinates hexCoordinates, HexDirection hexDirection, int distance)
    {
        int x = hexCoordinates.X, z = hexCoordinates.Z;
        switch(hexDirection)
        {
            case HexDirection.NE:
                z += distance;
                break;
            case HexDirection.E:
                x += distance;
                break;
            case HexDirection.SE:
                x += distance;
                z -= distance;
                break;
            case HexDirection.SW:
                z -= distance;
                break;
            case HexDirection.W:
                x -= distance;
                break;
            case HexDirection.NW:
                x -= distance;
                z += distance;
                break;
        }
        return new HexCoordinates(x, z);
    }
}
