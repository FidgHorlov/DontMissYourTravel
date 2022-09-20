namespace DontMissTravel.Data
{
    public enum Direction
    {
        Idle = 0,
        Left,
        Right,
        Up,
        Down
    }
    
    public enum GateState
    {
        WillOpen,
        Opened,
        Closed
    }

    public enum Action
    {
        Move,
        Grab,
        Meet,
        Stay
    }

    public enum EnemyType
    {
        Policeman,
        Nurse
    }

    public enum EnemyName
    {
        Policeman1,
        Policeman2,
        Nurse1,
        Nurse2
    }

    public enum EnvironmentType
    {
        Border,
        ChairLeftSide,
        ChairRightSide,
        ChairFrontSide,
        Gate
    }

    public enum GameState
    {
        Play,
        HideGame,
        Pause
    }

    public enum TypeOfLevel
    {
        Train,
        Bus,
        Plane
    }

    public enum WindowName
    {
        Lose,
        Win,
        Pause
    }

    public enum BonusType
    {
        TimeDelay,
        SpeedUpgrade,
        Invisible
    }
}