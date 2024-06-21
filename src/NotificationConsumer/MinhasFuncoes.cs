using Hangfire;

namespace NotificationConsumer
{
    public static class MinhasFuncoes
    {
        [Queue("default")]
        public static void Inicializacao()
        {
            Console.WriteLine("BEM-VINDO AO HANGFIRE!");
        }
    }
}
