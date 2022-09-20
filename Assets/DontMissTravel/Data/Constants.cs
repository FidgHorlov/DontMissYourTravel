namespace DontMissTravel.Data
{
    public static class Constants
    {
        public const string FloatTextFormat = "##";
        public const float FadeAnimationTime = 0.5f;
        public const float ObjectAliveTime = 5f;
        public static class EnemiesParameters
        {
            public const int MaximumEnemies = 10;
        }
        
        public static class Tags
        {
            public const string Border = "Border";
            public const string Block = "Block";
            public const string Obstacle = "Obstacle";
            public const string Enemy = "Enemy";
            public const string Player = "Player";
            public const string Gate = "Gate";
        }

        public static class SpeedParameters
        {
            public const float SpeedIncrease = 1.5f;
        }
        
        public static class InformationTexts
        { 
            public const string DelayCaption = "Delay:";
            public const string SpeedBoosted = "Your speed increased";
            public const string GreenTicket = "You got green ticket";
        }
    }
}