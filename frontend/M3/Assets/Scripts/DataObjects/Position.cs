public class Position
{
    public PositionValues positionValues;

    public struct PositionValues
    {
        public int row;
        public int column;

    };

    public void setPosition(int row, int column)
    {
        positionValues.column = column;
        positionValues.row = row;
    }

    public int getPositionRow()
    {
        return positionValues.row;
    }
    
    public int getPositionColumn()
    {
        return positionValues.column;
    }

}
