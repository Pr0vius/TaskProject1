namespace ms.rabbitmq.Consumer
{
    public interface IConsumer
    {
        Task Subscribe();
        void Unsubscribe();
    }
}
