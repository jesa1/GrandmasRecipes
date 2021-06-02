using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Amazon.Lambda;

namespace RecipesAPI.Tokens
{
    /// <summary>
    /// The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
    /// </summary>
    public class LocalEntryPoint
    {
        // public static void Main(string[] args)
        // {
        //     BuildWebHost(args).Run();
        // }


        private static readonly LambdaEntryPoint LambdaEntryPoint = new LambdaEntryPoint();
        private static readonly Func<APIGatewayProxyRequest, ILambdaContext, Task<APIGatewayProxyResponse>> Func = LambdaEntryPoint.FunctionHandlerAsync;

        public static async Task Main(string[] args)
        {
            try{
            #if DEBUG
                BuildWebHost(args).Run();
            #else
                //wrap the  FunctionHandler in a form that  LambdaBootstrap can work with
                using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(Func, new DefaultLambdaJsonSerializer()))

                //Instantiate a LambdaBootstrap and  run it 
                using (var bootstrap = new LambdaBootstrap(handlerWrapper))
                {
                    await bootstrap.RunAsync();
                }
            #endif

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message +"\n" + e.Source +"\n" + e.StackTrace);
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
