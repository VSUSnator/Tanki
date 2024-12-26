using static Tanki.Tank;
using Tanki;

public interface ITank : IMovable
{
    Bullet Shoot();
    void ChangeDirection(Direction newDirection);
    void Draw(Renderer renderer);
}