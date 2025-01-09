using static Tanki.Tank;
using Tanki;

public interface ITank : IMovable
{
    // Добавляем свойства X и Y
    int X { get; } // Координата X
    int Y { get; } // Координата Y

    Bullet? Shoot();
    void ChangeDirection(Direction newDirection);
    void Draw(Renderer renderer);
}