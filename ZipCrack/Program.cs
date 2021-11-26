using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;

namespace ZipCrack
{
    class Program
    {
        enum Message
        {
            Default,
            Error,
            Success,
            Information
        };

        static void WriteLine(string Text, Message type)
        {
            switch(type)
            {
                case Message.Default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Message.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case Message.Success:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case Message.Information:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.WriteLine(Text);
            Console.ResetColor();
        }
        static void DrawLogo()
        {
            Console.WriteLine(@" ______ _         _____                     _    ");
            Console.WriteLine(@"|___  /(_)       /  __ \                   | |   ");
            Console.WriteLine(@"   / /  _  _ __  | /  \/ _ __   __ _   ___ | | __");
            Console.WriteLine(@"  / /  | || '_ \ | |    | '__| / _` | / __|| |/ /");
            Console.WriteLine(@"./ /___| || |_) || \__/\| |   | (_| || (__ |   < ");
            Console.WriteLine(@"\_____/|_|| .__/  \____/|_|    \__,_| \___||_|\_\");
            Console.WriteLine(@"          | |                            by Dayen");
            Console.WriteLine(@"          |_|                                    ");
            Console.WriteLine("");
        }

        static void extractFile(string filePath, string outputFile, string password)
        {
            try
            {
                using (ZipFile archive = new ZipFile(filePath))
                {
                    archive.Password = password;
                    archive.Encryption = EncryptionAlgorithm.PkzipWeak;
                    archive.StatusMessageTextWriter = Console.Out;

                    archive.ExtractAll(outputFile, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch(Exception ex)
            {
                WriteLine("[+] " + ex.Message, Message.Error);
            }
        }
        static void Main(string[] args)
        {
            string[] Passwords;
            string outputPath = Environment.CurrentDirectory + "\\CrackedFiles\\";

            Console.Title = "ZipCrack";

            DrawLogo();

            wordlist:
            WriteLine("[*] Enter wordlist path: ", Message.Default);
            string passwordList = Console.ReadLine();

            if(File.Exists(passwordList))
            {
                Passwords = File.ReadAllLines(passwordList);
                WriteLine($"[!] { Passwords.Count() } passwords found!", Message.Information);

                zipfile:
                WriteLine("[*] Enter ZIP path: ", Message.Default);
                string zipPath = Console.ReadLine();

                if(File.Exists(zipPath))
                {
                    WriteLine($"[!] Output directory {outputPath}", Message.Information);

                    if(!Directory.Exists(outputPath))
                    {
                        Directory.CreateDirectory(outputPath);
                    }

                    string name = zipPath.Split('\\').Last();
                    string type = zipPath.Split('.').Last();
                    long size = new FileInfo(zipPath).Length;


                    WriteLine("\n-- ZIP INFORMATION --", Message.Information);
                    WriteLine($"NAME: {name}", Message.Information);
                    WriteLine($"TYPE: {type}", Message.Information);
                    WriteLine($"SIZE: {size}KB", Message.Information);
                    WriteLine("---------------------\n", Message.Information);

                    WriteLine("[*] Do you want start attack? (y/n)", Message.Default);
                    string response = Console.ReadLine().ToLower();

                    if(response == "y")
                    {
                        WriteLine("[!] Starting attack...", Message.Information);

                        for(int i=0; i < Passwords.Count(); i++)
                        {
                            string currentPassword = Passwords[i];
                            WriteLine($"[!] Testing {i+1} password of {Passwords.Count() } passwords... Password: {currentPassword}", Message.Information);

                            extractFile(zipPath, outputPath, currentPassword);

                        }
                    }
                    else
                    {
                        WriteLine("[*] Attack canceled...", Message.Error);
                        goto wordlist;
                    }
                }
                else
                {
                    WriteLine("[!] ZIP file not found!", Message.Error);
                    goto zipfile;
                }
            }
            else
            {
                WriteLine("[!] Password list not found!", Message.Error);
                goto wordlist;
            }

            Console.Read();
        }
    }
}
