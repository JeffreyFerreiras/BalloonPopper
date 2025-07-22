using System.ComponentModel;

namespace BalloonPopper.Models
{
    /// <summary>
    /// Represents a balloon entity in the game with its properties and behaviors
    /// </summary>
    public class Balloon : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _velocityY;
        private bool _isPopped;
        private double _scale = 1.0;

        public int Id { get; set; }
        public BalloonType Type { get; set; }
        public BalloonColor Color { get; set; }
        public int Points { get; set; }
        public double Size { get; set; }
        public DateTime SpawnTime { get; set; }
        public double LifeSpan { get; set; } // in seconds

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }

        public double VelocityY
        {
            get => _velocityY;
            set
            {
                _velocityY = value;
                OnPropertyChanged();
            }
        }

        public bool IsPopped
        {
            get => _isPopped;
            set
            {
                _isPopped = value;
                OnPropertyChanged();
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpired => DateTime.Now.Subtract(SpawnTime).TotalSeconds > LifeSpan;
        public bool ShouldBeRemoved => IsPopped || IsExpired || Y < -Size;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(
            [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null
        )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates balloon position based on velocity and time delta
        /// </summary>
        public void Update(double deltaTime)
        {
            Y -= VelocityY * deltaTime;
        }

        /// <summary>
        /// Pops the balloon and returns the score earned
        /// </summary>
        public int Pop()
        {
            if (IsPopped)
                return 0;

            IsPopped = true;
            return Points;
        }
    }

    public enum BalloonType
    {
        Normal,
        Speed, // Faster moving balloon
        Giant, // Larger balloon, more points
        Bonus, // Extra points
        Multiplier, // Score multiplier
        Bomb, // Destroys nearby balloons
        Shield, // Temporary invincibility
        TimeFreeze, // Slows down time temporarily
        DoublePoints, // Doubles points for a duration
    }

    public enum BalloonColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Orange,
        Pink,
        White,
        Black,
        Rainbow, // Special multicolor balloon
    }
}
