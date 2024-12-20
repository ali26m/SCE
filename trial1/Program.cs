using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Path to the Python script
        string pythonScriptPath = @"C:\\Users\\alihi\\ipynb\\Image Processing\\image-processing-project\\app.py";

        // Path to the image
        string imagePath = @"C:\\Users\\alihi\\ipynb\\Image Processing\\image-processing-project\\skip.jpg";


        // Start the Python process
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "C:\\Users\\alihi\\AppData\\Local\\Programs\\Python\\Python312\\python.exe", // Ensure Python is in PATH
            Arguments = $"\"{pythonScriptPath}\" \"{imagePath}\"", // Pass script and image path
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start process."))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd().Trim(); // Read Python's output
                    Console.WriteLine(result);

                    //if (result == "skip") {
                    //    Console.WriteLine("Skipping forward...");
                    //}
                    if (result == "unknown")
                        Console.WriteLine("unknown");

                    //Perform the action based on the result
                    switch (result)
                    {
                        case "volumeup":
                            Console.WriteLine("Adjusting volume...");
                            break;
                        case "playback":
                            Console.WriteLine("Toggling playback...");
                            break;
                        case "skip":
                            Console.WriteLine("Skipping forward...");
                            break;
                        case "drawback":
                            Console.WriteLine("Skipping backward...");
                            break;
                        case "no_hand_detected":
                            Console.WriteLine("No hand detected.");
                            break;
                        default:
                            Console.WriteLine("Unknown action.");
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
