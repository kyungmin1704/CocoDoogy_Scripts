namespace CocoDoogy.LifeCycle
{
    public interface IInit<in T>
    {
        public void OnInit(T data);
    }
}