//Chase Brower, 2023

namespace RSAEncryptionDemo;

public class Program
{
    public static void Main(string[] args)
    {
        string inputBuffer = "";

        while(inputBuffer.ToUpper() != "EXIT")
        {
           Console.Write("Type 'help' for help or enter a command: ");
           inputBuffer = Console.ReadLine() ?? "";

            switch(inputBuffer.ToUpper())
            {
                case "HELP":
                    Console.WriteLine("'RSAProbabilistic' to run a Miller-Rabin based RSA implementation");
                    Console.WriteLine("'RSADeterministic' to run a slower guaranteed prime RSA implementation");
                    break;
                case "RSAPROBABILISTIC":
                    EncryptionScenarios.RunRSAProbabilisticScenario();
                    break;
                case "RSADETERMINISTIC":
                    EncryptionScenarios.RunRSAPreciseScenario();
                    break;
                default:
                    Console.WriteLine("Command not recognized.");
                    break;
            }
            Console.WriteLine();
        }
    }
}