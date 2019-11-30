using System;
using System.Threading;

using NetworkSnifferLib.UI;

namespace NetworkSnifferLib
{
    class Program
    {
        public const int LIMIT_TIMEOUT = 25;

        static void Main(string[] args)
        {
            try
            {

                IpUI objUI = new IpUI();
                if (objUI.RenderMenu())
                {
                    for (int i = 1; i <= LIMIT_TIMEOUT; i++)
                    {
                        Thread.Sleep(1000);
                    }

                    objUI.GetLogSniff();
                }

            }
            catch (Exception exc)
            {
                throw new Exception("Main exception: "+exc);
            }
            finally
            {
                Console.WriteLine("---------- END --------------");
                
            }
            Console.ReadKey();
        }
    }
}
