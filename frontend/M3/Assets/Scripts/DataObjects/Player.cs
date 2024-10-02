public class Player
{
    private bool _myturn;
    private string _name;
    private int _points;
    private int _cubes;

    public Player(string name)
    {
        _myturn = false;
        _points = 0;
        _name = name;
        _cubes = 5;
    }

    public string Name()
    {
        return _name;
    }
    public int Points()
    {
        return _points;
    }

    public void AddPoints(int points)
    {
        _points += points;
    }

    public void SetCubes(int cubes)
    {
        _cubes = cubes;
    }

    public int GetCubes()
    {
        return _cubes;
    }

    public bool IsMyTurn()
    {
        return _myturn;
    }

    public void MyTurn()
    {
        _myturn = true;
    }

    public void TurnOver()
    {
        _myturn = false;
    }


}
