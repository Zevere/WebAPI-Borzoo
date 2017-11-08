﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Borzoo
{
    public class Program
    {
        public static void Main(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
}
