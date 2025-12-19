public abstract class Entity
{
    public int id;
    private int owner;
    public int GetOwner() { return owner; }
    public void SetOwner(int ownerId) { owner = ownerId; }
}