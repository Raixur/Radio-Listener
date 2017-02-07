using System;
using System.IO;
using Microsoft.Extensions.Configuration;

using RadioListener.Services;

namespace RadioListener
{
    public class Program
    {
        public static void Main (string[] args)
        {
            try
            {
                var factory = CreateFactory();
                var listener = factory.Listener;

                listener.Listen();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static ServiceFactory CreateFactory()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json");

            var config = builder.Build()
                                .GetSection("RadioListenerConfig")
                                .Get<RadioListenerConfig>();

            return new ServiceFactory(config);
        }
    }
}
