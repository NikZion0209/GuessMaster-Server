namespace GuessMaster.Service.Interface
{ 
    public interface IEventHandler
    {
        void Subscribe();
        void Unsubscribe();
    }
}