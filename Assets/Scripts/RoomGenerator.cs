internal class RoomGenerator
{
    private int maxIterations;
    private int roomWidthMin;
    private int roomLengthMin;

    public RoomGenerator(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        this.maxIterations = maxIterations;
        this.roomWidthMin = roomWidthMin;
        this.roomLengthMin = roomLengthMin;
    }
}