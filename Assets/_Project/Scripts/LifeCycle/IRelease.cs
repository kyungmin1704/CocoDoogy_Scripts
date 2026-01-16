namespace CocoDoogy.LifeCycle
{
    public interface IRelease<T>
    {
        public void OnRelease(T data);
    }
}